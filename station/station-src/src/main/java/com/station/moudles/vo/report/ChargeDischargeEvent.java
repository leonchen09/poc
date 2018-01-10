package com.station.moudles.vo.report;

import java.util.List;

import com.station.moudles.entity.PackDataInfo;

/**
 * Created by changbin.li on 9/25/17.
 */
public class ChargeDischargeEvent {

    private String startTime;
    private String endTime;
    private String event;
    private String startVoltage;
    private String endVoltage;
    private List<PackDataInfo> details;

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

    public String getEvent() {
        return event;
    }

    public void setEvent(String event) {
        this.event = event;
    }

	public String getStartVoltage() {
		return startVoltage;
	}

	public void setStartVoltage(String startVoltage) {
		this.startVoltage = startVoltage;
	}

	public String getEndVoltage() {
		return endVoltage;
	}

	public void setEndVoltage(String endVoltage) {
		this.endVoltage = endVoltage;
	}

	public List<PackDataInfo> getDetails() {
		return details;
	}

	public void setDetails(List<PackDataInfo> details) {
		this.details = details;
	}
}
