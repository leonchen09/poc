package com.station.moudles.service.impl;

import java.util.ArrayList;
import java.util.Comparator;
import java.util.Date;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.TreeSet;

import org.apache.poi.ss.usermodel.DateUtil;
import org.apache.poi.ss.usermodel.Row;
import org.springframework.beans.BeanUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.google.common.collect.Maps;
import com.station.common.Constant;
import com.station.common.utils.RowParseHelper;
import com.station.common.utils.StringUtils;
import com.station.moudles.entity.GprsConfigInfo;
import com.station.moudles.entity.GprsDeviceType;
import com.station.moudles.entity.InspectSignCellIndex;
import com.station.moudles.entity.ModifyGprsidSend;
import com.station.moudles.entity.RoutingInspectionDetail;
import com.station.moudles.entity.RoutingInspectionStationDetail;
import com.station.moudles.entity.RoutingInspections;
import com.station.moudles.entity.StationInfo;
import com.station.moudles.entity.SubDevice;
import com.station.moudles.mapper.GprsConfigInfoMapper;
import com.station.moudles.mapper.RoutingInspectionDetailMapper;
import com.station.moudles.service.CellInfoService;
import com.station.moudles.service.GprsConfigInfoService;
import com.station.moudles.service.GprsDeviceTypeService;
import com.station.moudles.service.InspectSignCellIndexService;
import com.station.moudles.service.ModifyGprsidSendService;
import com.station.moudles.service.RoutingInspectionDetailService;
import com.station.moudles.service.RoutingInspectionsService;
import com.station.moudles.service.StationInfoService;
import com.station.moudles.service.SubDeviceService;
import com.station.moudles.service.UserService;
import com.station.moudles.vo.AjaxResponse;

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
	@Autowired
	GprsDeviceTypeService gprsDeviceTypeSer;

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
		if(StringUtils.isNull(row.getCell(2))) {
			throw new RuntimeException("设备类型为必填项！");
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
		// 通过gprsid查询得到设备类型
		GprsConfigInfo queryGprsConfig = new GprsConfigInfo();
		queryGprsConfig.setGprsId(station.get(0).getGprsId());
		List<GprsConfigInfo> gprs = gprsConfigInfoSer.selectListSelective(queryGprsConfig);
		if (gprs.size() != 0) {
			routingDetail.setDeviceType(gprs.get(0).getDeviceType());
			GprsDeviceType devieceType = gprsDeviceTypeSer.selectDevieceType(row.getCell(2).toString());
			if(devieceType ==null || devieceType.getTypeCode() != gprs.get(0).getDeviceType()) {
				throw new RuntimeException("设备类型和电池组的设备类型不一致");
			}
		}
		// 操作时间
		if (!StringUtils.isNull(row.getCell(5))) {
			// 判断是否为日期类型
			if (0 == row.getCell(5).getCellType()) {
				if (DateUtil.isCellDateFormatted(row.getCell(5))) {
					// 用于转化为日期格式
					Date date = row.getCell(5).getDateCellValue();
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
		if (!StringUtils.isNull(row.getCell(6))) {
			String cell = RowParseHelper.getCell(row, 6);
			if (cell.length() <= 30) {
				routingDetail.setOperateName(cell);
			} else {
				throw new RuntimeException("操作人员的名字不能超过30个字符");
			}
		}
		// 电话号码
		if (!StringUtils.isNull(row.getCell(7))) {
			String phone = RowParseHelper.getCell(row, 7);
			// "^(((13[0-9])|(15([0-3]|[5-9]))|(18[0,5-9]))\\d{8})|(0\\d{2}-\\d{8})|(0\\d{3}-\\d{7})$");
			if (phone.matches("^(1\\d{10})|(0\\d{2}-\\d{8})|(0\\d{3}-\\d{7})$")) {
				routingDetail.setOperatePhone(phone);
			} else {
				throw new RuntimeException("填写的电话号码不符合规范 必须是有效的11位数字！");
			}
		}
		// 巡检记录备注
		if (!StringUtils.isNull(row.getCell(9))) {
			String cell = RowParseHelper.getCell(row, 9);
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
		if (!StringUtils.isNull(row.getCell(10))) {
			String gprsIdNew = row.getCell(10).toString().trim();
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
		if (!StringUtils.isNull(row.getCell(11))) {
			String hall = RowParseHelper.getCell(row, 11);
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
		/**
		 * 判断是否是 蓄电池12V从机检测设备
		 */
		List<Integer> cellIndexs = new ArrayList<Integer>();
		if(!StringUtils.isNull(row.getCell(2))) {
			String deviceType = RowParseHelper.getCell(row, 2);
			if ("蓄电池12V监测设备".equals(deviceType)) {
				updateSubDeviceInfpect(row, routingDetail, routingInspections, routingInspectionId, 4);
				cellIndexs = recordCell(row,16,4);
			} else {
				updateSubDeviceInfpect(row, routingDetail, routingInspections, routingInspectionId, 24);
				cellIndexs = recordCell(row,36,24);
			}
		} else {
			throw new RuntimeException("设备类型没有填写！");
		}
		// 蓄电池12V从机检测设备添加从机巡检记录
		// 修改从机
//		for (int i = 0; i < 24; i++) {
//			if (!StringUtils.isNull(row.getCell(12 + i))) {
//				String subDeviceNew = row.getCell(12 + i).toString().trim();
//				if (subDeviceNew.matches("^([A-Z]{1}\\d{8})$")) {
//					RoutingInspectionDetail subDeviceRID = new RoutingInspectionDetail();
//					subDeviceRID.setRoutingInspectionsId(routingInspectionId);
//					subDeviceRID.setDetailOperateName(routingDetail.getOperateName());
//					subDeviceRID.setCellIndex(i + 1);
//					subDeviceRID.setDetailOperateType(4);// 跟换从机
//					subDeviceRID.setDetailOperateValueOld(null);
//					subDeviceRID.setDetailOperateValueNew(subDeviceNew);
//					subDeviceRID.setCreateTime(routingInspections.getOperateTime());
//					subDeviceRID.setRequestSeq(1);
//					subDeviceRID.setRequestType(0);
//					routingInspectionDetailSer.insertSelective(subDeviceRID);
//				} else {
//					throw new RuntimeException("从机编号：" + (i + 1) + "，id填写不符合规范！ 例：B00000002");
//				}
//			}
//		}

		// 记录故障单体编号 根据逗号分隔
		//List<Integer> cellIndexs = new ArrayList<Integer>();
//		if (StringUtils.getString(row.getCell(36)) != null) {
//			try {
//				String value = RowParseHelper.getCell(row, 36);
//				String[] arr = value.split(",|，");// 根据“ ”和“,”区分
//				for (int i = 0; i < arr.length; i++) {
//					if (Integer.parseInt(arr[i]) < 1 || Integer.parseInt(arr[i]) > 24) {
//						int a = 9 / 0;
//					}
//					cellIndexs.add(Integer.parseInt(arr[i]));
//				}
//
//			} catch (Exception e) {
//				throw new RuntimeException("故障单体编号填写格式不正确，例子：1,2,3...24 整数  !注意是英文逗号或者中文逗号分隔开");
//			}
//		}
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
	 * 记录修改从机信息
	 * @param row
	 * @param routingDetail
	 * @param routingInspections
	 * @param routingInspectionId
	 * @param count
	 */
	public void updateSubDeviceInfpect(Row row, RoutingInspectionStationDetail routingDetail,
			RoutingInspections routingInspections, Integer routingInspectionId, Integer count) {
		// 修改从机
		for (int i = 0; i < count; i++) {
			if (!StringUtils.isNull(row.getCell(12 + i))) {
				String subDeviceNew = row.getCell(12 + i).toString().trim();
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

	}
	/**
	 * 记录标记的单体
	 * @param row 
	 * @param cellNumer 标记单体在excel 表的列
	 * @param subCount  故障从机的数量
	 * @return
	 */
	public List<Integer> recordCell(Row row, Integer cellNumer,Integer subCount) {
		List<Integer> cellIndexs = new ArrayList<Integer>();
		if (StringUtils.getString(row.getCell(cellNumer)) != null) {
			try {
				String value = RowParseHelper.getCell(row, cellNumer);
				String[] arr = value.split(",|，");// 根据“ ”和“,”区分
				for (int i = 0; i < arr.length; i++) {
					if (Integer.parseInt(arr[i]) < 1 || Integer.parseInt(arr[i]) > subCount) {
						int a = 9 / 0;
					}
					cellIndexs.add(Integer.parseInt(arr[i]));
				}

			} catch (Exception e) {
				throw new RuntimeException("故障单体编号填写格式不正确，例子：1,2,3...24 整数  !注意是英文逗号或者中文逗号分隔开");
			}
		}
		return cellIndexs;

	}
	
	
	/**
	 * 导入单体巡检记录
	 */
	@Override
	public void routingInsepectionExcelFileCell(Row row) {
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
			throw new RuntimeException("基站名称：" + query.getName() + "或设备编号：" + query.getGprsId() + "不存在！");
		}
		RoutingInspectionStationDetail routingDetail = new RoutingInspectionStationDetail();
		routingDetail.setStationId(station.get(0).getId());
		routingDetail.setGprsId(station.get(0).getGprsId());
		// 通过gprsid查询得到设备类型
		GprsConfigInfo queryGprsConfig = new GprsConfigInfo();
		queryGprsConfig.setGprsId(station.get(0).getGprsId());
		List<GprsConfigInfo> gprs = gprsConfigInfoSer.selectListSelective(queryGprsConfig);
		if (gprs.size() != 0) {
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
			if (!StringUtils.isNull(row.getCell(7 + 3 * i)) || !StringUtils.isNull(row.getCell(8 + 3 * i))
					|| !StringUtils.isNull(row.getCell(9 + 3 * i))) {
				// 有一个不为空，就要校验 新电池类型
				String newType = null;
				if (StringUtils.isNull(row.getCell(8 + 3 * i))) {
					throw new RuntimeException("编号" + (i + 1) + "确认有跟换的情况下，新换电池类型为必填项，且为‘新电池’或‘二次利旧’");
				} else {
					newType = RowParseHelper.getCell(row, 8 + 3 * i);
					if (!"二次利旧".equals(newType) && !"新电池".equals(newType)) {
						throw new RuntimeException("编号" + (i + 1) + "确认有跟换的情况下，新换电池的类型填写错误！正确类型为‘新电池’或‘二次利旧’");
					}
				}
				String oldType = RowParseHelper.getCell(row, 7 + 3 * i);
				if (oldType != null && (!"淘汰".equals(oldType) && !"二次利旧".equals(oldType))) {
					throw new RuntimeException("编号" + (i + 1) + "确认有跟换的情况下，被替换电池的类型填写错误！正确类型为‘淘汰’或‘二次利旧’");
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
				cellInfoSer.exportUpdateCellInfo(routingDetail, cellDetailList, cellPlant);
			}
		}
	}

	@Override
	public AjaxResponse<RoutingInspectionStationDetail> appSaveInfo(RoutingInspectionStationDetail routingInspectionStationDetail) {
		AjaxResponse<RoutingInspectionStationDetail> ajaxResponse =  new AjaxResponse<RoutingInspectionStationDetail>(Constant.RS_CODE_SUCCESS, "提交成功！");
		// 改变电池组的状态 安装、维护流程，电池组状态。
		// 99:未安装，30:已安装，31，在线；32，离线；10:安装中，11:安装中等待确认状态，12:安装中后台确认未完成状态，20:维护中，21:维护中等待确认状态，22:维护中后台确认未完成状态
		StationInfo station = new StationInfo();
		StationInfo stationinfo = stationInfoSer.selectByPrimaryKey(routingInspectionStationDetail.getStationId());
		if (stationinfo != null) {
			station.setInspectStatus(routingInspectionStationDetail.getInspectStatus());
			station.setId(routingInspectionStationDetail.getStationId());
			stationInfoSer.updateByPrimaryKeySelective(station);
		}
		RoutingInspections routingInspections = new RoutingInspections();
		routingInspections.setStationId(routingInspectionStationDetail.getStationId());
		routingInspections.setGprsId(routingInspectionStationDetail.getGprsId());
		//routingInspections.setRoutingInspectionStatus(1);// 设备维护中
		// 通过电池组的状态在维护中 ，电池组id,设备id来判断是否有这条数据 如果没有就新增
		List<RoutingInspections> routingList = routingInspectionsSer.selectListSelectiveFirst(routingInspections);

		if (routingList.size() == 0) {
			Date date = new Date();
			routingInspections.setOperateTime(date);
			routingInspections.setRoutingInspectionStatus(1);
			routingInspections.setOperateType(routingInspectionStationDetail.getOperateType());
			routingInspections.setOperateId(routingInspectionStationDetail.getOperateId());
			routingInspections.setOperateName(routingInspectionStationDetail.getOperateName());
			routingInspections.setOperatePhone(routingInspectionStationDetail.getOperatePhone());
			routingInspections.setDeviceType(routingInspectionStationDetail.getDeviceType());
			routingInspectionsSer.insertSelective(routingInspections);
		}else {
			routingList.get(0).setRoutingInspectionStatus(1);			
			routingInspectionsSer.updateByPrimaryKeySelective(routingList.get(0));
		}
		// 查询刚插入的这条数据得到id后--新增详情记录
		Integer routingId = null;
		if (routingList.size() == 0) {
			routingId = routingInspections.getRoutingInspectionId();
		} else {
			routingId = routingList.get(0).getRoutingInspectionId();
		}
		if (routingId != null) {
			RoutingInspectionDetail routingInspectionDetail = new RoutingInspectionDetail();
			routingInspectionDetail.setRoutingInspectionsId(routingId);
			routingInspectionDetail.setRequestType(1);//通过requestType == 1 查询web是否回应
			//提交信息的集合
			List<RoutingInspectionDetail> routingInspectionDetailList = routingInspectionStationDetail.getRoutingInspectionDetailList();
			// 查询出web提交的
			List<RoutingInspectionDetail> routingDetailType = routingInspectionDetailSer.selectListSelective(routingInspectionDetail);
			routingInspectionDetail.setRequestType(0);//通过requestType == 0 查询出最大RequestSeq的数据
			List<RoutingInspectionDetail> routingDetailSeq = routingInspectionDetailSer.selectListSelective(routingInspectionDetail);

			if (routingDetailType.size() != 0) {
					RoutingInspectionDetail  InspectionDetail = null;
				if(routingDetailSeq.size() != 0) {
					 InspectionDetail = routingDetailSeq.stream().max(Comparator.comparing(RoutingInspectionDetail::getRequestSeq)).get();
				}
				// 循环新增详情
				if (routingInspectionDetailList.size() != 0) {
					for (RoutingInspectionDetail detailList : routingInspectionDetailList) {
						if (InspectionDetail == null) {
							//如果web返回时app的详情没有数据,就是web端的最大数据加1
							RoutingInspectionDetail	 InspectionDetailType = routingDetailType.stream().max(Comparator.comparing(RoutingInspectionDetail::getRequestSeq)).get();
							//detailList.setRequestSeq(1);
							detailList.setRequestSeq(InspectionDetailType.getRequestSeq()+1);
						} else {
							detailList.setRequestSeq(InspectionDetail.getRequestSeq() + 1);
						}
						// 判断operateType == 4 更换从机
						if (detailList.getDetailOperateType() == 4) {
							AjaxResponse<RoutingInspectionStationDetail> updateSubDevice = updateSubDevice(
									routingInspectionStationDetail, detailList);
							if (updateSubDevice.getCode().equals("0001")) {
								throw new IllegalArgumentException(updateSubDevice.getMsg());
							}
						}
						detailList.setRoutingInspectionsId(routingId);
						detailList.setDetailOperateId(routingInspectionStationDetail.getOperateId());
						detailList.setDetailOperateName(routingInspectionStationDetail.getOperateName());
						routingInspectionDetailSer.insertSelective(detailList);
					}
				}
			} else {
				if (routingInspectionDetailList.size() != 0) {
					for (RoutingInspectionDetail detailList : routingInspectionDetailList) {
						// 判断operateType == 4 更换从机
						if (detailList.getDetailOperateType() == 4) {
							AjaxResponse<RoutingInspectionStationDetail> updateSubDevice = updateSubDevice(
									routingInspectionStationDetail, detailList);
							if (updateSubDevice.getCode().equals("0001")) {
								throw new IllegalArgumentException(updateSubDevice.getMsg());
							}
						}
						detailList.setRequestSeq(1);
						detailList.setRoutingInspectionsId(routingId);
						detailList.setDetailOperateId(routingInspectionStationDetail.getOperateId());
						detailList.setDetailOperateName(routingInspectionStationDetail.getOperateName());
	
						routingInspectionDetailSer.insertSelective(detailList);
					}
				}
			}
		} else {
			throw new IllegalArgumentException("提交失败！");
		}
		return ajaxResponse;
	}
	
	
	/*
	 * 当DetailOperateType() == 4 的时候调用修改从机
	 */
	public AjaxResponse<RoutingInspectionStationDetail> updateSubDevice(RoutingInspectionStationDetail routingInspectionStationDetail,RoutingInspectionDetail detailList ){
		AjaxResponse<RoutingInspectionStationDetail> ajaxResponse = new AjaxResponse<RoutingInspectionStationDetail>(Constant.RS_CODE_SUCCESS,"修改成功");
		//根据gprsid 和cellIndex 查找出old sub_device_id_out
		SubDevice subDevice = new SubDevice();
		subDevice.setGprsId(routingInspectionStationDetail.getGprsId());
		subDevice.setCellSort(detailList.getCellIndex());
		subDevice.setSubFlag(0);//配套
		List<SubDevice> subDeviceList = subDeviceSer.selectListSelective(subDevice);
		if(subDeviceList.size() != 0) {
		detailList.setDetailOperateValueOld(subDeviceList.get(0).getSubDeviceIdOut());
		//判断是否是备用从机
		SubDevice queryStandby = new SubDevice();
		queryStandby.setSubDeviceId(detailList.getDetailOperateValueNew());
		queryStandby.setSubFlag(1);							
		List<SubDevice> isStandby = subDeviceSer.selectListSelective(queryStandby);
		if(isStandby.size() != 0) {
			//发送指令
			ModifyGprsidSend send = new ModifyGprsidSend();
			send.setGprsId(routingInspectionStationDetail.getGprsId());
			send.setInnerId(subDeviceList.get(0).getSubDeviceIdOut());
			send.setOuterId(detailList.getDetailOperateValueNew());
			send.setType(2);// 跟换从机
			try {
				modifyGprsidSendSer.changeDeviceId(send, 0);
			} catch (Exception e) {
				ajaxResponse.setCode(Constant.RS_CODE_ERROR);
				ajaxResponse.setMsg(e.getMessage());
				return ajaxResponse;			
			}
			
		}else {
			throw new IllegalArgumentException("跟换的从机不是备用从机！");
		}
	
		}else {
			throw new IllegalArgumentException("没有对应的从机！");
		}
	return ajaxResponse;
		
	}

	@Override
	public void notAccomplishSubmit(RoutingInspectionDetail routingInspectionDetail) {
		
		RoutingInspectionDetail query = new RoutingInspectionDetail();
		query.setRoutingInspectionsId(routingInspectionDetail.getRoutingInspectionsId());
		query.setRequestType(1);// 后台返回给APP时 状态要改变 0 是没有回应 1 是回应
		List<RoutingInspectionDetail> selectListSelective = routingInspectionDetailSer.selectListSelective(query);
		if (selectListSelective.size() == 0) {
			// 后台返回给APP时 状态要改变 0 是没有回应 1 是回应
			routingInspectionDetail.setRequestType(1);
			routingInspectionDetail.setRequestSeq(1);
			routingInspectionDetail.setRoutingInspectionsId(routingInspectionDetail.getRoutingInspectionsId());//确认未完成状态
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
		RoutingInspections routing = new RoutingInspections();
		routing.setRoutingInspectionStatus(3);//确定未完成
		routing.setRoutingInspectionId(routingInspectionDetail.getRoutingInspectionsId());
		routingInspectionsSer.updateByPrimaryKeySelective(routing);
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
		}else {
			throw new IllegalArgumentException("没有电池组信息！");
		}
		
	}

	@Override
	public void confirmRoutingInspection(RoutingInspectionDetail routingInspectionDetail) {
		if (routingInspectionDetail.getRoutingInspectionsId() != null) {
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
			throw new IllegalArgumentException("确完成失败！");
		}
		
	}
}