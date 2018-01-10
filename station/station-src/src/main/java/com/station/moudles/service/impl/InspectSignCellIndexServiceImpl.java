package com.station.moudles.service.impl;

import java.util.Comparator;
import java.util.Date;
import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.station.moudles.entity.InspectSignCellIndex;
import com.station.moudles.entity.RoutingInspectionDetail;
import com.station.moudles.entity.RoutingInspectionStationDetail;
import com.station.moudles.entity.RoutingInspections;
import com.station.moudles.service.InspectSignCellIndexService;
import com.station.moudles.service.RoutingInspectionDetailService;
import com.station.moudles.service.RoutingInspectionsService;

@Service
public class InspectSignCellIndexServiceImpl extends BaseServiceImpl<InspectSignCellIndex, Integer>
		implements InspectSignCellIndexService {
	@Autowired
	RoutingInspectionsService routingInspectionsSer;
	@Autowired
	RoutingInspectionDetailService routingInspectionDetailSer;
	@Autowired
	InspectSignCellIndexService InspectSignCellIndexSer;

	// 保存标记的故障单体编号
	@Override
	public void addinspectSignCellIndex(RoutingInspectionStationDetail stationDetail) {
		// 更换单体保存
		RoutingInspections routingInspections = new RoutingInspections();
		routingInspections.setStationId(stationDetail.getStationId());
		routingInspections.setGprsId(stationDetail.getGprsId());
		routingInspections.setOperateType(2);// 安装维护状态
		// 通过电池组的状态在维护中 ，电池组id,设备id来判断是否有这条数据 如果没有就新增
		List<RoutingInspections> routingList = routingInspectionsSer.selectListSelective(routingInspections);
		boolean flag = true;
		for (RoutingInspections ListStatus : routingList) {
			// 如果电池组是安装维护中或者是失败状态就不在主表中插入数据
			if (ListStatus.getRoutingInspectionStatus() != null) {
				if (ListStatus.getRoutingInspectionStatus() == 1 || ListStatus.getRoutingInspectionStatus() == 3) {
					routingInspections.setRoutingInspectionStatus(ListStatus.getRoutingInspectionStatus());
					flag = false;
				}
			}
		}
		if (flag) {
			if (stationDetail.getOperateTime() != null) {
				routingInspections.setOperateTime(stationDetail.getOperateTime());
			} else {
				Date date = new Date();
				routingInspections.setOperateTime(date);
			}
			routingInspections.setRoutingInspectionStatus(1);
			routingInspections.setOperateId(stationDetail.getOperateId());
			routingInspections.setOperateName(stationDetail.getOperateName());
			routingInspections.setOperatePhone(stationDetail.getOperatePhone());
			routingInspectionsSer.insertSelective(routingInspections);
			logger.info("新增巡检记录stationid({})--->({})完成",
					new Object[] { stationDetail.getStationId(), stationDetail.getGprsId() });
		}
		// 查询刚插入的这条数据得到id后--新增详情记录
		// 将这个routingInspections 重新写一个类
		RoutingInspections query = new RoutingInspections();
		query.setStationId(stationDetail.getStationId());
		query.setGprsId(stationDetail.getGprsId());
		query.setOperateType(2);
		query.setRoutingInspectionStatus(1);

		List<RoutingInspections> selectListSelective = routingInspectionsSer.selectListSelective(query);
		if (selectListSelective.size() != 0) {
			RoutingInspections RI = selectListSelective.get(selectListSelective.size() - 1);
			Integer routingInspectionId = RI.getRoutingInspectionId();

			// 将标记的故障单体保存在inspect_sign_cell_index表中
			List<Integer> cellIndex = stationDetail.getCellIndex();
			InspectSignCellIndex InspectSignCell = new InspectSignCellIndex();
			InspectSignCell.setRoutingInspectionId(routingInspectionId);
			for (Integer integer : cellIndex) {
				InspectSignCell.setCellIndex(integer);
				InspectSignCellIndexSer.insert(InspectSignCell);
			}

			RoutingInspectionDetail routingInspectionDetail = new RoutingInspectionDetail();
			routingInspectionDetail.setRoutingInspectionsId(routingInspectionId);
			routingInspectionDetail.setRequestType(0);
			// 查询出提交的最大次数
			List<RoutingInspectionDetail> routingDetail = routingInspectionDetailSer
					.selectListSelective(routingInspectionDetail);
			// 详情保存
			if (routingDetail.size() != 0) {
				// 得出最新数据
				// RoutingInspectionDetail InspectionDetail =
				// routingDetail.get(routingDetail.size() - 1);
				RoutingInspectionDetail InspectionDetail = routingDetail.stream()
						.max(Comparator.comparing(RoutingInspectionDetail::getRequestSeq)).get();
				for (Integer cellNum : cellIndex) {

					if (InspectionDetail.getRequestSeq() == null) {
						routingInspectionDetail.setRequestSeq(1);
					} else {
						routingInspectionDetail.setRequestSeq(InspectionDetail.getRequestSeq() + 1);
					}
					if (stationDetail.getOperateTime() != null) {
						routingInspectionDetail.setCreateTime(stationDetail.getOperateTime());
					}
					routingInspectionDetail.setDetailOperateId(RI.getOperateId());
					routingInspectionDetail.setDetailOperateName(RI.getOperateName());
					routingInspectionDetail.setDetailOperateType(7);// 标记故障单体
					routingInspectionDetail.setCellIndex(cellNum);
					routingInspectionDetail.setDetailOperateValueOld(null);
					routingInspectionDetail.setDetailOperateValueNew(null);
					routingInspectionDetailSer.insertSelective(routingInspectionDetail);
				}
			} else {
				for (Integer cellNum : cellIndex) {
					if (stationDetail.getOperateTime() != null) {
						routingInspectionDetail.setCreateTime(stationDetail.getOperateTime());
					}
					routingInspectionDetail.setRequestSeq(1);
					routingInspectionDetail.setDetailOperateId(RI.getOperateId());
					routingInspectionDetail.setDetailOperateName(RI.getOperateName());
					routingInspectionDetail.setDetailOperateType(7);// 标记故障单体
					routingInspectionDetail.setCellIndex(cellNum);
					routingInspectionDetail.setDetailOperateValueOld(null);
					routingInspectionDetail.setDetailOperateValueNew(null);
					routingInspectionDetailSer.insertSelective(routingInspectionDetail);
				}
			}
		} else {
			throw new IllegalArgumentException("巡检记录详情添加失败！");
		}

	}

}
