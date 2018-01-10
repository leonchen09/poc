package com.station.moudles.mapper;

import java.util.List;
import java.util.Map;

import com.station.moudles.entity.StationWarningInfo;
import com.station.moudles.entity.WarnArea;
import com.station.moudles.entity.WarningInfo;

public interface WarningInfoMapper extends BaseMapper<WarningInfo, Integer> {

	List<StationWarningInfo> selectWarningListByStation(Map m);

	List<WarnArea> selectWarnAreaList(Map m);
}