package com.station.moudles.vo.report;

/**
 * Created by Jack on 9/22/2017.
 */
public class SuggestionReportItem {
    private String companyName;
    private String stationName;
    private String carrier;
    private String maintainanceId;
    private String gprsId;
    private String deviceType;
    private String suggestion1;
    private String suggestion2;
    private String address;

    public String getCompanyName() {
        return companyName;
    }

    public void setCompanyName(String companyName) {
        this.companyName = companyName;
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

    public String getSuggestion1() {
        return suggestion1;
    }

    public void setSuggestion1(String suggestion1) {
        this.suggestion1 = suggestion1;
    }

    public String getSuggestion2() {
        return suggestion2;
    }

    public void setSuggestion2(String suggestion2) {
        this.suggestion2 = suggestion2;
    }

    public String getAddress() {
        return address;
    }

    public void setAddress(String address) {
        this.address = address;
    }
}
