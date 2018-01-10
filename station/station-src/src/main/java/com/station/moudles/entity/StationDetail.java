package com.station.moudles.entity;

import com.fasterxml.jackson.annotation.JsonFormat;
import io.swagger.annotations.ApiModelProperty;

import java.math.BigDecimal;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import com.fasterxml.jackson.annotation.JsonFormat;

import io.swagger.annotations.ApiModelProperty;

public class StationDetail extends StationInfo {
	@JsonFormat(pattern = "yyyy-MM-dd HH:mm:ss", timezone = "GMT+8")
	@ApiModelProperty(value = "updateTime")
	private Date updateTime;
	/**
	 * This field corresponds to the database column
	 * pack_data_expand_latest.pack_cap_pred 电池组容量预测
	 */
	@ApiModelProperty(value = "电池组容量预测", required = false)
	private Integer packCapPred;
	/**
	 * This field corresponds to the database column
	 * pack_data_expand_latest.pack_discharge_time_pred 电池组放点时长预测， 单位min
	 */
	@ApiModelProperty(value = "电池组放点时长预测， 单位min", required = false)
	private BigDecimal packDischargeTimePred;
	@ApiModelProperty(value = "电池状态： 充电,放电,浮充", example = "电池状态： 充电,放电,浮充", required = false)
	private String state;
	/**
	 * This field corresponds to the database column pack_data_info_latest.gen_vol
	 * 总电压 0~999.999
	 */
	@ApiModelProperty(value = "总电压 0~999.999 ", required = false)
	private BigDecimal genVol;
	/**
	 * This field corresponds to the database column pack_data_info_latest.gen_cur
	 * 总电流 -999.999~999.999 负数为充电电流，负数数为放电电流
	 */
	@ApiModelProperty(value = "总电流 -999.999~999.999 负数为充电电流，负数数为放电电流", required = false)
	private BigDecimal genCur;
	@ApiModelProperty(value = "电池单体详情 ", required = false)
	private List<CellInfoDetail> cellInfoDetailList = new ArrayList<CellInfoDetail>();
	@ApiModelProperty(value = "环境温度", required = false)
	private Integer environTem;
	@ApiModelProperty(value = "基站电池容量百分比， 范围 0~100", required = false)
	private Integer soc;

    private String operatorTypeStr;
    private float durationHour;

    private Date currentModelCalculationTime;
    private Integer currentModelCalculationStatus; // 0:内阻容量都成功, 1:内阻成功容量失败, 2:内阻失败容量成功, 3:内阻容量都失败
    private String currentModelCalculationStatusStr;
    private Date lastResistanceCalculationSuccessTime;
    private Date lastCapcityCalculationSuccessTime;

    private PackDataInfoLatest packDataInfoLatest;
    
    @ApiModelProperty(value = "主机标志,1备用, 0不备用", required = false)
    private Integer gprsFlag;
    
    @ApiModelProperty(value = "app显示主机是否正常", required = false)
    
    private boolean isNormal;
    
	public boolean getIsNormal() {
		return isNormal;
	}

	public void setIsNormal(boolean isNormal) {
		this.isNormal = isNormal;
	}

	public float getDurationHour() {
		return durationHour;
	}

	public void setDurationHour(float durationHour) {
		this.durationHour = durationHour;
	}

	public String getOperatorTypeStr() {
		return operatorTypeStr;
	}

	public void setOperatorTypeStr(String operatorTypeStr) {
		this.operatorTypeStr = operatorTypeStr;
	}

	/**
	 * @return the environTem
	 */
	public Integer getEnvironTem() {
		return environTem;
	}

	/**
	 * @param environTem
	 *            the environTem to set
	 */
	public void setEnvironTem(Integer environTem) {
		this.environTem = environTem;
	}

	/**
	 * @return the soc
	 */
	public Integer getSoc() {
		return soc;
	}

	/**
	 * @param soc
	 *            the soc to set
	 */
	public void setSoc(Integer soc) {
		this.soc = soc;
	}

	/**
	 * @return the updateTime
	 */
	public Date getUpdateTime() {
		return updateTime;
	}

	/**
	 * @param updateTime
	 *            the updateTime to set
	 */
	public void setUpdateTime(Date updateTime) {
		this.updateTime = updateTime;
	}

	/**
	 * @return the packCapPred
	 */
	public Integer getPackCapPred() {
		return packCapPred;
	}

	/**
	 * @param packCapPred
	 *            the packCapPred to set
	 */
	public void setPackCapPred(Integer packCapPred) {
		this.packCapPred = packCapPred;
	}

	/**
	 * @return the packDischargeTimePred
	 */
	public BigDecimal getPackDischargeTimePred() {
		return packDischargeTimePred;
	}

	/**
	 * @param packDischargeTimePred
	 *            the packDischargeTimePred to set
	 */
	public void setPackDischargeTimePred(BigDecimal packDischargeTimePred) {
		this.packDischargeTimePred = packDischargeTimePred;
	}

	/**
	 * @return the state
	 */
	public String getState() {
		return state;
	}

	/**
	 * @param state
	 *            the state to set
	 */
	public void setState(String state) {
		this.state = state;
	}

	/**
	 * @return the genVol
	 */
	public BigDecimal getGenVol() {
		return genVol;
	}

	/**
	 * @param genVol
	 *            the genVol to set
	 */
	public void setGenVol(BigDecimal genVol) {
		this.genVol = genVol;
	}

	/**
	 * @return the genCur
	 */
	public BigDecimal getGenCur() {
		return genCur;
	}

	/**
	 * @param genCur
	 *            the genCur to set
	 */
	public void setGenCur(BigDecimal genCur) {
		this.genCur = genCur;
	}

	/**
	 * @return the cellInfoDetailList
	 */
	public List<CellInfoDetail> getCellInfoDetailList() {
		return cellInfoDetailList;
	}

    /**
     * @param cellInfoDetailList the cellInfoDetailList to set
     */
    public void setCellInfoDetailList(List<CellInfoDetail> cellInfoDetailList) {
        this.cellInfoDetailList = cellInfoDetailList;
    }

    public Date getCurrentModelCalculationTime() {
        return currentModelCalculationTime;
    }

    public void setCurrentModelCalculationTime(Date currentModelCalculationTime) {
        this.currentModelCalculationTime = currentModelCalculationTime;
    }

    public Integer getCurrentModelCalculationStatus() {
        return currentModelCalculationStatus;
    }

    public void setCurrentModelCalculationStatus(Integer currentModelCalculationStatus) {
        this.currentModelCalculationStatus = currentModelCalculationStatus;
    }

    public String getCurrentModelCalculationStatusStr() {
        return currentModelCalculationStatusStr;
    }

    public void setCurrentModelCalculationStatusStr(String currentModelCalculationStatusStr) {
        this.currentModelCalculationStatusStr = currentModelCalculationStatusStr;
    }

    public Date getLastResistanceCalculationSuccessTime() {
        return lastResistanceCalculationSuccessTime;
    }

    public void setLastResistanceCalculationSuccessTime(Date lastResistanceCalculationSuccessTime) {
        this.lastResistanceCalculationSuccessTime = lastResistanceCalculationSuccessTime;
    }

    public Date getLastCapcityCalculationSuccessTime() {
        return lastCapcityCalculationSuccessTime;
    }

    public void setLastCapcityCalculationSuccessTime(Date lastCapcityCalculationSuccessTime) {
        this.lastCapcityCalculationSuccessTime = lastCapcityCalculationSuccessTime;
    }

	public PackDataInfoLatest getPackDataInfoLatest() {
		return packDataInfoLatest;
	}

	public void setPackDataInfoLatest(PackDataInfoLatest packDataInfoLatest) {
		this.packDataInfoLatest = packDataInfoLatest;
	}

	public Integer getGprsFlag() {
		return gprsFlag;
	}

	public void setGprsFlag(Integer gprsFlag) {
		this.gprsFlag = gprsFlag;
	}
    
}
