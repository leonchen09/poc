package com.station.moudles.service;

import java.util.Date;
import java.util.List;

import com.station.moudles.helper.AbstractEvent;
import com.station.moudles.helper.ChargeEvent;
import com.station.moudles.helper.DischargeEvent;
import com.station.moudles.helper.EventParams;
import com.station.moudles.helper.LossChargeEvent;
import com.station.moudles.vo.report.ChargeDischargeEvent;
import com.station.moudles.vo.report.ChargeDischargeReport;
import com.station.moudles.vo.report.PulseReport;
import com.station.moudles.vo.report.StationReport;
import com.station.moudles.vo.report.SuggestionReport;

/**
 * Created by Jack on 9/17/2017.
 */
public interface ReportService {

    /**
     * 生成特征测试统计数据
     *
     * @param companyId
     * @param startTime
     * @param endTime
     * @return
     */
    PulseReport generatePulseReport(Integer companyId, Date startTime, Date endTime);

    /**
     * 整治建议
     *
     * @param companyId
     * @param province
     * @param city
     * @param district
     * @return
     */
    SuggestionReport generateSuggestionReport(Integer companyId, String province, String city, String district);

    /**
     * @param companyId
     * @param province
     * @param city
     * @param district
     * @return
     */
    SuggestionReport getSuggestionReportItems(Integer companyId, String province, String city, String district);

    SuggestionReport generateSuggestionReport(Integer stationId);

    ChargeDischargeReport getChargeDischargeReport(Integer stationId, Date startTime, Date endTime,
    												ChargeEvent chargeEvent,
    												DischargeEvent dischargeEvent,
    												LossChargeEvent lossChargeEvent);
    
    StationReport generateStationVolCurStr(StationReport stationReport);
    
    
    List<ChargeDischargeEvent> getChargeDischargeEvent(String gprsId, Date startTime, Date endTime,
			AbstractEvent event,
			EventParams params);
}
