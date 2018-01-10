package com.station.moudles.entity;

import java.math.BigDecimal;
import java.text.DecimalFormat;
import java.util.Date;

import io.swagger.annotations.ApiModel;
import io.swagger.annotations.ApiModelProperty;

/**
 * This class was generated by Bill Generator.
 * This class corresponds to the database table pulse_discharge_info  特征放电表
 *
 * @zdmgenerated 2017-13-19 07:13
 */
@ApiModel(value = "特征放电表", description = "特征放电表描述")
public class PulseDischargeInfo {
	/**
	 * This field corresponds to the database column pulse_discharge_info.id  
	 */
	@ApiModelProperty(value = "pk", required = true)
	private Integer id;

	/**
	 * This field corresponds to the database column pulse_discharge_info.pulse_discharge_send_id  
	 */
	@ApiModelProperty(value = "pulseDischargeSendId", required = false)
	private Integer pulseDischargeSendId;

	/**
	 * This field corresponds to the database column pulse_discharge_info.voltage  
	 */
	@ApiModelProperty(value = "voltage", example = "voltage", required = false)
	private String voltage;

	/**
	 * This field corresponds to the database column pulse_discharge_info.current  
	 */
	@ApiModelProperty(value = "current", example = "current", required = false)
	private String current;
	
	
    @ApiModelProperty(value = "快速采样间隔（区间1和3）， 0：5ms; 1: 10ms; 2: 20ms; 3: 40ms", required = false)
    private Integer fastSampleInterval;
    /**
     * This field corresponds to the database column pulse_discharge_send.slow_sample_interval  慢速采样间隔（区间1和3）， 0：1s; 1: 5s; 2: 10s; 3: 20s
     */
    @ApiModelProperty(value = "慢速采样间隔（区间5）， 0：1s; 1: 5s; 2: 10s; 3: 20s", required = false)
    private Integer slowSampleInterval;
    /**
     * This field corresponds to the database column pulse_discharge_send.discharge_time  区间2时间， 0：1s; 1: 2s; 2: 3s; 3: 4s
     */
    @ApiModelProperty(value = "区间2时间， 0：1s; 1: 2s; 2: 3s; 3: 4s", required = false)
    private Integer dischargeTime;
    /**
     * This field corresponds to the database column pulse_discharge_send.slow_sample_time  慢采集采样时间（区间5）0~600s
     */
    @ApiModelProperty(value = "慢采集采样时间（区间5）0~600s", required = false)
    private Integer slowSampleTime;
    //------add 10/26
    @ApiModelProperty(value = "过滤后的电压", required = false)
    private String filterVoltage;
    @ApiModelProperty(value = "过滤后的电流", required = false)
    private String filterCurrent;
    
    private Integer pulseCell;
    @ApiModelProperty(value = "电池内阻", required = false)
    private BigDecimal impendance;

    private Date endTime;
    
	public BigDecimal getImpendance() {
		return impendance;
	}

	public void setImpendance(BigDecimal impendance) {
		this.impendance = impendance;
	}

	public String getFilterVoltage() {
		return filterVoltage;
	}

	public void setFilterVoltage(String filterVoltage) {
		this.filterVoltage = filterVoltage;
	}

	public String getFilterCurrent() {
		return filterCurrent;
	}

	public void setFilterCurrent(String filterCurrent) {
		this.filterCurrent = filterCurrent;
	}

	/**
	 * This method returns the value of the database column pulse_discharge_info.id  
	 * @return the value of pulse_discharge_info.id
	 */
	public Integer getId() {
		return id;
	}

	/**
	 * This method sets the value of the database column pulse_discharge_info.id  
	 * @param id the value for pulse_discharge_info.id
	 */
	public void setId(Integer id) {
		this.id = id;
	}

	/**
	 * This method returns the value of the database column pulse_discharge_info.pulse_discharge_send_id  
	 * @return the value of pulse_discharge_info.pulse_discharge_send_id
	 */
	public Integer getPulseDischargeSendId() {
		return pulseDischargeSendId;
	}

	/**
	 * This method sets the value of the database column pulse_discharge_info.pulse_discharge_send_id  
	 * @param pulseDischargeSendId the value for pulse_discharge_info.pulse_discharge_send_id
	 */
	public void setPulseDischargeSendId(Integer pulseDischargeSendId) {
		this.pulseDischargeSendId = pulseDischargeSendId;
	}

	/**
	 * This method returns the value of the database column pulse_discharge_info.voltage  
	 * @return the value of pulse_discharge_info.voltage
	 */
	public String getVoltage() {
		return voltage;
	}

	/**
	 * This method sets the value of the database column pulse_discharge_info.voltage  
	 * @param voltage the value for pulse_discharge_info.voltage
	 */
	public void setVoltage(String voltage) {
		this.voltage = voltage == null ? null : voltage.trim();
	}

	/**
	 * This method returns the value of the database column pulse_discharge_info.current  
	 * @return the value of pulse_discharge_info.current
	 */
	public String getCurrent() {
		return current;
	}

	/**
	 * This method sets the value of the database column pulse_discharge_info.current  
	 * @param current the value for pulse_discharge_info.current
	 */
	public void setCurrent(String current) {
		this.current = current == null ? null : current.trim();
	}

	public Integer getFastSampleInterval() {
		return fastSampleInterval;
	}

	public void setFastSampleInterval(Integer fastSampleInterval) {
		this.fastSampleInterval = fastSampleInterval;
	}

	public Integer getSlowSampleInterval() {
		return slowSampleInterval;
	}

	public void setSlowSampleInterval(Integer slowSampleInterval) {
		this.slowSampleInterval = slowSampleInterval;
	}

	public Integer getDischargeTime() {
		return dischargeTime;
	}

	public void setDischargeTime(Integer dischargeTime) {
		this.dischargeTime = dischargeTime;
	}

	public Integer getSlowSampleTime() {
		return slowSampleTime;
	}

	public void setSlowSampleTime(Integer slowSampleTime) {
		this.slowSampleTime = slowSampleTime;
	}

	public Integer getPulseCell() {
		return pulseCell;
	}

	public void setPulseCell(Integer pulseCell) {
		this.pulseCell = pulseCell;
	}

	public Date getEndTime() {
		return endTime;
	}

	public void setEndTime(Date endTime) {
		this.endTime = endTime;
	}
	
}