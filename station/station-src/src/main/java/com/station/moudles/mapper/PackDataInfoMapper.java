package com.station.moudles.mapper;

import com.station.moudles.entity.PackDataInfo;

import java.util.List;
import java.util.Map;

public interface PackDataInfoMapper extends BaseMapper<PackDataInfo, Integer> {
    List<PackDataInfo> selectListByGprsIdTime(Map m);

    List<PackDataInfo> selectByOrderSelective(Map m);

    List<PackDataInfo> selectListByTime(Map m);

    /**
     * 获取最近5次有效放电记录
     * 如该电池组历史数据在valid_day期限内，有数据同时满足gen_vol小于等于valid_discharge_vol及gen_cur大于等于valid_discharge_cur两个条件，则电池组有最近一次放电记录。
     *
     * @param m
     * @return
     */
    List<PackDataInfo> getLatestValidDischargeRecord(Map m);

    /**
     * 分页获取小于指定ID的测试数据
     *
     * @return
     */
    List<PackDataInfo> getPackDataInfosWhichIdLessThanGivenValue(Map m);

    /**
     * 分页获取大于指定ID的测试数据
     *
     * @return
     */
    List<PackDataInfo> getPackDataInfosWhichGreaterThanGivenValue(Map m);

    /**
     * 分页获取指定时间范围类的测试数据
     *
     * @param m
     * @return
     */
    List<PackDataInfo> getPackDataInfosByTimes(Map m);
}