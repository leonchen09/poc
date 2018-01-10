package com.station.moudles.vo;

import javax.validation.constraints.Pattern;

public class AppConfigVo {
	private String appPhone1;
	private String appPhone2;
	private String appPhone3;

	private String refreshTime;

	private String chargeMaxVol;

	private String chargeMinVol;

	private String deviceMaxCur;

	private String deviceMaxVol;

	private String deviceMinCur;

	private String deviceMinVol;

	private String phoneDesc;

	private String subMaxVol;

	private String subMinVol;
	
	@Pattern(regexp = "^0|(-?[1-9]\\d*\\.?\\d+)$" , message ="电压最大值设置参数非法！")
	private String maxGenVol;
	
	@Pattern(regexp = "^0|(-?[1-9]\\d*\\.?\\d+)$" , message ="电压最小值设置参数非法！")
	private String minGenVol;
	
	@Pattern(regexp = "^0|(-?[1-9]\\d*\\.?\\d+)$" , message ="电流最大值设置参数非法！")
	private String maxGenCur;
	
	@Pattern(regexp = "^0|(-?[1-9]\\d*\\.?\\d+)$" , message ="电流最小值设置参数非法！")
	private String minGenCur;
	
	@Pattern(regexp = "^0|(-?[1-9]\\d*)$" , message ="温度最高值设置参数非法！")
	private String maxEnvironTem;
	
	@Pattern(regexp = "^0|(-?[1-9]\\d*)$" , message ="温度最低值设置参数非法！")
	private String minEnvironTem;
	
	@Pattern(regexp = "^0|(-?[1-9]\\d*\\.?\\d+)$" , message ="单体电压最大值设置参数非法！")
	private String maxCellVol;
	
	@Pattern(regexp = "^0|(-?[1-9]\\d*\\.?\\d+)$" , message ="单体电压最小值设置参数非法！")
	private String minCellVol;
	
	@Pattern(regexp = "^0|(-?[1-9]\\d*)$" , message ="单体温度最高值设置参数非法！")
	private String maxCellTem;
	
	@Pattern(regexp = "^0|(-?[1-9]\\d*)$" , message ="单体温度最低值设置参数非法！")
	private String minCellTem;
	
	@Pattern(regexp = "^0|(-?[1-9]\\d*)$" , message ="通讯成功率最大值设置参数非法！")
	private String maxComSuc;
	
	@Pattern(regexp = "^0|(-?[1-9]\\d*)$" , message ="通讯成功率最小值设置参数非法！")
	private String minComSuc;
	//参数种类
	private String parameterCategory;
	
	public String getParameterCategory() {
		return parameterCategory;
	}

	public void setParameterCategory(String parameterCategory) {
		this.parameterCategory = parameterCategory;
	}

	public String getAppPhone1() {
		return appPhone1;
	}

	public void setAppPhone1(String appPhone1) {
		this.appPhone1 = appPhone1;
	}

	public String getAppPhone2() {
		return appPhone2;
	}

	public void setAppPhone2(String appPhone2) {
		this.appPhone2 = appPhone2;
	}

	public String getAppPhone3() {
		return appPhone3;
	}

	public void setAppPhone3(String appPhone3) {
		this.appPhone3 = appPhone3;
	}

	public String getRefreshTime() {
		return refreshTime;
	}

	public void setRefreshTime(String refreshTime) {
		this.refreshTime = refreshTime;
	}

	public String getChargeMaxVol() {
		return chargeMaxVol;
	}

	public void setChargeMaxVol(String chargeMaxVol) {
		this.chargeMaxVol = chargeMaxVol;
	}

	public String getChargeMinVol() {
		return chargeMinVol;
	}

	public void setChargeMinVol(String chargeMinVol) {
		this.chargeMinVol = chargeMinVol;
	}

	public String getDeviceMaxCur() {
		return deviceMaxCur;
	}

	public void setDeviceMaxCur(String deviceMaxCur) {
		this.deviceMaxCur = deviceMaxCur;
	}

	public String getDeviceMaxVol() {
		return deviceMaxVol;
	}

	public void setDeviceMaxVol(String deviceMaxVol) {
		this.deviceMaxVol = deviceMaxVol;
	}

	public String getDeviceMinCur() {
		return deviceMinCur;
	}

	public void setDeviceMinCur(String deviceMinCur) {
		this.deviceMinCur = deviceMinCur;
	}

	public String getDeviceMinVol() {
		return deviceMinVol;
	}

	public void setDeviceMinVol(String deviceMinVol) {
		this.deviceMinVol = deviceMinVol;
	}

	public String getPhoneDesc() {
		return phoneDesc;
	}

	public void setPhoneDesc(String phoneDesc) {
		this.phoneDesc = phoneDesc;
	}

	public String getSubMaxVol() {
		return subMaxVol;
	}

	public void setSubMaxVol(String subMaxVol) {
		this.subMaxVol = subMaxVol;
	}

	public String getSubMinVol() {
		return subMinVol;
	}

	public void setSubMinVol(String subMinVol) {
		this.subMinVol = subMinVol;
	}

	public String getMaxGenVol() {
		return maxGenVol;
	}

	public void setMaxGenVol(String maxGenVol) {
		this.maxGenVol = maxGenVol;
	}

	public String getMinGenVol() {
		return minGenVol;
	}

	public void setMinGenVol(String minGenVol) {
		this.minGenVol = minGenVol;
	}

	public String getMaxGenCur() {
		return maxGenCur;
	}

	public void setMaxGenCur(String maxGenCur) {
		this.maxGenCur = maxGenCur;
	}

	public String getMinGenCur() {
		return minGenCur;
	}

	public void setMinGenCur(String minGenCur) {
		this.minGenCur = minGenCur;
	}

	public String getMaxEnvironTem() {
		return maxEnvironTem;
	}

	public void setMaxEnvironTem(String maxEnvironTem) {
		this.maxEnvironTem = maxEnvironTem;
	}

	public String getMinEnvironTem() {
		return minEnvironTem;
	}

	public void setMinEnvironTem(String minEnvironTem) {
		this.minEnvironTem = minEnvironTem;
	}

	public String getMaxCellVol() {
		return maxCellVol;
	}

	public void setMaxCellVol(String maxCellVol) {
		this.maxCellVol = maxCellVol;
	}

	public String getMinCellVol() {
		return minCellVol;
	}

	public void setMinCellVol(String minCellVol) {
		this.minCellVol = minCellVol;
	}

	public String getMaxCellTem() {
		return maxCellTem;
	}

	public void setMaxCellTem(String maxCellTem) {
		this.maxCellTem = maxCellTem;
	}

	public String getMinCellTem() {
		return minCellTem;
	}

	public void setMinCellTem(String minCellTem) {
		this.minCellTem = minCellTem;
	}

	public String getMaxComSuc() {
		return maxComSuc;
	}

	public void setMaxComSuc(String maxComSuc) {
		this.maxComSuc = maxComSuc;
	}

	public String getMinComSuc() {
		return minComSuc;
	}

	public void setMinComSuc(String minComSuc) {
		this.minComSuc = minComSuc;
	}
	
}
