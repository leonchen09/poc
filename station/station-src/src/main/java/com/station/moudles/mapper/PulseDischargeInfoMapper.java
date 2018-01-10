package com.station.moudles.mapper;

import com.station.moudles.entity.PulseDischargeInfo;

import java.util.List;

public interface PulseDischargeInfoMapper extends BaseMapper<PulseDischargeInfo, Integer> {

    /**
     * 获取指定特征指令的特征数据
     *
     * @param sendIds
     * @return
     */
    List<PulseDischargeInfo> findByPulseDischargeSendIds(Integer... sendIds);
    

}