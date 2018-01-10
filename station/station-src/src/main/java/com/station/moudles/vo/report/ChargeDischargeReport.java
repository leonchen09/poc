package com.station.moudles.vo.report;

import java.util.List;

public class ChargeDischargeReport {

    private String companyName;
    private String startTime;
    private String endTime;
    private String stationName;
    private String carrier;
    private String maintainanceId;
    private String packType;
    private String roomType;
    private String gprsId;
    private String deviceType;
    private String address;
    private String lat; // 纬度
    private String lng; // 经度
    private String vol;
    private String minCur;
    private String maxCur;

    private List<ChargeDischargeEvent> events;
    private List<ChargeDischargeReportItem> items;   // 截止电压最低放电详情
    private List<ChargeDischargeReportItem> details; // 最近一次有效放电详情

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

    public String getStationName() {
        return stationName;
    }

    public void setStationName(String stationName) {
        this.stationName = stationName;
    }

    public String getCarrier() {
        return carrier;
    }

    public void setCarrier(String carrier) {
        this.carrier = carrier;
    }

    public String getMaintainanceId() {
		return maintainanceId;
	}

	public void setMaintainanceId(String maintainanceId) {
		this.maintainanceId = maintainanceId;
	}

	public String getPackType() {
        return packType;
    }

    public void setPackType(String packType) {
        this.packType = packType;
    }

    public String getRoomType() {
        return roomType;
    }

    public void setRoomType(String roomType) {
        this.roomType = roomType;
    }

    public String getGprsId() {
        return gprsId;
    }

    public void setGprsId(String gprsId) {
        this.gprsId = gprsId;
    }

    public String getDeviceType() {
        return deviceType;
    }

    public void setDeviceType(String deviceType) {
        this.deviceType = deviceType;
    }

    public String getAddress() {
        return address;
    }

    public void setAddress(String address) {
        this.address = address;
    }

    public String getLat() {
        return lat;
    }

    public void setLat(String lat) {
        this.lat = lat;
    }

    public String getLng() {
        return lng;
    }

    public void setLng(String lng) {
        this.lng = lng;
    }

    public String getVol() {
        return vol;
    }

    public void setVol(String vol) {
        this.vol = vol;
    }

    public String getMinCur() {
        return minCur;
    }

    public void setMinCur(String minCur) {
        this.minCur = minCur;
    }

    public String getMaxCur() {
        return maxCur;
    }

    public void setMaxCur(String maxCur) {
        this.maxCur = maxCur;
    }

    public List<ChargeDischargeEvent> getEvents() {
        return events;
    }

    public void setEvents(List<ChargeDischargeEvent> events) {
        this.events = events;
    }

    public List<ChargeDischargeReportItem> getItems() {
        return items;
    }

    public void setItems(List<ChargeDischargeReportItem> items) {
        this.items = items;
    }

    public List<ChargeDischargeReportItem> getDetails() {
        return details;
    }

    public void setDetails(List<ChargeDischargeReportItem> details) {
        this.details = details;
    }
}
