package com.station.moudles.entity;

import java.util.Date;

import com.fasterxml.jackson.annotation.JsonFormat;

import io.swagger.annotations.ApiModelProperty;

public class RoutingInspectionPackData extends PackDataInfoLatest{

	@ApiModelProperty(value = "pk", required = true)
	private Integer routingInspectionId;

	@ApiModelProperty(value = "stationId", required = false)
	private Integer stationId;

	@ApiModelProperty(value = "1首次安装，2更换主从机，3更换单体", required = false)
	private Integer operateType;

	@ApiModelProperty(value = "operateId", required = false)
	private Integer operateId;

	@ApiModelProperty(value = "operateName", example = "operateName", required = false)
	private String operateName;

	@JsonFormat(pattern = "yyyy-MM-dd HH:mm:ss", timezone = "GMT+8")
	@ApiModelProperty(value = "operateTime")
	private Date operateTime;

	@ApiModelProperty(value = "1安装维护中 ,2成功,3 失败" , required = false)
	private Integer routingInspectionStatus;

	@ApiModelProperty(value = "confirmOperateId", required = false)
	private Integer confirmOperateId;
	
	@ApiModelProperty(value = "confirmOperateName", example = "confirmOperateName", required = false)
	private String confirmOperateName;
	//操作人员电话
	@ApiModelProperty(value = "operatePhone", example = "operatePhone", required = false)
	private String operatePhone;

	@ApiModelProperty(value = "巡检记录备注",required = false)
	private String remark;
	
	@ApiModelProperty(value = "设备类型", required = false)
	private Integer deviceType;

	public Integer getDeviceType() {
		return deviceType;
	}

	public void setDeviceType(Integer deviceType) {
		this.deviceType = deviceType;
	}


	public String getOperatePhone() {
		return operatePhone;
	}

	public void setOperatePhone(String operatePhone) {
		this.operatePhone = operatePhone;
	}

	public Integer getRoutingInspectionId() {
		return routingInspectionId;
	}

	public void setRoutingInspectionId(Integer routingInspectionId) {
		this.routingInspectionId = routingInspectionId;
	}

	public Integer getStationId() {
		return stationId;
	}

	public void setStationId(Integer stationId) {
		this.stationId = stationId;
	}

	public Integer getOperateType() {
		return operateType;
	}

	public void setOperateType(Integer operateType) {
		this.operateType = operateType;
	}

	public Integer getOperateId() {
		return operateId;
	}

	public void setOperateId(Integer operateId) {
		this.operateId = operateId;
	}

	public String getOperateName() {
		return operateName;
	}

	public void setOperateName(String operateName) {
		this.operateName = operateName == null ? null : operateName.trim();
	}

	public Date getOperateTime() {
		return operateTime;
	}

	public void setOperateTime(Date operateTime) {
		this.operateTime = operateTime;
	}

	public Integer getRoutingInspectionStatus() {
		return routingInspectionStatus;
	}

	public void setRoutingInspectionStatus(Integer routingInspectionStatus) {
		this.routingInspectionStatus = routingInspectionStatus;
	}

	public Integer getConfirmOperateId() {
		return confirmOperateId;
	}

	public void setConfirmOperateId(Integer confirmOperateId) {
		this.confirmOperateId = confirmOperateId;
	}

	public String getConfirmOperateName() {
		return confirmOperateName;
	}

	public void setConfirmOperateName(String confirmOperateName) {
		this.confirmOperateName = confirmOperateName;
	}

	public String getRemark() {
		return remark;
	}

	public void setRemark(String remark) {
		this.remark = remark;
	}
}