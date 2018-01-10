package com.station.moudles.vo.search;

import io.swagger.annotations.ApiModel;
import io.swagger.annotations.ApiModelProperty;

/**
 * This class was generated by Bill Generator.
 * This class corresponds to the database table pulse_discharge_info  特征放电表
 *
 * @zdmgenerated 2017-13-19 07:13
 */
@ApiModel(value="特征放电表查询",description="特征放电表查询描述")
public class SearchPulseDischargeInfoPagingVo extends PageEntity {
    /**
     * This field corresponds to the database column pulse_discharge_info.id  
     */
    @ApiModelProperty(value="pk",required=false)
    private Integer id;

    /**
     * This field corresponds to the database column pulse_discharge_info.pulse_discharge_send_id  
     */
    @ApiModelProperty(value="pulseDischargeSendId",required=false)
    private Integer pulseDischargeSendId;

    /**
     * This field corresponds to the database column pulse_discharge_info.voltage  
     */
    @ApiModelProperty(value="voltage",example="voltage",required=false)
    private String voltage;

    /**
     * This field corresponds to the database column pulse_discharge_info.current  
     */
    @ApiModelProperty(value="current",example="current",required=false)
    private String current;
    
    //------add 10/26
    @ApiModelProperty(value = "过滤后的电压", required = false)
    private String filterVoltage;
    @ApiModelProperty(value = "过滤后的电流", required = false)
    private String filterCurrent;

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
}