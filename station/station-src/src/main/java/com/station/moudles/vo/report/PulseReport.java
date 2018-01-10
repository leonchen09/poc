package com.station.moudles.vo.report;

import java.util.List;

/**
 * Created by Jack on 9/17/2017.
 */
public class PulseReport {
    private String companyName;
    private String startTime;
    private String endTime;
    private List<PulseReportItem> items;

    public String getCompanyName() {
        return companyName;
    }

    public void setCompanyName(String companyName) {
        this.companyName = companyName;
    }

    public String getStartTime() {
        return startTime;
    }

    public void setStartTime(String startTime) {
        this.startTime = startTime;
    }

    public String getEndTime() {
        return endTime;
    }

    public void setEndTime(String endTime) {
        this.endTime = endTime;
    }

    public List<PulseReportItem> getItems() {
        return items;
    }

    public void setItems(List<PulseReportItem> items) {
        this.items = items;
    }
}
