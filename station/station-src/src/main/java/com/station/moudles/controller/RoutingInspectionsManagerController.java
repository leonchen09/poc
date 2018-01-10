package com.station.moudles.controller;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.math.BigDecimal;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.Date;
import java.util.HashSet;
import java.util.List;
import java.util.Map.Entry;
import java.util.stream.Collectors;
import java.util.Set;
import java.util.TreeMap;
import java.util.TreeSet;

import org.apache.poi.EncryptedDocumentException;
import org.apache.poi.openxml4j.exceptions.InvalidFormatException;
import org.apache.poi.ss.usermodel.Row;
import org.apache.poi.ss.usermodel.Sheet;
import org.apache.poi.ss.usermodel.Workbook;
import org.apache.poi.ss.usermodel.WorkbookFactory;
import org.springframework.beans.BeanUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.ResponseBody;
import org.springframework.web.multipart.MultipartFile;

import com.station.common.Constant;
import com.station.common.utils.CommonConvertUtils;
import com.station.common.utils.ReflectUtil;
import com.station.common.utils.RowParseHelper;
import com.station.common.utils.StringUtils;
import com.station.moudles.entity.CellInfo;
import com.station.moudles.entity.CellInfoDetail;
import com.station.moudles.entity.GprsConfigInfo;
import com.station.moudles.entity.InspectSignCellIndex;
import com.station.moudles.entity.PackDataInfoLatest;
import com.station.moudles.entity.RoutingInspectionDetail;
import com.station.moudles.entity.RoutingInspectionStationDetail;
import com.station.moudles.entity.RoutingInspections;
import com.station.moudles.entity.RoutingInspectionsStation;
import com.station.moudles.entity.StationInfo;
import com.station.moudles.service.CellInfoService;
import com.station.moudles.service.GprsConfigInfoService;
import com.station.moudles.service.InspectSignCellIndexService;
import com.station.moudles.service.PackDataInfoLatestService;
import com.station.moudles.service.RoutingInspectionDetailService;
import com.station.moudles.service.RoutingInspectionsService;
import com.station.moudles.service.StationInfoService;
import com.station.moudles.service.UserService;
/**
 * 巡检管理
 */
import com.station.moudles.vo.AjaxResponse;
import com.station.moudles.vo.CommonSearchVo;
import com.station.moudles.vo.ShowPage;
import com.station.moudles.vo.search.SearchStationInfoPagingVo;

import io.swagger.annotations.ApiOperation;

@Controller
@RequestMapping(value = "/routionInspectionsManager")
public class RoutingInspectionsManagerController extends BaseController {

	@Autowired
	GprsConfigInfoService gprsConfigInfoSer;
	@Autowired
	StationInfoService stationInfoSer;
	@Autowired
	UserService userSer;
	@Autowired
	CellInfoService cellInfoSer;
	@Autowired
	PackDataInfoLatestService packDataInfoLatestSer;
	@Autowired
	RoutingInspectionsService routingInspectionsSer;
	@Autowired
	RoutingInspectionDetailService routingInspectionDetailSer;
	@Autowired
	InspectSignCellIndexService inspectSignCellIndexSer;

	/**
	 * 展示基本页面
	 */
	@RequestMapping(value = "/station/listPage", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据条件获取巡检记录列表", notes = "返回巡检记录列表")
	public AjaxResponse<ShowPage<RoutingInspectionsStation>> getRoutingInspectionsStationList(
			@RequestBody CommonSearchVo commonSearchVo) {
		SearchStationInfoPagingVo searchStationInfoPagingVo = new SearchStationInfoPagingVo();
		CommonConvertUtils.convertCommonToStationPage(commonSearchVo, searchStationInfoPagingVo);
		List<RoutingInspectionsStation> routingInspectionsList = routingInspectionsSer
				.selectStationListSelectivePaging(searchStationInfoPagingVo);
		ShowPage<RoutingInspectionsStation> page = new ShowPage<RoutingInspectionsStation>(searchStationInfoPagingVo,
				routingInspectionsList);
		AjaxResponse<ShowPage<RoutingInspectionsStation>> ajaxResponse = new AjaxResponse<ShowPage<RoutingInspectionsStation>>(
				page);
		return ajaxResponse;
	}

	/**
	 * 提交信息 确定未完成 传的参数要要有 ID remark operate_id operate_name gprs_id_device
	 * station_id
	 */
	@RequestMapping(value = "/notAccomplishSubmit", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据条件保存没有完成的信息", notes = "根据条件保存没有完成的信息")
	public AjaxResponse<Object> saveNotAccomplishSubmit(@RequestBody RoutingInspectionDetail routingInspectionDetail) {
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>(Constant.RS_CODE_SUCCESS, "确认完成！");
		if (routingInspectionDetail == null) {
			return ajaxResponse;
		}
		try {
			routingInspectionDetailSer.notAccomplishSubmit(routingInspectionDetail);
		} catch (Exception e) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg(e.getMessage());
			return ajaxResponse;
		}
		return ajaxResponse;
/*		RoutingInspectionDetail query = new RoutingInspectionDetail();
		query.setRoutingInspectionsId(routingInspectionDetail.getRoutingInspectionsId());
		query.setRequestType(1);// 后台返回给APP时 状态要改变 0 是没有回应 1 是回应
		List<RoutingInspectionDetail> selectListSelective = routingInspectionDetailSer.selectListSelective(query);
		if (selectListSelective.size() == 0) {
			// 后台返回给APP时 状态要改变 0 是没有回应 1 是回应
			routingInspectionDetail.setRequestType(1);
			routingInspectionDetail.setRequestSeq(1);
			routingInspectionDetail.setRoutingInspectionsId(routingInspectionDetail.getRoutingInspectionsId());
			routingInspectionDetail.setDetailOperateId(routingInspectionDetail.getUserId());
			routingInspectionDetail.setDetailOperateName(routingInspectionDetail.getUserName());
			routingInspectionDetailSer.insertSelective(routingInspectionDetail);
		} else {
			// 得到web回应的最大一条数据
			RoutingInspectionDetail routingDetail = selectListSelective.stream()
					.max(Comparator.comparing(RoutingInspectionDetail::getRequestSeq)).get();
			routingInspectionDetail.setRequestType(1);
			routingInspectionDetail.setRequestSeq(routingDetail.getRequestSeq() + 1);
			routingInspectionDetail.setRoutingInspectionsId(routingInspectionDetail.getRoutingInspectionsId());
			routingInspectionDetail.setDetailOperateId(routingInspectionDetail.getUserId());
			routingInspectionDetail.setDetailOperateName(routingInspectionDetail.getUserName());
			routingInspectionDetailSer.insertSelective(routingInspectionDetail);
		}
		// 确定未完成改变电池组的状态
		StationInfo station = new StationInfo();
		station.setId(routingInspectionDetail.getStationId());
		StationInfo stationinfo = stationInfoSer.selectByPrimaryKey(routingInspectionDetail.getStationId());
		if (stationinfo != null) {
			// 如果主机的状态是11安装中等待确认状态 改为12 安装中后台确认未完成状态
			if (stationinfo.getInspectStatus() == 11) {
				station.setInspectStatus(12);
				stationInfoSer.updateByPrimaryKeySelective(station);
			}
			// 如果主机的状态是21维护中等待确认状态改为22维护中后台确认未完成状态
			if (stationinfo.getInspectStatus() == 21) {
				station.setInspectStatus(22);
				stationInfoSer.updateByPrimaryKeySelective(station);
			}
			//如果主机状态是23跟换单体等待确定状态 24 跟换单体后台确定未完成状态
			if (stationinfo.getInspectStatus() == 23) {
				station.setInspectStatus(24);
				stationInfoSer.updateByPrimaryKeySelective(station);
			}
		}*/
//		ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
//		ajaxResponse.setMsg("提交成功！");
//		return ajaxResponse;

	}

	/**
	 * 后台确定完成信息
	 */
	@RequestMapping(value = "/confirmRoutingInspection", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "确定完成", notes = "确定完成")
	public AjaxResponse<RoutingInspectionDetail> confirmRoutingInspection(
			@RequestBody RoutingInspectionDetail routingInspectionDetail) {
		if (routingInspectionDetail == null) {
			return new AjaxResponse<RoutingInspectionDetail>(Constant.RS_CODE_ERROR, "请选择确认！");
		}

		AjaxResponse<RoutingInspectionDetail> ajaxResponse = new AjaxResponse<RoutingInspectionDetail>(Constant.RS_CODE_SUCCESS, "确认完成成功！");
		try {
			routingInspectionDetailSer.confirmRoutingInspection(routingInspectionDetail);

		} catch (Exception e) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg(e.getMessage());
			return ajaxResponse;
		}
		
		/*if (routingInspectionDetail.getRoutingInspectionsId() != null) {
			// 更新确定列表的状态 改为确定成功
			RoutingInspections routingInspections = new RoutingInspections();			
			// 得到Date类型当前时间
			Date date = new Date();
			routingInspections.setOperateTime(date);	
			routingInspections.setRoutingInspectionStatus(2);// 2 确定完成
			routingInspections.setRoutingInspectionId(routingInspectionDetail.getRoutingInspectionsId());
			routingInspections.setConfirmOperateId(routingInspectionDetail.getUserId());
			routingInspections.setConfirmOperateName(routingInspectionDetail.getUserName());
			routingInspectionsSer.updateByPrimaryKeySelective(routingInspections);
			// 添加确记录
			RoutingInspectionDetail query = new RoutingInspectionDetail();
			query.setRoutingInspectionsId(routingInspectionDetail.getRoutingInspectionsId());
			query.setRequestType(1);// 后台返回给APP时 状态要改变 0 是没有回应 1 是回应
			List<RoutingInspectionDetail> routingDetail = routingInspectionDetailSer.selectListSelective(query);
			if(routingDetail.size() == 0) {
				routingInspectionDetail.setRequestSeq(1);
			}else {
				RoutingInspectionDetail detailLast = routingDetail.stream().max(Comparator.comparing(RoutingInspectionDetail::getRequestSeq)).get();
				routingInspectionDetail.setRequestSeq(detailLast.getRequestSeq()+1);
			}
			routingInspectionDetail.setRequestType(1);// 后台确认后的状态 设置为1
			routingInspectionDetail.setDetailOperateId(routingInspectionDetail.getUserId());
			routingInspectionDetail.setDetailOperateName(routingInspectionDetail.getUserName());
			routingInspectionDetail.setRemark("确认完成");
			routingInspectionDetailSer.insertSelective(routingInspectionDetail);

			// 确完成后改变主机的状态 30 已安装
			StationInfo station = new StationInfo();
			RoutingInspections routingInspec = routingInspectionsSer.selectByPrimaryKey(routingInspectionDetail.getRoutingInspectionsId());
			station.setId(routingInspec.getStationId());
			station.setInspectStatus(30);
			stationInfoSer.updateByPrimaryKeySelective(station);
			//确完成后记录标记需要更换的单体
			RoutingInspectionDetail querySignCell = new RoutingInspectionDetail();
			querySignCell.setRoutingInspectionsId(routingInspectionDetail.getRoutingInspectionsId());
			querySignCell.setDetailOperateType(7);//标记单体
			List<RoutingInspectionDetail> routingCellinfo = routingInspectionDetailSer.selectListSelective(querySignCell);
			if(routingCellinfo.size() != 0) {
				Set<Integer> cells = new TreeSet<Integer>();
				for(RoutingInspectionDetail cellIndex : routingCellinfo){
					cells.add(cellIndex.getCellIndex());
				}
				// 巡检标记单体表添加
				for (Integer index : cells) {
				InspectSignCellIndex inspectSignCell = new InspectSignCellIndex();
				inspectSignCell.setRoutingInspectionId(routingInspectionDetail.getRoutingInspectionsId());
				inspectSignCell.setCellIndex(index);
				inspectSignCellIndexSer.insert(inspectSignCell);
				}
			}
			//确定完成后修改该更换单体信息
			querySignCell.setRoutingInspectionsId(routingInspectionDetail.getRoutingInspectionsId());
			querySignCell.setDetailOperateType(5);//更换单体类型
			List<RoutingInspectionDetail> inspectionCellinfoListType = routingInspectionDetailSer.selectListSelective(querySignCell);
			List<Integer> indexType = new ArrayList<Integer>();//得到跟换单体类型的单体号
			boolean isInsert = true;
			if(inspectionCellinfoListType.size() != 0) {
				for(RoutingInspectionDetail inspectionCellType : inspectionCellinfoListType) {
					RoutingInspectionStationDetail stationDetail = new RoutingInspectionStationDetail();
					indexType.add(inspectionCellType.getCellIndex());
					stationDetail.setGprsId(routingInspec.getGprsId());
					stationDetail.setStationId(routingInspec.getStationId());
					stationDetail.setOperateTime(inspectionCellType.getCreateTime());
					cellInfoSer.appUpdateCellInfo(stationDetail,inspectionCellType,inspectionCellType.getDetailOperateValueNew(),isInsert);
				}
			}
			querySignCell.setDetailOperateType(6);//更换单体品牌
			List<RoutingInspectionDetail> inspectionCellinfoListPant = routingInspectionDetailSer.selectListSelective(querySignCell);
		
			if(inspectionCellinfoListPant.size() != 0) {
				for(RoutingInspectionDetail inspectionCell : inspectionCellinfoListPant) {
					RoutingInspectionStationDetail stationDetail = new RoutingInspectionStationDetail();
					
					for(Integer index :indexType) {
						if(index == inspectionCell.getCellIndex()) {
							isInsert = false;
							continue;
						};
					
					}					
					stationDetail.setGprsId(routingInspec.getGprsId());
					stationDetail.setStationId(routingInspec.getStationId());
					stationDetail.setOperateTime(inspectionCell.getCreateTime());
					cellInfoSer.appUpdateCellInfo(stationDetail,inspectionCell,inspectionCell.getDetailOperateValueNew(),isInsert);
				}
			}
		} else {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("确认完成失败！");
		}*/
		return ajaxResponse;

	}

	/**
	 * 点击确定显示操作管理详情 展示所有的信息
	 */
	@RequestMapping(value = "/routingInspectionDetail/{routingInspectionId}", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据pk获取巡检记录", notes = "根据pk获取巡检记录")
	public AjaxResponse<RoutingInspectionStationDetail> getRoutingInspectionDetail(
			@PathVariable Integer routingInspectionId) {
		if (routingInspectionId == null) {
			return new AjaxResponse<RoutingInspectionStationDetail>(Constant.RS_CODE_ERROR, "请选择pk！");
		}
		// 得到基站的基本信息
		RoutingInspectionStationDetail routingInspections = routingInspectionsSer
				.selectStationDetailByPrimaryKey(routingInspectionId);
		// 展示app端提交的操作记录
		RoutingInspectionDetail routingInspectionDetail = new RoutingInspectionDetail();
		routingInspectionDetail.setRoutingInspectionsId(routingInspectionId);
		routingInspectionDetail.setRequestType(0);// 0 是后台没有返回的
		List<RoutingInspectionDetail> routingInspectionDetailList = routingInspectionDetailSer
				.selectListSelectiveApp(routingInspectionDetail);
		if (routingInspectionDetailList.size() != 0) {
			//过滤掉品牌为null的
//			List<RoutingInspectionDetail> collect = new ArrayList<RoutingInspectionDetail>();
//			for(RoutingInspectionDetail detail :routingInspectionDetailList) {
//				if(detail.getDetailOperateValueNew() !=null) {
//					collect.add(detail);
//				}
//			}
			routingInspections.setRoutingInspectionDetailList(routingInspectionDetailList);
		}

		// 获得24个单体的信息
		if (!routingInspections.getGprsId().equals("-1")) {
			GprsConfigInfo queryGprsConfigInfo = new GprsConfigInfo();
			queryGprsConfigInfo.setGprsId(routingInspections.getGprsId());

			List<GprsConfigInfo> configList = gprsConfigInfoSer.selectListSelective(queryGprsConfigInfo);
			GprsConfigInfo c = null;
			if (configList.size() > 0) {
				c = configList.get(0);
				routingInspections.setLinkStatus(c.getLinkStatus());
				routingInspections.setDeviceType(c.getDeviceType());
				routingInspections.setDevicePhone(c.getDevicePhone());
				routingInspections.setGprsPort(c.getGprsPort());
				routingInspections.setGprsSpec(c.getGprsSpec());
			}
			PackDataInfoLatest queryPdi = new PackDataInfoLatest();
			queryPdi.setGprsId(routingInspections.getGprsId());
			List<PackDataInfoLatest> pdiList = packDataInfoLatestSer.selectListSelective(queryPdi);
			PackDataInfoLatest pdi = null;
			if (pdiList.size() > 0) {
				pdi = pdiList.get(0);

				routingInspections.setState(pdi.getState());
				routingInspections.setGenCur(pdi.getGenCur());
				routingInspections.setGenVol(pdi.getGenVol());
				routingInspections.setEnvironTem(pdi.getEnvironTem());
				// routingInspections.setPackDataInfoLatest(pdi);
			}
			// 将24个单体封装成一个数组
			CellInfo queryCell = new CellInfo();
			queryCell.setStationId(routingInspections.getStationId());
			queryCell.setGprsId(routingInspections.getGprsId());
			List<CellInfo> cellList = cellInfoSer.selectListSelective(queryCell);

			List<CellInfoDetail> cellDetailList = new ArrayList<CellInfoDetail>();
			for (CellInfo cell : cellList) {
				CellInfoDetail cellDetail = new CellInfoDetail();
				BeanUtils.copyProperties(cell, cellDetail);

				if (pdi != null) {
					cellDetail.setCellVol(
							(BigDecimal) ReflectUtil.getValueByKey(pdi, "cellVol" + cellDetail.getCellIndex()));
					cellDetail.setCellEqu((Byte) ReflectUtil.getValueByKey(pdi, "cellEqu" + cellDetail.getCellIndex()));
					cellDetail.setCellTem(
							(Integer) ReflectUtil.getValueByKey(pdi, "cellTem" + cellDetail.getCellIndex()));
				}
				cellDetailList.add(cellDetail);
			}
			routingInspections.setCellInfoDetailList(cellDetailList);
		}
		AjaxResponse<RoutingInspectionStationDetail> ajaxResponse = new AjaxResponse<RoutingInspectionStationDetail>(
				Constant.RS_CODE_SUCCESS, "获取操作详情成功！");
		if (routingInspections != null) {
			ajaxResponse.setData(routingInspections);
		} else {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("获取操作详情失败！");
		}
		return ajaxResponse;

	}

	/**
	 * 巡检记录详情
	 */
	@RequestMapping(value = "/routingInspectionDetail/entity/{routingInspectionId}", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据pk获取巡检记录", notes = "根据pk获取巡检记录")
	public AjaxResponse<RoutingInspectionStationDetail> getRoutingInspectionDetailEntity(
			@PathVariable Integer routingInspectionId) {
		if (routingInspectionId == null) {
			return new AjaxResponse<RoutingInspectionStationDetail>(Constant.RS_CODE_ERROR, "请选择pk！");
		}
		RoutingInspectionStationDetail routingInspections = routingInspectionsSer
				.selectStationDetailByPrimaryKey(routingInspectionId);

		// 获取详情列表
		Integer routingInspectionsId = routingInspections.getRoutingInspectionId();
		List<RoutingInspectionDetail> routingInspectionDetailList = routingInspectionDetailSer
				.selectStationSelective(routingInspectionsId);
		// 按 cellIndex 升序
		routingInspectionDetailList.sort(( r1 , r2 ) -> 
					(r1.getCellIndex() == null ? new Integer(-1) : r1.getCellIndex())
					.compareTo(r2.getCellIndex() == null ? new Integer(-1) : r2.getCellIndex()));
		routingInspections.setRoutingInspectionDetailList(routingInspectionDetailList);
		AjaxResponse<RoutingInspectionStationDetail> ajaxResponse = new AjaxResponse<RoutingInspectionStationDetail>(
				Constant.RS_CODE_SUCCESS, "获取巡检记录成功！");
		if (routingInspections != null) {
			ajaxResponse.setData(routingInspections);
		} else {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("获取巡检记录失败！");
		}
		return ajaxResponse;
	}

	@ResponseBody
	@RequestMapping(value = "/fileImportCell", method = RequestMethod.POST)
	public AjaxResponse<Object> fileImportCell(@RequestParam MultipartFile file, Integer companyId)
			throws IOException, EncryptedDocumentException, InvalidFormatException {
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>(Constant.RS_CODE_ERROR, "文件导入失败");
		request.setAttribute(Constant.ERROR_REQUEST, ajaxResponse);
		if (!file.isEmpty()) {
			InputStream in = null;
			OutputStream out = null;
			File dir = new File("tmpFiles");
			if (!dir.exists()) {
				dir.mkdirs();
			}
			File serverFile = new File(dir.getAbsolutePath() + File.separator + System.currentTimeMillis());
			in = file.getInputStream();
			out = new FileOutputStream(serverFile);
			byte[] b = new byte[1024];
			int len = 0;
			while ((len = in.read(b)) > 0) {
				out.write(b, 0, len);
			}
			out.close();
			in.close();
			logger.info("Server File Location=" + serverFile.getAbsolutePath());
			cellFile(serverFile, ajaxResponse, companyId);
			return ajaxResponse;
		} else {
			ajaxResponse.setMsg("文件为空！");
			return ajaxResponse;
		}
	}

	public boolean cellFile(File file, @SuppressWarnings("rawtypes") AjaxResponse ajaxResponse, Integer companyId)
			throws EncryptedDocumentException, FileNotFoundException, InvalidFormatException, IOException {
		// 导入巡检记录，问题行详细记录信息
		TreeMap<Integer, String> errorRow = new TreeMap<>();
		try {
			int rowNum = 1;
			int successCount = 0;
			InputStream inp = new FileInputStream(file);
			Workbook wb = WorkbookFactory.create(inp);
			Sheet sheet = wb.getSheetAt(0);
			for (Row row : sheet) {
				rowNum = row.getRowNum();
				if (rowNum >= 6) {
					if (!RowParseHelper.hasData(row, 79)) {
						// 解析当前行有没有数据
						break;
					}
					try {
						routingInspectionDetailSer.routingInsepectionExcelFileCell(row);
						successCount++;
					} catch (Exception e) {
						errorRow.put(rowNum + 1, e.getMessage());
					}
				}
			}
			StringBuffer sb = new StringBuffer();
			if (errorRow.size() > 0) {
				for (Entry<Integer, String> et : errorRow.entrySet()) {
					String error = "第" + et.getKey() + "行，" + et.getValue() + "\r\n";
					sb.append(error);
				}
			}
			ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
			ajaxResponse.setMsg("电池记录导入处理完成，共导入" + (successCount) + "行数据\r\n" + sb.toString());
			logger.info("parse RoutionInspections excel file over!");
		} catch (Exception e) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			StringBuffer sb = new StringBuffer();
			if (errorRow.size() > 0) {
				for (Entry<Integer, String> et : errorRow.entrySet()) {
					String error = "第" + et.getKey() + "行，" + et.getValue() + "\r\n";
					sb.append(error);
				}
			}
			ajaxResponse.setMsg("电池记录导入失败\r\n" + sb.toString());
			ajaxResponse.getMsg();
			logger.error("电池记录导入出错-->", e.getMessage());
		}
		return true;
	}

	/**
	 * 巡检记录导入
	 */
	@Override
	public boolean parseFile(File file, @SuppressWarnings("rawtypes") AjaxResponse ajaxResponse, Integer companyId)
			throws EncryptedDocumentException, FileNotFoundException, InvalidFormatException, IOException {
		TreeMap<Integer, String> errorRow = new TreeMap<>();
		try {
			int rowNum = 1;
			int successCount = 0;
			String str = "";
			InputStream inp = new FileInputStream(file);
			Workbook wb = WorkbookFactory.create(inp);
			Sheet sheet = wb.getSheetAt(0);
			for (Row row : sheet) {
				rowNum = row.getRowNum();
				boolean type1 = false;
				boolean type2 = false;
				if(rowNum == 4) {
					if (StringUtils.isNull(row.getCell(35))) {
						str="";
					}else {
						str = row.getCell(35).toString();
					}
				}				
				if (rowNum >= 6) {
					if (!RowParseHelper.hasData(row, 36)) {
						// 解析当前行有没有数据
						break;
					}
					try {
					//如果再cell(35)出不为空并且是蓄电池12V监测设备 ；说明模板内填写错误的设备类型
					if("蓄电池12V监测设备".equals(row.getCell(2).toString()) && str.length() !=0 ) {
						successCount++;
						type1 = true;
					}
					if(type1) {
						throw new RuntimeException("模板中是设备类型填写与该模板不对应！");
					}
					if(!"蓄电池12V监测设备".equals(row.getCell(2).toString()) && str.length() ==0 ) {
						successCount++;
						type2 = true;
					}
					if(type2) {
						throw new RuntimeException("模板中是设备类型填写与该模板不对应！");
					}
					
						routingInspectionDetailSer.routingInsepectionExcelFile(row);
						successCount++;
					} catch (Exception e) {
						errorRow.put(rowNum + 1, e.getMessage());
					}
				}
			}
			StringBuffer sb = new StringBuffer();
			if (errorRow.size() > 0) {
				for (Entry<Integer, String> et : errorRow.entrySet()) {
					String error = "第" + et.getKey() + "行，" + et.getValue() + "\r\n";
					sb.append(error);
				}
			}
			ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
			ajaxResponse.setMsg("巡检记录导入处理完成，共导入" + (successCount) + "行数据\r\n" + sb.toString());
			logger.info("parse RoutionInspections excel file over!");
		} catch (Exception e) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			StringBuffer sb = new StringBuffer();
			if (errorRow.size() > 0) {
				for (Entry<Integer, String> et : errorRow.entrySet()) {
					String error = "第" + et.getKey() + "行，" + et.getValue() + "\r\n";
					sb.append(error);
				}
			}
			ajaxResponse.setMsg("巡检记录导入失败\r\n" + sb.toString());
			ajaxResponse.getMsg();
			logger.error("巡检记录导入出错-->", e.getMessage());
		}
		return true;
	}
}
