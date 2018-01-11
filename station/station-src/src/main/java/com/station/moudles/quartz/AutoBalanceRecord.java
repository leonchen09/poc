package com.station.moudles.quartz;

import java.util.Date;

public class AutoBalanceRecord {
	private String gprsId;
	
	private Date dischargeStartTime; 
	
	private Date balanceStartTime;
	
	private Date balanceEndTime;
	
	private Integer newCellCount;
	
	public String getGprsId() {
		return gprsId;
	}

	public void setGprsId(String gprsId) {
		this.gprsId = gprsId;
	}

	public Date getDischargeStartTime() {
		return dischargeStartTime;
	}

	public void setDischargeStartTime(Date dischargeStartTime) {
		this.dischargeStartTime = dischargeStartTime;
	}

	public Date getBalanceStartTime() {
		return balanceStartTime;
	}

	public void setBalanceStartTime(Date balanceStartTime) {
		this.balanceStartTime = balanceStartTime;
	}

	public Date getBalanceEndTime() {
		return balanceEndTime;
	}

	public void setBalanceEndTime(Date balanceEndTime) {
		this.balanceEndTime = balanceEndTime;
	}

	public Integer getNewCellCount() {
		return newCellCount;
	}

	public void setNewCellCount(Integer newCellCount) {
		this.newCellCount = newCellCount;
	}

}
