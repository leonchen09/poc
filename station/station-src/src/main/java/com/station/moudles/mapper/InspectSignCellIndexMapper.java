package com.station.moudles.mapper;

import com.station.moudles.entity.InspectSignCellIndex;
import com.station.moudles.entity.PulseCalculationSend;

import java.util.List;
import java.util.Map;

public interface InspectSignCellIndexMapper extends BaseMapper<InspectSignCellIndex, Integer> {

    /**
     * 获取指定基站最新巡检标记单体
     *
     * @param stationId
     * @return
     */
    List<InspectSignCellIndex> getLatestByStationId(Integer stationId);

    List<InspectSignCellIndex> getLatestByStationIds(Iterable<Integer> stationIds);
}