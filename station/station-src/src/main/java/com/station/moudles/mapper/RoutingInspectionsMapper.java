package com.station.moudles.mapper;

import java.util.List;
import java.util.Map;

import com.station.moudles.entity.RoutingInspectionStationDetail;
import com.station.moudles.entity.RoutingInspections;
import com.station.moudles.entity.RoutingInspectionsStation;
import com.station.moudles.vo.search.SearchStationInfoPagingVo;

public interface RoutingInspectionsMapper extends BaseMapper<RoutingInspections, Integer> {
	List<RoutingInspectionsStation> selectStationListSelectivePaging(SearchStationInfoPagingVo searchStationInfoPagingVo);

	RoutingInspectionStationDetail selectStationDetailByPrimaryKey(Integer pk);

	//查看整治信息
	List<RoutingInspections> selectCellInspace(RoutingInspections routingInspections);
	
	//查找指定电池组有标记故障单体的记录
	List<RoutingInspections> selectListHasInspectSignCell(Map param);
	
	RoutingInspections selectOneLatestSelective(RoutingInspections routingInspections);
	
	//查询在判断主表中是否有新增的数据
	List<RoutingInspections> selectListSelectiveFirst(RoutingInspections routingInspections);
}