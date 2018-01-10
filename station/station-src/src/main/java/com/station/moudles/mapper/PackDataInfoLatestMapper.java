package com.station.moudles.mapper;

import java.util.List;
import java.util.Map;

import com.station.moudles.entity.PackDataInfoLatest;
import com.station.moudles.entity.RoutingInspectionPackData;

public interface PackDataInfoLatestMapper extends BaseMapper<PackDataInfoLatest, String> {
	// 关联 pack_data_info_latest 表查找指定电池状态和操作类型的记录
	List<RoutingInspectionPackData> selectListHasChangeCell(Map param);
}