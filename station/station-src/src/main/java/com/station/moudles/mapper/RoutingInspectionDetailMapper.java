package com.station.moudles.mapper;

import java.util.List;
import java.util.Map;

import org.apache.ibatis.annotations.Param;

import com.station.moudles.entity.RoutingInspectionDetail;

public interface RoutingInspectionDetailMapper extends BaseMapper<RoutingInspectionDetail, Integer> {
	List<RoutingInspectionDetail> selectDetailByInspectionIds(Map m);
	//----10/18 add 查看app提交的记录
	List<RoutingInspectionDetail> selectListSelectiveApp(RoutingInspectionDetail routingInspectionDetail);

}