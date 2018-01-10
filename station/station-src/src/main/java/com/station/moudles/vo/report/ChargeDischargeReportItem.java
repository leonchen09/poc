package com.station.moudles.vo.report;

import java.util.Map;

/**
 * Created by changbin.li on 9/25/17.
 */
public class ChargeDischargeReportItem {

    private String time;
    private String state;
    private String cur; // 电压
    private String vol;//电流
    private Map<Integer, String> cellMap;

    public String getTime() {
        return time;
    }

    public void setTime(String time) {
        this.time = time;
    }

    public String getState() {
        return state;
    }

    public void setState(String state) {
        this.state = state;
    }

    public String getCur() {
        return cur;
    }

    public void setCur(String cur) {
        this.cur = cur;
    }

    public String getVol() {
        return vol;
    }

    public void setVol(String vol) {
        this.vol = vol;
    }

    public Map<Integer, String> getCellMap() {
        return cellMap;
    }

    public void setCellMap(Map<Integer, String> cellMap) {
        this.cellMap = cellMap;
    }
}
