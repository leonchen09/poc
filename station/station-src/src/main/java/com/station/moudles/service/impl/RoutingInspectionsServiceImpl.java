package com.station.moudles.service.impl;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import org.apache.poi.EncryptedDocumentException;
import org.apache.poi.openxml4j.exceptions.InvalidFormatException;
import org.apache.poi.ss.usermodel.Row;
import org.apache.poi.ss.usermodel.Sheet;
import org.apache.poi.ss.usermodel.Workbook;
import org.apache.poi.ss.usermodel.WorkbookFactory;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.BeanUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.station.common.Constant;
import com.station.common.utils.CommonConvertUtils;
import com.station.common.utils.MD5;
import com.station.common.utils.MyDateUtils;
import com.station.common.utils.StringUtils;
import com.station.moudles.entity.CellInfo;
import com.station.moudles.entity.RoutingInspectionDetail;
import com.station.moudles.entity.RoutingInspectionStationDetail;
import com.station.moudles.entity.RoutingInspections;
import com.station.moudles.entity.RoutingInspectionsStation;
import com.station.moudles.entity.StationInfo;
import com.station.moudles.entity.User;
import com.station.moudles.mapper.RoutingInspectionsMapper;
import com.station.moudles.service.CommonService;
import com.station.moudles.service.GprsConfigInfoService;
import com.station.moudles.service.RoutingInspectionDetailService;
import com.station.moudles.service.RoutingInspectionsService;
import com.station.moudles.service.StationInfoService;
import com.station.moudles.service.UserService;
import com.station.moudles.vo.AjaxResponse;
import com.station.moudles.vo.search.SearchStationInfoPagingVo;

@Service
public class RoutingInspectionsServiceImpl extends BaseServiceImpl<RoutingInspections, Integer>
		implements RoutingInspectionsService {
	@Autowired
	RoutingInspectionsMapper routingInspectionsMapper;
	@Autowired
	CommonService commonSer;
	@Autowired
	StationInfoService stationInfoSer;
	@Autowired
	GprsConfigInfoService gprsConfigInfoSer;
	@Autowired
	UserService userSer;
	@Autowired
	RoutingInspectionDetailService routingInspectionDetailSer;
	private static final Logger logger = LoggerFactory.getLogger(RoutingInspectionsServiceImpl.class);

	@Override
	public boolean parseRoutingInspectionExcelFile(File file, AjaxResponse ajaxResponse)
			throws EncryptedDocumentException, InvalidFormatException, IOException {
		logger.info("start parse Routing Inspection excel file!");
		int rowNum = 1;
		InputStream inp = new FileInputStream(file);
		Workbook wb = WorkbookFactory.create(inp);
		Sheet sheet = wb.getSheetAt(0);
		for (Row row : sheet) {
			rowNum = row.getRowNum();
			if (rowNum > 4) {
				ajaxResponse.setMsg("文件导入失败，第" + (rowNum + 1) + "行数据有问题。");
				StationInfo queryStationInfo = new StationInfo();
				queryStationInfo.setName(row.getCell(0).toString());
				queryStationInfo
						.setOperatorType(CommonConvertUtils.convertStrToStationOperatorType(row.getCell(1).toString()));
				List<StationInfo> queryStationList = stationInfoSer.selectListSelective(queryStationInfo);
				if (queryStationList.size() == 0) {
					queryStationInfo.setAddress(row.getCell(2).toString());
					queryStationInfo.setMaintainanceId(row.getCell(3).toString());
					stationInfoSer.createStationCells(queryStationInfo);
				} else {
					queryStationInfo = queryStationList.get(0);
				}
				String userPhone = StringUtils.getString(row.getCell(158));
				String appUserName = StringUtils.getString(row.getCell(156));
				User appUser = new User();
				if (userPhone != null) {
					appUser.setUserPhone(userPhone);
					appUser.setUserType(3);
					List<User> appUserList = userSer.selectListSelective(appUser);
					if (appUserList.size() == 0) {
						appUser.setCreateName("system");
						appUser.setLoginId(appUser.getUserPhone());
						appUser.setRemarks("线下维护记录导入创建");
						appUser.setUserName(appUserName);
						appUser.setUserPassword(MD5.toMD5(appUser.getUserPhone()));
						userSer.insertSelective(appUser);
					} else {
						appUser = appUserList.get(0);
					}
				}
				Integer deviceOprateType = CommonConvertUtils.convertDeviceOperateType(row.getCell(6).toString());
				RoutingInspections routingInspections = new RoutingInspections();
				routingInspections.setOperateType(deviceOprateType);
				routingInspections.setOperateId(appUser.getUserId());
				routingInspections.setOperateName(appUser.getUserName());
				Date operateTime = MyDateUtils.parseDate(row.getCell(155).toString());
				routingInspections.setOperateTime(operateTime);
				routingInspections.setStationId(queryStationInfo.getId());
				routingInspections.setGprsId(StringUtils.getString(row.getCell(4)));
				List<RoutingInspectionDetail> riDetailList = new ArrayList<RoutingInspectionDetail>();
				StationInfo updateStationInfo = new StationInfo();
				updateStationInfo.setId(queryStationInfo.getId());
				RoutingInspectionDetail commonriDetail = new RoutingInspectionDetail();
//				commonriDetail.setDetailOperatorType(1);
				commonriDetail.setDetailOperateId(appUser.getUserId());
				commonriDetail.setDetailOperateName(appUser.getUserName());
				if (deviceOprateType.equals(1)) {
					updateStationInfo.setGprsId(row.getCell(7).toString());
					updateStationInfo.setGprsIdOut(row.getCell(7).toString());
					stationInfoSer.updateByPrimaryKeySelective(updateStationInfo);
					routingInspections.setRoutingInspectionStatus(1);
					RoutingInspectionDetail riDetail = new RoutingInspectionDetail();
					BeanUtils.copyProperties(commonriDetail, riDetail);
					riDetail.setDetailOperateType(1);
					riDetailList.add(riDetail);
				} else if (deviceOprateType.equals(2)) {
					routingInspections.setRoutingInspectionStatus(2);
					// 更换主机
					changeMasterDevice(commonriDetail, row, riDetailList);
					// 霍尔感应器
					checkHall(commonriDetail, row, riDetailList);
					// 更换从机
					changeSubDevice(commonriDetail, row, riDetailList);
				} else if (deviceOprateType.equals(3)) {
					routingInspections.setRoutingInspectionStatus(3);
					// 更换主机
					changeMasterDevice(commonriDetail, row, riDetailList);
					// 霍尔感应器
					checkHall(commonriDetail, row, riDetailList);
					// 更换从机
					changeSubDevice(commonriDetail, row, riDetailList);
					// 更换电池单体
					changeCell(commonriDetail, row, riDetailList);
					// 更换电池品牌
					changeCellPlant(commonriDetail, row, riDetailList);
				}
				insertSelective(routingInspections);
				logger.debug(routingInspections.getRoutingInspectionId() + "!!!!!!!!!!!!!!!!!!!");
				// insert detail
				for (RoutingInspectionDetail riDetail : riDetailList) {
					riDetail.setRoutingInspectionsId(routingInspections.getRoutingInspectionId());
					routingInspectionDetailSer.insertSelective(riDetail);
				}
			}
		}
		ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
		ajaxResponse.setMsg("文件导入成功");
		return true;
	}

	public void changeMasterDevice(RoutingInspectionDetail commonriDetail, Row row,
			List<RoutingInspectionDetail> riDetailList) {
		if (!StringUtils.isNull(row.getCell(9))) {
			RoutingInspectionDetail riDetail = new RoutingInspectionDetail();
			BeanUtils.copyProperties(commonriDetail, riDetail);
			// riDetail.setRoutingInspectionsId(routingInspectionsId);
			riDetail.setDetailOperateType(2);
			riDetail.setDetailOperateValueOld(row.getCell(8).toString());
			riDetail.setDetailOperateValueNew(row.getCell(9).toString());
			riDetailList.add(riDetail);
		}
	}

	public void checkHall(RoutingInspectionDetail commonriDetail, Row row, List<RoutingInspectionDetail> riDetailList) {
		if (!StringUtils.isNull(row.getCell(10))) {
			if (row.getCell(10).toString().equals("是")) {
				RoutingInspectionDetail riDetail = new RoutingInspectionDetail();
				BeanUtils.copyProperties(commonriDetail, riDetail);
				riDetail.setDetailOperateType(3);
				riDetailList.add(riDetail);
			}
		}
	}

	public void changeSubDevice(RoutingInspectionDetail commonriDetail, Row row,
			List<RoutingInspectionDetail> riDetailList) {
		for (int i = 0; i < 24; i++) {
			if (!StringUtils.isNull(row.getCell(11 + 2 * i)) && !StringUtils.isNull(row.getCell(12 + 2 * i))) {
				RoutingInspectionDetail riDetail = new RoutingInspectionDetail();
				BeanUtils.copyProperties(commonriDetail, riDetail);
				riDetail.setDetailOperateType(4);
				riDetail.setDetailOperateValueOld(row.getCell(11 + 2 * i).toString());
				riDetail.setDetailOperateValueNew(row.getCell(12 + 2 * i).toString());
				riDetail.setCellIndex(i + 1);
				riDetailList.add(riDetail);
			}
		}
	}

	public void changeCell(RoutingInspectionDetail commonriDetail, Row row,
			List<RoutingInspectionDetail> riDetailList) {
		for (int i = 0; i < 24; i++) {
			if (!StringUtils.isNull(row.getCell(59 + 2 * i)) || !StringUtils.isNull(row.getCell(60 + 2 * i))) {
				RoutingInspectionDetail riDetail = new RoutingInspectionDetail();
				BeanUtils.copyProperties(commonriDetail, riDetail);
				riDetail.setDetailOperateType(5);
				riDetail.setDetailOperateValueOld(row.getCell(59 + 2 * i).toString());
				riDetail.setDetailOperateValueNew(row.getCell(60 + 2 * i).toString());
				riDetail.setCellIndex(i + 1);
				riDetailList.add(riDetail);
			}
		}
	}

	public void changeCellPlant(RoutingInspectionDetail commonriDetail, Row row,
			List<RoutingInspectionDetail> riDetailList) {
		for (int i = 0; i < 24; i++) {
			if (!StringUtils.isNull(row.getCell(107 + 2 * i)) || !StringUtils.isNull(row.getCell(108 + 2 * i))) {
				RoutingInspectionDetail riDetail = new RoutingInspectionDetail();
				BeanUtils.copyProperties(commonriDetail, riDetail);
				riDetail.setDetailOperateType(6);
				riDetail.setDetailOperateValueOld(row.getCell(107 + 2 * i).toString());
				riDetail.setDetailOperateValueNew(row.getCell(108 + 2 * i).toString());
				riDetail.setCellIndex(i + 1);
				riDetailList.add(riDetail);
			}
		}
	}

	@Override
	public List<RoutingInspectionsStation> selectStationListSelectivePaging(
			SearchStationInfoPagingVo searchStationInfoPagingVo) {
		return routingInspectionsMapper.selectStationListSelectivePaging(searchStationInfoPagingVo);
	}

	@Override
	public RoutingInspectionStationDetail selectStationDetailByPrimaryKey(Integer pk) {
		return routingInspectionsMapper.selectStationDetailByPrimaryKey(pk);
	}

	@Override
	public void updateRoutingInspectionStationDetail(RoutingInspectionStationDetail routingInspectionStationDetail) {
		RoutingInspectionDetail delRoutingInspectionDetail = new RoutingInspectionDetail();
		delRoutingInspectionDetail.setRoutingInspectionsId(routingInspectionStationDetail.getRoutingInspectionId());
		routingInspectionDetailSer.deleteSelective(delRoutingInspectionDetail);
		RoutingInspections updateRoutingInspections = new RoutingInspections();
		BeanUtils.copyProperties(routingInspectionStationDetail, updateRoutingInspections);
		updateByPrimaryKeySelective(updateRoutingInspections);
		for (RoutingInspectionDetail rDetail : routingInspectionStationDetail.getRoutingInspectionDetailList()) {
			rDetail.setRoutingInspectionsId(routingInspectionStationDetail.getRoutingInspectionId());
			routingInspectionDetailSer.insertSelective(rDetail);
		}
	}

	@Override
	public List<RoutingInspections> selectCellInspace(RoutingInspections routingInspections) {
		
		return routingInspectionsMapper.selectCellInspace(routingInspections);
	}

	@Override
	public List<RoutingInspections> selectListSelectiveFirst(RoutingInspections routingInspections) {
		
		return routingInspectionsMapper.selectListSelectiveFirst(routingInspections);
	}

}