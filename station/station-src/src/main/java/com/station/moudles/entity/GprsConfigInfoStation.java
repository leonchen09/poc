package com.station.moudles.entity;

import io.swagger.annotations.ApiModelProperty;

public class GprsConfigInfoStation extends GprsConfigInfo {
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

}
