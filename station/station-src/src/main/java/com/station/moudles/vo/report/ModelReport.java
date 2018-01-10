package com.station.moudles.vo.report;

import java.util.List;

/**
 * Created by Jack on 9/16/2017.
 */
public class ModelReport {
    private String companyName;
    private String exportDate;
    private List<ModelReportItem> items;

    public String getCompanyName() {
        return companyName;
    }

    public void setCompanyName(String companyName) {
        this.companyName = companyName;
    }

    public String getExportDate() {
        return exportDate;
    }

    public void setExportDate(String exportDate) {
        this.exportDate = exportDate;
    }

    public List<ModelReportItem> getItems() {
        return items;
    }

    public void setItems(List<ModelReportItem> items) {
        this.items = items;
    }
}
