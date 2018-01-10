package com.station.moudles.entity;

import io.swagger.annotations.ApiModelProperty;

public class StationWarningInfo extends WarningInfo {
	@ApiModelProperty(value = "三级公司id", required = false)
	private Integer companyId3;
	@ApiModelProperty(value = "companyName3", example = "companyName3", required = false)
	private String companyName3;
	@ApiModelProperty(value = "基站名称", example = "基站名称", required = false)
	private String name;
	@ApiModelProperty(value = "运营商类型,1移动，2联通，3电信", required = false)
	private Integer operatorType;
	@ApiModelProperty(value = "运维ID", example = "运维ID", required = false)
	private String maintainanceId;
	@ApiModelProperty(value = "基站地址", example = "基站地址", required = false)
	private String address;
	private String operatorTypeStr;
	@ApiModelProperty(value = "掉电告警", required = false)
	private String lossElectricityStr;
	@ApiModelProperty(value = "总电压告警", required = false)
	private String genVolStr;
	@ApiModelProperty(value = "环境温度告警", required = false)
	private String envTemStr;
	@ApiModelProperty(value = "单体温度告警", required = false)
	private String cellTemStr;
	@ApiModelProperty(value = "剩余电量告警", required = false)
	private String socStr;
	@ApiModelProperty(value = "设备类型，1蓄电池串联复用设备,2蓄电池串联复用诊断组件", required = false)
	private Integer deviceType;

	/**
	 * @return the deviceType
	 */
	public Integer getDeviceType() {
		return deviceType;
	}

	/**
	 * @param deviceType the deviceType to set
	 */
	public void setDeviceType(Integer deviceType) {
		this.deviceType = deviceType;
	}

	/**
	 * @return the operatorTypeStr
	 */
	public String getOperatorTypeStr() {
		return operatorTypeStr;
	}

	/**
	 * @param operatorTypeStr the operatorTypeStr to set
	 */
	public void setOperatorTypeStr(String operatorTypeStr) {
		this.operatorTypeStr = operatorTypeStr;
	}

	/**
	 * @return the lossElectricityStr
	 */
	public String getLossElectricityStr() {
		return lossElectricityStr;
	}

	/**
	 * @param lossElectricityStr the lossElectricityStr to set
	 */
	public void setLossElectricityStr(String lossElectricityStr) {
		this.lossElectricityStr = lossElectricityStr;
	}

	/**
	 * @return the genVolStr
	 */
	public String getGenVolStr() {
		return genVolStr;
	}

	/**
	 * @param genVolStr the genVolStr to set
	 */
	public void setGenVolStr(String genVolStr) {
		this.genVolStr = genVolStr;
	}

	/**
	 * @return the envTemStr
	 */
	public String getEnvTemStr() {
		return envTemStr;
	}

	/**
	 * @param envTemStr the envTemStr to set
	 */
	public void setEnvTemStr(String envTemStr) {
		this.envTemStr = envTemStr;
	}

	/**
	 * @return the cellTemStr
	 */
	public String getCellTemStr() {
		return cellTemStr;
	}

	/**
	 * @param cellTemStr the cellTemStr to set
	 */
	public void setCellTemStr(String cellTemStr) {
		this.cellTemStr = cellTemStr;
	}

	/**
	 * @return the socStr
	 */
	public String getSocStr() {
		return socStr;
	}

	/**
	 * @param socStr the socStr to set
	 */
	public void setSocStr(String socStr) {
		this.socStr = socStr;
	}

	/**
	 * @return the companyId3
	 */
	public Integer getCompanyId3() {
		return companyId3;
	}

	/**
	 * @param companyId3 the companyId3 to set
	 */
	public void setCompanyId3(Integer companyId3) {
		this.companyId3 = companyId3;
	}

	/**
	 * @return the companyName3
	 */
	public String getCompanyName3() {
		return companyName3;
	}

	/**
	 * @param companyName3 the companyName3 to set
	 */
	public void setCompanyName3(String companyName3) {
		this.companyName3 = companyName3;
	}

	/**
	 * @return the name
	 */
	public String getName() {
		return name;
	}

	/**
	 * @param name the name to set
	 */
	public void setName(String name) {
		this.name = name;
	}

	/**
	 * @return the operatorType
	 */
	public Integer getOperatorType() {
		return operatorType;
	}

	/**
	 * @param operatorType the operatorType to set
	 */
	public void setOperatorType(Integer operatorType) {
		this.operatorType = operatorType;
	}

	/**
	 * @return the maintainanceId
	 */
	public String getMaintainanceId() {
		return maintainanceId;
	}

	/**
	 * @param maintainanceId the maintainanceId to set
	 */
	public void setMaintainanceId(String maintainanceId) {
		this.maintainanceId = maintainanceId;
	}

	/**
	 * @return the address
	 */
	public String getAddress() {
		return address;
	}

	/**
	 * @param address the address to set
	 */
	public void setAddress(String address) {
		this.address = address;
	}
}
