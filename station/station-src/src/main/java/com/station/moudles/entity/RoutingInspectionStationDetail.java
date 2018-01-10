package com.station.moudles.entity;

import java.math.BigDecimal;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import com.fasterxml.jackson.annotation.JsonFormat;

import io.swagger.annotations.ApiModelProperty;

public class RoutingInspectionStationDetail extends StationInfo {
	@ApiModelProperty(value = "pk", required = true)
	private Integer routingInspectionId;
	/**
	 * This field corresponds to the database column routing_inspections.station_id  
	 */
	@ApiModelProperty(value = "stationId", required = false)
	private Integer stationId;
	/**
	 * This field corresponds to the database column routing_inspections.operate_type  1首次安装，2更换主从机，3更换单体
	 */
	@ApiModelProperty(value = "1首次安装，2更换主从机，3更换单体", required = false)
	private Integer operateType;
	/**
	 * This field corresponds to the database column routing_inspections.operate_id  
	 */
	@ApiModelProperty(value = "operateId", required = false)
	private Integer operateId;
	/**
	 * This field corresponds to the database column routing_inspections.operate_name  
	 */
	@ApiModelProperty(value = "operateName", example = "operateName", required = false)
	private String operateName;
	/**
	 * This field corresponds to the database column routing_inspections.operate_time  
	 */
	@JsonFormat(pattern = "yyyy-MM-dd HH:mm:ss", timezone = "GMT+8")
	@ApiModelProperty(value = "operateTime")
	private Date operateTime;
	/**
	 * This field corresponds to the database column routing_inspections.routing_inspection_status  1成功，2安装维护中
	 */
	@ApiModelProperty(value = "2成功，1安装维护中", required = false)
	private Integer routingInspectionStatus;
	private List<RoutingInspectionDetail> routingInspectionDetailList;

	@ApiModelProperty(value = "gprsId", example = "gprsId", required = false)
	private String gprsId;

	@ApiModelProperty(value = "巡检状态", example = "inspectStatus", required = false)
	private Integer inspectStatus;
					
	/**
	 *  -----10、18 add 
	 */
	@ApiModelProperty(value = "confirmOperateId", required = false)
	private Integer confirmOperateId;
	


	@ApiModelProperty(value = "confirmOperateName", example = "confirmOperateName", required = false)
	private String confirmOperateName;
	
	@ApiModelProperty(value = "电池单体详情 ", required = false)
	private List<CellInfoDetail> cellInfoDetailList = new ArrayList<CellInfoDetail>();
	
	@ApiModelProperty(value = "1安装主机，2更换主机，3安装霍尔感应器，4更换从机，5更换单体_电池类型,6更换单体_电池品牌", required = false)
	private Integer detailOperateType;
	
	@JsonFormat(pattern = "yyyy-MM-dd HH:mm:ss", timezone = "GMT+8")
	@ApiModelProperty(value = "巡检记录创建时间")
	private Date createTime;

	@ApiModelProperty(value = "操作人员电话", required = false)
	private String operatePhone;
	
	@ApiModelProperty(value = "环境温度", required = false)
	private Integer environTem;
	
	@ApiModelProperty(value = "电池状态： 充电,放电,浮充", example = "电池状态： 充电,放电,浮充", required = false)
	private String state;
	
	@ApiModelProperty(value = "总电压 0~999.999 ", required = false)
	private BigDecimal genVol;
	
	@ApiModelProperty(value = "总电流 -999.999~999.999 负数为充电电流，负数数为放电电流", required = false)
	private BigDecimal genCur;
	
	@ApiModelProperty(value = "最新一条数据", required = false)
    private PackDataInfoLatest packDataInfoLatest;
	
	@ApiModelProperty(value = "安装说明", required = false)
	private String comment;
	
	private  List<Integer> cellIndex = new ArrayList<Integer>() ;
	
	@ApiModelProperty(value = "巡检记录备注",required = false)
	private String remark;
	
	@ApiModelProperty(value = "设备类型", required = false)
	private Integer deviceType;

	public String getGprsId() {
		return gprsId;
	}

	public void setGprsId(String gprsId) {
		this.gprsId = gprsId;
	}

	public Integer getDeviceType() {
		return deviceType;
	}

	public void setDeviceType(Integer deviceType) {
		this.deviceType = deviceType;
	}

	public List<Integer> getCellIndex() {
		return cellIndex;
	}

	public void setCellIndex(List<Integer> cellIndex) {
		this.cellIndex = cellIndex;
	}

	public String getComment() {
		return comment;
	}

	public void setComment(String comment) {
		this.comment = comment;
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

	public Integer getInspectStatus() {
		return inspectStatus;
	}

	public void setInspectStatus(Integer inspectStatus) {
		this.inspectStatus = inspectStatus;
	}

	
	
	public void setConfirmOperateName(String confirmOperateName) {
		this.confirmOperateName = confirmOperateName;
	}
	
	
	public List<CellInfoDetail> getCellInfoDetailList() {
		return cellInfoDetailList;
	}

	public void setCellInfoDetailList(List<CellInfoDetail> cellInfoDetailList) {
		this.cellInfoDetailList = cellInfoDetailList;
	}

	public Integer getDetailOperateType() {
		return detailOperateType;
	}

	public void setDetailOperateType(Integer detailOperateType) {
		this.detailOperateType = detailOperateType;
	}

	public Date getCreateTime() {
		return createTime;
	}

	public void setCreateTime(Date createTime) {
		this.createTime = createTime;
	}
	
	public Integer getEnvironTem() {
		return environTem;
	}

	public void setEnvironTem(Integer environTem) {
		this.environTem = environTem;
	}

	public String getState() {
		return state;
	}

	public void setState(String state) {
		this.state = state;
	}

	public BigDecimal getGenVol() {
		return genVol;
	}

	public void setGenVol(BigDecimal genVol) {
		this.genVol = genVol;
	}

	public BigDecimal getGenCur() {
		return genCur;
	}

	public void setGenCur(BigDecimal genCur) {
		this.genCur = genCur;
	}

	public PackDataInfoLatest getPackDataInfoLatest() {
		return packDataInfoLatest;
	}

	public void setPackDataInfoLatest(PackDataInfoLatest packDataInfoLatest) {
		this.packDataInfoLatest = packDataInfoLatest;
	}

	public String getOperatePhone() {
		return operatePhone;
	}

	public void setOperatePhone(String operatePhone) {
		this.operatePhone = operatePhone;
	}


	/**
	 * @return the routingInspectionDetailList
	 */
	public List<RoutingInspectionDetail> getRoutingInspectionDetailList() {
		return routingInspectionDetailList;
	}

	/**
	 * @param routingInspectionDetailList the routingInspectionDetailList to set
	 */
	public void setRoutingInspectionDetailList(List<RoutingInspectionDetail> routingInspectionDetailList) {
		this.routingInspectionDetailList = routingInspectionDetailList;
	}

	/**
	 * @return the routingInspectionId
	 */
	public Integer getRoutingInspectionId() {
		return routingInspectionId;
	}

	/**
	 * @param routingInspectionId the routingInspectionId to set
	 */
	public void setRoutingInspectionId(Integer routingInspectionId) {
		this.routingInspectionId = routingInspectionId;
	}

	/**
	 * @return the stationId
	 */
	public Integer getStationId() {
		return stationId;
	}

	/**
	 * @param stationId the stationId to set
	 */
	public void setStationId(Integer stationId) {
		this.stationId = stationId;
	}

	/**
	 * @return the operateType
	 */
	public Integer getOperateType() {
		return operateType;
	}

	/**
	 * @param operateType the operateType to set
	 */
	public void setOperateType(Integer operateType) {
		this.operateType = operateType;
	}

	/**
	 * @return the operateId
	 */
	public Integer getOperateId() {
		return operateId;
	}

	/**
	 * @param operateId the operateId to set
	 */
	public void setOperateId(Integer operateId) {
		this.operateId = operateId;
	}

	/**
	 * @return the operateName
	 */
	public String getOperateName() {
		return operateName;
	}

	/**
	 * @param operateName the operateName to set
	 */
	public void setOperateName(String operateName) {
		this.operateName = operateName;
	}

	/**
	 * @return the operateTime
	 */
	public Date getOperateTime() {
		return operateTime;
	}

	/**
	 * @param operateTime the operateTime to set
	 */
	public void setOperateTime(Date operateTime) {
		this.operateTime = operateTime;
	}

	/**
	 * @return the routingInspectionStatus
	 */
	public Integer getRoutingInspectionStatus() {
		return routingInspectionStatus;
	}

	/**
	 * @param routingInspectionStatus the routingInspectionStatus to set
	 */
	public void setRoutingInspectionStatus(Integer routingInspectionStatus) {
		this.routingInspectionStatus = routingInspectionStatus;
	}

	public String getRemark() {
		return remark;
	}

	public void setRemark(String remark) {
		this.remark = remark;
	}
}
