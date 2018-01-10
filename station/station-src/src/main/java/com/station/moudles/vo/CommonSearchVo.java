package com.station.moudles.vo;

import java.util.List;

import javax.validation.constraints.NotNull;

import com.station.moudles.vo.search.PageEntity;

import io.swagger.annotations.ApiModelProperty;

public class CommonSearchVo extends PageEntity {
	@NotNull
	private Integer companyId;
	@NotNull
	private Integer companyLevel;
	@ApiModelProperty(value = "省", example = "省", required = false)
	private String province;
	/**
	 * This field corresponds to the database column base_station_info.city  市
	 */
	@ApiModelProperty(value = "市", example = "市", required = false)
	private String city;
	/**
	 * This field corresponds to the database column base_station_info.district  区
	 */
	@ApiModelProperty(value = "区", example = "区", required = false)
	private String district;
	@ApiModelProperty(value = "基站名称", example = "基站名称", required = false)
	private String name;
	private boolean isGprsIdNotNull = false;
	private List<String> gprsList;

	@ApiModelProperty(value = "0未删除，1删除", required = false)
	private Integer delFlag;

	@ApiModelProperty(value = "设备在线状态,0离线,1在线", required = false)
	private Byte linkStatus;
	
	// -------10/18 add巡检人员姓名
	@ApiModelProperty(value = "巡检人员姓名", required = false)
	private String operateName;
	@ApiModelProperty(value = "安装维护状态 1 安装维护中 2 确定 3 失败", required = false)
	private Integer routingInspectionStatus;

	private String state;
	//用户id
	private String loginId;
	//电压级别
	private Integer volLevel;

	public Integer getVolLevel() {
		return volLevel;
	}

	public void setVolLevel(Integer volLevel) {
		this.volLevel = volLevel;
	}

	public String getLoginId() {
		return loginId;
	}

	public void setLoginId(String loginId) {
		this.loginId = loginId;
	}

	public Integer getRoutingInspectionStatus() {
		return routingInspectionStatus;
	}

	public void setRoutingInspectionStatus(Integer routingInspectionStatus) {
		this.routingInspectionStatus = routingInspectionStatus;
	}

	public String getOperateName() {
		return operateName;
	}

	public void setOperateName(String operateName) {
		this.operateName = operateName;
	}

	public Byte getLinkStatus() {
		return linkStatus;
	}

	public void setLinkStatus(Byte linkStatus) {
		this.linkStatus = linkStatus;
	}

	/**
	 * @return the delFlag
	 */
	public Integer getDelFlag() {
		return delFlag;
	}

	/**
	 * @param delFlag the delFlag to set
	 */
	public void setDelFlag(Integer delFlag) {
		this.delFlag = delFlag;
	}

	/**
	 * @return the companyLevel
	 */
	public Integer getCompanyLevel() {
		return companyLevel;
	}

	/**
	 * @param companyLevel the companyLevel to set
	 */
	public void setCompanyLevel(Integer companyLevel) {
		this.companyLevel = companyLevel;
	}

	/**
	 * @return the isGprsIdNotNull
	 */
	public boolean isGprsIdNotNull() {
		return isGprsIdNotNull;
	}

	/**
	 * @param isGprsIdNotNull the isGprsIdNotNull to set
	 */
	public void setGprsIdNotNull(boolean isGprsIdNotNull) {
		this.isGprsIdNotNull = isGprsIdNotNull;
	}

	/**
	 * @return the companyId
	 */
	public Integer getCompanyId() {
		return companyId;
	}

	/**
	 * @param companyId the companyId to set
	 */
	public void setCompanyId(Integer companyId) {
		this.companyId = companyId;
	}

	/**
	 * @return the province
	 */
	public String getProvince() {
		return province;
	}

	/**
	 * @param province the province to set
	 */
	public void setProvince(String province) {
		this.province = province;
	}

	/**
	 * @return the city
	 */
	public String getCity() {
		return city;
	}

	/**
	 * @param city the city to set
	 */
	public void setCity(String city) {
		this.city = city;
	}

	/**
	 * @return the district
	 */
	public String getDistrict() {
		return district;
	}

	/**
	 * @param district the district to set
	 */
	public void setDistrict(String district) {
		this.district = district;
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
	 * @return the gprsList
	 */
	public List<String> getGprsList() {
		return gprsList;
	}

	/**
	 * @param gprsList the gprsList to set
	 */
	public void setGprsList(List<String> gprsList) {
		this.gprsList = gprsList;
	}

	public String getState() {
		return state;
	}

	public void setState(String state) {
		this.state = state;
	}
}
