package com.station.moudles.service.impl;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Map;

import org.apache.poi.ss.usermodel.DateUtil;
import org.apache.poi.ss.usermodel.Row;
import org.springframework.beans.BeanUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.google.common.collect.Maps;
import com.station.common.utils.RowParseHelper;
import com.station.common.utils.StringUtils;
import com.station.moudles.entity.GprsConfigInfo;
import com.station.moudles.entity.InspectSignCellIndex;
import com.station.moudles.entity.RoutingInspectionDetail;
import com.station.moudles.entity.RoutingInspectionStationDetail;
import com.station.moudles.entity.RoutingInspections;
import com.station.moudles.entity.StationInfo;
import com.station.moudles.mapper.GprsConfigInfoMapper;
import com.station.moudles.mapper.RoutingInspectionDetailMapper;
import com.station.moudles.service.CellInfoService;
import com.station.moudles.service.GprsConfigInfoService;
import com.station.moudles.service.InspectSignCellIndexService;
import com.station.moudles.service.ModifyGprsidSendService;
import com.station.moudles.service.RoutingInspectionDetailService;
import com.station.moudles.service.RoutingInspectionsService;
import com.station.moudles.service.StationInfoService;
import com.station.moudles.service.SubDeviceService;
import com.station.moudles.service.UserService;

@Service
public class RoutingInspectionDetailServiceImpl extends BaseServiceImpl<RoutingInspectionDetail, Integer>
		implements RoutingInspectionDetailService {
	public final static char SEPARATER = ',';
	@Autowired
	RoutingInspectionDetailMapper routingInspectionDetailMapper;
	@Autowired
	RoutingInspectionsService routingInspectionsSer;
	@Autowired
	RoutingInspectionDetailService routingInspectionDetailSer;
	@Autowired
	StationInfoService stationInfoSer;
	@Autowired
	GprsConfigInfoMapper gprsConfigInfoMapper;
	@Autowired
	ModifyGprsidSendService modifyGprsidSendSer;
	@Autowired
	GprsConfigInfoService gprsConfigInfoSer;
	@Autowired
	UserService userSer;
	@Autowired
	CellInfoService cellInfoSer;
	@Autowired
	SubDeviceService subDeviceSer;
	@Autowired
	InspectSignCellIndexService inspectSignCellIndexSer;

	@Override
	public List<RoutingInspectionDetail> selectStationSelective(Integer ids) {
		List<Integer> list = new ArrayList<Integer>();
		list.add(ids);
		Map<String, Object> m = Maps.newHashMap();
		m.put("ids", list);
		return routingInspectionDetailMapper.selectDetailByInspectionIds(m);
	}

	@Override
	public List<RoutingInspectionDetail> selectListSelectiveApp(RoutingInspectionDetail routingInspectionDetail) {

		return routingInspectionDetailMapper.selectListSelectiveApp(routingInspectionDetail);
	}

	/**
	 * 巡检记录导入
	 */
	@Override
	public void routingInsepectionExcelFile(Row row) {
		if (StringUtils.isNull(row.getCell(0))) {
			throw new RuntimeException("基站名称必填项！");
		}
		if (StringUtils.isNull(row.getCell(1))) {
			throw new RuntimeException("设备编号必填项！");
		}
		// 查询电池组通过基站名称和设备编号
		StationInfo query = new StationInfo();
		query.setName(row.getCell(0).toString().trim());
		query.setGprsId(row.getCell(1).toString().trim());
		List<StationInfo> station = stationInfoSer.selectListSelective(query);
		if (station == null || station.size() == 0) {
			// 提示该列电池组不存在
			throw new RuntimeException("基站：" + query.getName() + "或设备编号：" + query.getGprsId() + "不存在！");
		}

		RoutingInspectionStationDetail routingDetail = new RoutingInspectionStationDetail();
		routingDetail.setStationId(station.get(0).getId());
		routingDetail.setGprsId(station.get(0).getGprsId());
		//通过gprsid查询得到设备类型
		GprsConfigInfo queryGprsConfig = new GprsConfigInfo();
		queryGprsConfig.setGprsId(station.get(0).getGprsId());
		List<GprsConfigInfo> gprs = gprsConfigInfoSer.selectListSelective(queryGprsConfig);
		if(gprs.size() != 0) {
			routingDetail.setDeviceType(gprs.get(0).getDeviceType());
		}
		// 操作时间
		if (!StringUtils.isNull(row.getCell(4))) {
			// 判断是否为日期类型
			if (0 == row.getCell(4).getCellType()) {
				if (DateUtil.isCellDateFormatted(row.getCell(4))) {
					// 用于转化为日期格式
					Date date = row.getCell(4).getDateCellValue();
					routingDetail.setOperateTime(date);
				}
			} else {
				throw new RuntimeException("操作日期填写格式不正确，正确的格式是：年/月/日 或者 年/月/日 时:分:秒");
			}
		} else {
			Date time = new Date();
			routingDetail.setOperateTime(time);
		}
		// 操作人员
		if (!StringUtils.isNull(row.getCell(5))) {
			String cell = RowParseHelper.getCell(row, 5);
			if (cell.length() <= 30) {
				routingDetail.setOperateName(cell);
			} else {
				throw new RuntimeException("操作人员的名字不能超过30个字符");
			}
		}
		// 电话号码
		if (!StringUtils.isNull(row.getCell(6))) {
			String phone = RowParseHelper.getCell(row, 6);
			// "^(((13[0-9])|(15([0-3]|[5-9]))|(18[0,5-9]))\\d{8})|(0\\d{2}-\\d{8})|(0\\d{3}-\\d{7})$");
			if (phone.matches("^(1\\d{10})|(0\\d{2}-\\d{8})|(0\\d{3}-\\d{7})$")) {
				routingDetail.setOperatePhone(phone);
			} else {
				throw new RuntimeException("填写的电话号码不符合规范 必须是有效的11位数字！");
			}
		}
		// 巡检记录备注
		if (!StringUtils.isNull(row.getCell(8))) {
			String cell = RowParseHelper.getCell(row, 8);
			if (cell.length() <= 100) {
				routingDetail.setRemark(cell);
			} else {
				throw new RuntimeException("情况备注的字符不能超过100个字符！");
			}
		}
		// 主表插入一条数据
		RoutingInspections routingInspections = new RoutingInspections();
		BeanUtils.copyProperties(routingDetail, routingInspections);
		routingInspections.setOperateType(2);// 设备维护
		routingInspections.setRoutingInspectionStatus(2);
		routingInspectionsSer.insertSelective(routingInspections);
		// 得到刚插入的这条数据的id--新增详情记录
		Integer routingInspectionId = routingInspections.getRoutingInspectionId();

		// 添加主机巡检记录
		if (!StringUtils.isNull(row.getCell(9))) {
			String gprsIdNew = row.getCell(9).toString().trim();
			if (gprsIdNew.matches("^([A-Z]{1}\\d{1}[A-Z]{1}\\d{6})$")) {
				routingDetail.setNewGprsIdOut(gprsIdNew);
			} else {
				throw new RuntimeException("新的主机id填写不符合规范！ 例：Y0A000002");
			}
			RoutingInspectionDetail deviceRID = new RoutingInspectionDetail();
			deviceRID.setRoutingInspectionsId(routingInspectionId);
			deviceRID.setDetailOperateName(routingDetail.getOperateName());
			deviceRID.setDetailOperateType(2);// 跟换主机
			deviceRID.setDetailOperateValueNew(gprsIdNew);
			deviceRID.setDetailOperateValueOld(routingDetail.getGprsId());
			deviceRID.setCreateTime(routingInspections.getOperateTime());
			deviceRID.setRequestSeq(1);
			deviceRID.setRequestType(0);
			routingInspectionDetailSer.insertSelective(deviceRID);
		}

		// 添加安装霍尔巡检记录
		if (!StringUtils.isNull(row.getCell(10))) {
			String hall = RowParseHelper.getCell(row, 10);
			if ("有".equals(hall)) {
				RoutingInspectionDetail deviceRID = new RoutingInspectionDetail();
				deviceRID.setRoutingInspectionsId(routingInspectionId);
				deviceRID.setDetailOperateName(routingDetail.getOperateName());
				deviceRID.setDetailOperateType(3);// 安装霍尔
				deviceRID.setDetailOperateValueOld(routingDetail.getGprsId());
				deviceRID.setCreateTime(routingInspections.getOperateTime());
				deviceRID.setRequestSeq(1);
				deviceRID.setRequestType(0);
				routingInspectionDetailSer.insertSelective(deviceRID);
			}
		}

		// 添加从机巡检记录
		// 修改从机
		for (int i = 0; i < 24; i++) {
			if (!StringUtils.isNull(row.getCell(11 + i))) {
				String subDeviceNew = row.getCell(11 + i).toString().trim();
				if (subDeviceNew.matches("^([A-Z]{1}\\d{8})$")) {
					RoutingInspectionDetail subDeviceRID = new RoutingInspectionDetail();
					subDeviceRID.setRoutingInspectionsId(routingInspectionId);
					subDeviceRID.setDetailOperateName(routingDetail.getOperateName());
					subDeviceRID.setCellIndex(i + 1);
					subDeviceRID.setDetailOperateType(4);// 跟换从机
					subDeviceRID.setDetailOperateValueOld(null);
					subDeviceRID.setDetailOperateValueNew(subDeviceNew);
					subDeviceRID.setCreateTime(routingInspections.getOperateTime());
					subDeviceRID.setRequestSeq(1);
					subDeviceRID.setRequestType(0);
					routingInspectionDetailSer.insertSelective(subDeviceRID);
				} else {
					throw new RuntimeException("从机编号：" + (i + 1) + "，id填写不符合规范！ 例：B00000002");
				}
			}
		}

		// 记录故障单体编号 根据逗号分隔
		List<Integer> cellIndexs = new ArrayList<Integer>();
		if (StringUtils.getString(row.getCell(35)) != null) {
			try {
				String value = RowParseHelper.getCell(row, 35);
				String[] arr = value.split(",|，");// 根据“ ”和“,”区分
				for (int i = 0; i < arr.length; i++) {
					if (Integer.parseInt(arr[i]) < 1 || Integer.parseInt(arr[i]) > 24) {
						int a = 9 / 0;
					}
					cellIndexs.add(Integer.parseInt(arr[i]));
				}

			} catch (Exception e) {
				throw new RuntimeException("故障单体编号填写格式不正确，例子：1,2,3...24 整数  !注意是英文逗号或者中文逗号分隔开");
			}
		}
		for (Integer index : cellIndexs) {
			// 巡检详情表添加
			RoutingInspectionDetail cellRID = new RoutingInspectionDetail();
			cellRID.setRoutingInspectionsId(routingInspectionId);
			cellRID.setDetailOperateName(routingDetail.getOperateName());
			cellRID.setCreateTime(routingInspections.getOperateTime());
			cellRID.setDetailOperateType(7);
			cellRID.setCellIndex(index);
			cellRID.setRequestSeq(1);
			cellRID.setRequestType(0);
			routingInspectionDetailSer.insertSelective(cellRID);
			// 巡检标记单体表添加
			InspectSignCellIndex inspectSignCell = new InspectSignCellIndex();
			inspectSignCell.setRoutingInspectionId(routingInspectionId);
			inspectSignCell.setCellIndex(index);
			inspectSignCellIndexSer.insert(inspectSignCell);
		}
	}

	/**
	 * 导入单体巡检记录
	 */
	@Override
	public void routingInsepectionExcelFileCell(Row row){
		if (StringUtils.isNull(row.getCell(0))) {
			throw new RuntimeException("基站名称必填项！");
		}
		if (StringUtils.isNull(row.getCell(1))) {
			throw new RuntimeException("设备编号必填项！");
		}
		// 查询电池组通过基站名称和设备编号
		StationInfo query = new StationInfo();
		query.setName(row.getCell(0).toString().trim());
		query.setGprsId(row.getCell(1).toString().trim());
		List<StationInfo> station = stationInfoSer.selectListSelective(query);
		if (station == null || station.size() == 0) {
			// 提示该列电池组不存在
			throw new RuntimeException("基站名称："+query.getName() + "或设备编号：" + query.getGprsId() + "不存在！");
		}
		RoutingInspectionStationDetail routingDetail = new RoutingInspectionStationDetail();
		routingDetail.setStationId(station.get(0).getId());
		routingDetail.setGprsId(station.get(0).getGprsId());
		//通过gprsid查询得到设备类型
		GprsConfigInfo queryGprsConfig = new GprsConfigInfo();
		queryGprsConfig.setGprsId(station.get(0).getGprsId());
		List<GprsConfigInfo> gprs = gprsConfigInfoSer.selectListSelective(queryGprsConfig);
		if(gprs.size() != 0) {
			routingDetail.setDeviceType(gprs.get(0).getDeviceType());
		}
		// 操作时间
		if (!StringUtils.isNull(row.getCell(2))) {
			// 判断是否为日期类型
			if (0 == row.getCell(2).getCellType()) {
				if (DateUtil.isCellDateFormatted(row.getCell(2))) {
					// 用于转化为日期格式
					Date date = row.getCell(2).getDateCellValue();
					routingDetail.setOperateTime(date);
				}
			} else {
				throw new RuntimeException("操作日期填写格式不正确，正确的格式是：年/月/日  或者：年/月/日  时:分:秒 ");
			}
		} else {
			Date time = new Date();
			routingDetail.setOperateTime(time);
		}
		// 操作人员
		if (!StringUtils.isNull(row.getCell(3))) {
			String cell = RowParseHelper.getCell(row, 3);
			if (cell.length() <= 30) {
				routingDetail.setOperateName(cell);
			} else {
				throw new RuntimeException("操作人员的名字不能超过30个字符");
			}
		}
		// 电话号码
		if (!StringUtils.isNull(row.getCell(4))) {
			String phone = RowParseHelper.getCell(row, 4);
			if (phone.matches("^(1\\d{10})|(0\\d{2}-\\d{8})|(0\\d{3}-\\d{7})$")) {
				routingDetail.setOperatePhone(phone);
			} else {
				throw new RuntimeException("填写的电话号码不符合规范 必须是有效的11位数字！");
			}
		}
		// 操作备注
		if (!StringUtils.isNull(row.getCell(6))) {
			String cell = RowParseHelper.getCell(row, 6);
			if (cell.length() <= 100) {
				routingDetail.setRemark(cell);
			} else {
				throw new RuntimeException("情况备注填写不能超过100个字符！");
			}
		}
		// 主表插入一条数据
		RoutingInspections routingInspections = new RoutingInspections();
		BeanUtils.copyProperties(routingDetail, routingInspections);
		routingInspections.setOperateType(3);// 跟换单体
		routingInspections.setRoutingInspectionStatus(2);
		routingInspectionsSer.insertSelective(routingInspections);

		// 得到刚插入的这条数据的id--新增详情记录
		Integer routingInspectionId = routingInspections.getRoutingInspectionId();
		
		RoutingInspectionDetail cellDetailList = new RoutingInspectionDetail();
		for (int i = 0; i < 24; i++) {
			if(!StringUtils.isNull(row.getCell(7 + 3 * i)) 
					|| !StringUtils.isNull(row.getCell(8 + 3 * i)) 
					|| !StringUtils.isNull(row.getCell(9 + 3 * i))) {
				// 有一个不为空，就要校验 新电池类型
				String newType = null;
				if (StringUtils.isNull(row.getCell(8 + 3 * i))) {
					throw new RuntimeException("编号"+ (i + 1)+"确认有跟换的情况下，新换电池类型为必填项，且为‘新电池’或‘二次利旧’");
				}else {
					newType = RowParseHelper.getCell(row, 8 + 3 * i);
					if (!"二次利旧".equals(newType) && !"新电池".equals(newType)) {
						throw new RuntimeException("编号"+ (i + 1)+"确认有跟换的情况下，新换电池的类型填写错误！正确类型为‘新电池’或‘二次利旧’");
					}
				}
				String oldType = RowParseHelper.getCell(row, 7 + 3 * i);
				if (oldType != null && (!"淘汰".equals(oldType) && !"二次利旧".equals(oldType))) {
					throw new RuntimeException("编号"+ (i + 1)+"确认有跟换的情况下，被替换电池的类型填写错误！正确类型为‘淘汰’或‘二次利旧’");
				}
				cellDetailList.setCellIndex(i + 1);
				cellDetailList.setRoutingInspectionsId(routingInspectionId);
				cellDetailList.setDetailOperateName(routingDetail.getOperateName());
				cellDetailList.setDetailOperateValueOld(oldType);
				cellDetailList.setDetailOperateValueNew(newType);
				cellDetailList.setCreateTime(routingInspections.getOperateTime());
				cellDetailList.setDetailOperateType(5);// 更换单体_电池类型
				cellDetailList.setRequestSeq(1);
				cellDetailList.setRequestType(0);
				// 保存详情
				routingInspectionDetailSer.insertSelective(cellDetailList);
				// 更新单体信息
				String cellPlant = RowParseHelper.getCell(row, 9 + 3 * i);
				cellInfoSer.appUpdateCellInfo(routingDetail, cellDetailList, cellPlant);
			}
		}
	}
}