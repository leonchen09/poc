package com.station.moudles.service;

import com.station.moudles.entity.GprsConfigInfo;
import com.station.moudles.entity.PackDataInfo;
import com.station.moudles.entity.PulseDischargeInfo;
import com.station.moudles.entity.StationInfo;
import com.station.moudles.vo.ResponseStatus;
import com.station.moudles.vo.report.ModelReport;

import java.math.BigDecimal;
import java.util.Date;
import java.util.List;
import java.util.Map;

public interface ModelCalculationService {

    /**
     * 内阻、容量模型计算
     *
     * @param stationId 基站编号
     */
    List<ResponseStatus> calculate(Integer stationId);

    /**
     * 取最近一次设备特征测试成功数据, 返回一个哈希Map, key是单体编号，Value是单体特征测试结果
     *
     * @param gprsId 设备编号
     * @return
     */
    Map<Integer, PulseDischargeInfo> getLatestPulseDischargeInfo(String gprsId);

    /**
     * <pre>
     *     最近12次浮充态的cell_vol_X值之和-max(cell_vol_X)-min(cell_vol_X)}/10，X=1~24
     *
     *     取每只电池最近12次浮充态的单体电压数据并去掉最高最低取剩余10个值的平均 值,分别作为单体有效浮充电压, 如单体浮充数据超过一周,则 视为本次模型计算失败。
     *     1、max(cell_vol_X):最近12次浮充态最大的current值,min(cell_vol_X)最近12次浮充态最小的cell_vol_X值，X=1~24
     *     2、最近12次的浮充态的cell_vol_X数据在最近一周内取，如果没有取到则返回模型计算失败。否则为成功。
     * </pre>
     *
     * @return key is cell num(1~24), value is the cell voltage
     */
    Map<Integer, BigDecimal> calculateValidVoltage(String gprsId, Date currentDate);

    List<PackDataInfo> getValidVoltage(String gprsId, Date currentDate);

    BigDecimal get102DischargeAverageCurrent(List<PackDataInfo> items);

    /**
     * 获取最近一次有效放电记录
     *
     * @param gprsId
     * @param date
     * @return
     */
    PackDataInfo getLatestDischargeRecord(String gprsId, Date date);

    /**
     * 获取最近有效放电记录区间数据
     * <p>
     * <pre>
     * "根据这条数据，向前向后查询其它数据，直到有连续10条非放电态的数据出现，则本次放电的数据截止。这其中的所有数据，即为本次放电的详情数据（包括前后连续十条非放电态数据）。
     * 本次放电的详情数据中第一条放电态的数据即为本次放电的开始数据，最后一条放电态的数据即为本次放电的结束数据。"
     * </pre>
     *
     * @param latestDischargeRecord
     * @param gprsId
     * @return
     */
    List<PackDataInfo> getContinuousDischargeRecords(PackDataInfo latestDischargeRecord,
                                                     String gprsId);

    ResponseStatus generateResponseStatus(boolean status, long startTime, String type, String extMessage);

    /**
     * <pre>
     *     CCn=单体标称容量乘以[1-已投入使用年限（精确到1位小数）*20%](n=1~24)
     *     已投入使用年限默认为3，实际使用年限小于等于3年时，则使用实际年限，实际使用年限大于3年时统一设置为3.5年；
     *     单体标称容量：电池出厂容量，由pack_type获取，如pack_type是48V500ah，则标称容量为500。
     *     未足一月按一月计，月数除以１２等于年。比如投入使用时间为2015年５月10日，离当前2017年8月30日时间为：２年３月21天，换成足月为２年４个月，4/12=0.33（取两位小数），则使用年份为：2.33年。
     * </pre>
     *
     * @param stationInfo
     * @param currentDate
     * @return key is cell num(1~24), value is the cell ccn
     */
    Map<Integer, BigDecimal> calculateCCN(StationInfo stationInfo, Date currentDate);

    /**
     * 生成模型计算报表数据结构
     *
     * @param companyId
     * @return
     */
    ModelReport generateModelReport(Integer companyId);
    
    //取得电池标称容量。
    double getStandardCapacityFromPackType(String packType);
    
    BigDecimal calculateSingleEntityResistance(PulseDischargeInfo pulseDischargeInfo, String gprsId);

}
