package com.station.moudles.vo.report;

/**
 * Created by Jack on 9/16/2017.
 */
public class ModelReportItem {
    private String stationName;
    private String gprsId;
    private String calculationTime;
    private String resistanceStatus;
    private String capacityStatus;
    private String lastResistanceSuccessTime;
    private String lastCapacitySuccessTime;

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

    public String getCalculationTime() {
        return calculationTime;
    }

    public void setCalculationTime(String calculationTime) {
        this.calculationTime = calculationTime;
    }

    public String getResistanceStatus() {
        return resistanceStatus;
    }

    public void setResistanceStatus(String resistanceStatus) {
        this.resistanceStatus = resistanceStatus;
    }

    public String getCapacityStatus() {
        return capacityStatus;
    }

    public void setCapacityStatus(String capacityStatus) {
        this.capacityStatus = capacityStatus;
    }

    public String getLastResistanceSuccessTime() {
        return lastResistanceSuccessTime;
    }

    public void setLastResistanceSuccessTime(String lastResistanceSuccessTime) {
        this.lastResistanceSuccessTime = lastResistanceSuccessTime;
    }

    public String getLastCapacitySuccessTime() {
        return lastCapacitySuccessTime;
    }

    public void setLastCapacitySuccessTime(String lastCapacitySuccessTime) {
        this.lastCapacitySuccessTime = lastCapacitySuccessTime;
    }
}
