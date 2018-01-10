package com.station.moudles.vo.report;

import java.util.Map;

/**
 * Created by Jack on 9/17/2017.
 */
public class PulseReportItem {
    private String stationName;
    private String gprsId;
    private Map<Integer, Byte> cellStatusMap;

    public String getStationName() {
        return stationName;
    }

    public void setStationName(String stationName) {
        this.stationName = stationName;
    }

    public String getGprsId() {
        return gprsId;
    }

    public void setGprsId(String gprsId) {
        this.gprsId = gprsId;
    }

    public Map<Integer, Byte> getCellStatusMap() {
        return cellStatusMap;
    }

    public void setCellStatusMap(Map<Integer, Byte> cellStatusMap) {
        this.cellStatusMap = cellStatusMap;
    }
}
