package com.station.moudles.vo.report;

import java.util.List;

/**
 * Created by Jack on 9/22/2017.
 */
public class SuggestionReport {
    private String companyName;
    private String exportTime;
    private String suggestCellCapPercent;
    private String suggestTime;
    private String marginTime;
    private String suggestAverageNum;

    private List<SuggestionReportItem> items;

    public String getCompanyName() {
        return companyName;
    }

    public void setCompanyName(String companyName) {
        this.companyName = companyName;
    }

    public String getExportTime() {
        return exportTime;
    }

    public void setExportTime(String exportTime) {
        this.exportTime = exportTime;
    }

    public String getSuggestCellCapPercent() {
        return suggestCellCapPercent;
    }

    public void setSuggestCellCapPercent(String suggestCellCapPercent) {
        this.suggestCellCapPercent = suggestCellCapPercent;
    }

    public String getSuggestTime() {
        return suggestTime;
    }

    public void setSuggestTime(String suggestTime) {
        this.suggestTime = suggestTime;
    }

    public String getMarginTime() {
        return marginTime;
    }

    public void setMarginTime(String marginTime) {
        this.marginTime = marginTime;
    }

    public String getSuggestAverageNum() {
        return suggestAverageNum;
    }

    public void setSuggestAverageNum(String suggestAverageNum) {
        this.suggestAverageNum = suggestAverageNum;
    }

    public List<SuggestionReportItem> getItems() {
        return items;
    }

    public void setItems(List<SuggestionReportItem> items) {
        this.items = items;
    }
}
