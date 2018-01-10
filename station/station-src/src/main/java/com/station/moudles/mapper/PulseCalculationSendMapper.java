package com.station.moudles.mapper;

import com.station.moudles.entity.PulseCalculationSend;

import java.util.List;
import java.util.Map;

public interface PulseCalculationSendMapper extends BaseMapper<PulseCalculationSend, Integer> {

    /**
     * 获取最近一条测试数据
     *
     * @param gprsId
     * @return
     */
    PulseCalculationSend getLatestRecord(String gprsId);

    PulseCalculationSend getResistanceRecord(Map m);

    PulseCalculationSend getCapacityRecord(Map m);

    List<PulseCalculationSend> getLatestRecords(Iterable<String> gprsIds);

    List<PulseCalculationSend> getLatestResistanceSuccessRecords(Map map);

    List<PulseCalculationSend> getLatestCapacitySuccessRecords(Map map);
}