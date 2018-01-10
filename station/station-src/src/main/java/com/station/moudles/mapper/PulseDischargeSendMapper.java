package com.station.moudles.mapper;

import com.station.moudles.entity.PulseDischargeSend;

import java.util.Date;
import java.util.List;
import java.util.Map;

public interface PulseDischargeSendMapper extends BaseMapper<PulseDischargeSend, Integer> {

    List<PulseDischargeSend> getPulseDischargeSendsByTimes(Map map);

    List<PulseDischargeSend> getLatestUnProcessedCommands(String gprsId);

    /**
     * 获取send_done=1但是在指定时间内没响应的数据
     *
     * @return
     */
    List<PulseDischargeSend> getRecordsWhichSentButUnPorcessed(Date specifiedTime);

    /**
     * 将指定记录并且send_done=1的数据状态send_done改为6
     *
     * @param id
     * @return
     */
    int updateAsTimeout(Integer id);
}