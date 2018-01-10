package com.station.moudles.vo.search;

import java.math.BigDecimal;
import java.util.Date;

import com.fasterxml.jackson.annotation.JsonFormat;

import io.swagger.annotations.ApiModelProperty;

/**
 * 自动检测电流
 * @author ywg
 *
 */
public class SearchDeviceChargeAutocheckVo extends PageEntity {
	
	@ApiModelProperty(value = "pk", example = "pk", required = false)
    private Integer id;
	
	@ApiModelProperty(value = "设备编号",required = false)
    private String gprsId;
	
	@JsonFormat(pattern = "yyyy-MM-dd HH:mm:ss", timezone = "GMT+8")
	@ApiModelProperty(value = "第一次放电时间",required = false)
    private Date firstDischargeTime;
	
	@ApiModelProperty(value = "开始电压",required = false)
    private BigDecimal startVol;
	
	@ApiModelProperty(value = "结束电压",required = false)
    private BigDecimal endVol;

	@ApiModelProperty(value = "数据是否正确",example="放电电压降压为正确", required = false)
    private Integer isCorrect;

	@JsonFormat(pattern = "yyyy-MM-dd HH:mm:ss", timezone = "GMT+8")
	@ApiModelProperty(value = "检测时间",required = false)
    private Date checkDate;

	@JsonFormat(pattern = "yyyy-MM-dd HH:mm:ss", timezone = "GMT+8")
	@ApiModelProperty(value = "设备修复时间",required = false)
    private Date correctDate;
 
	@ApiModelProperty(value = "是否修复",example="0 未修复；1修复",required = false)
    private Integer dataUpdated;
	
	@ApiModelProperty(value = "三级公司id", required = false)
	private Integer companyId3;
	//开始查询时间
	private Date startTime;
	//介绍查询时间
	private Date endTime;

	
    public Date getStartTime() {
		return startTime;
	}

	public void setStartTime(Date startTime) {
		this.startTime = startTime;
	}

	public Date getEndTime() {
		return endTime;
	}

	public void setEndTime(Date endTime) {
		this.endTime = endTime;
	}

	public Integer getCompanyId3() {
		return companyId3;
	}

	public void setCompanyId3(Integer companyId3) {
		this.companyId3 = companyId3;
	}

	public Integer getDataUpdated() {
		return dataUpdated;
	}

	public void setDataUpdated(Integer dataUpdated) {
		this.dataUpdated = dataUpdated;
	}

	public Integer getId() {
        return id;
    }

    public void setId(Integer id) {
        this.id = id;
    }

    public String getGprsId() {
        return gprsId;
    }

    public void setGprsId(String gprsId) {
        this.gprsId = gprsId == null ? null : gprsId.trim();
    }

    public Date getFirstDischargeTime() {
        return firstDischargeTime;
    }

    public void setFirstDischargeTime(Date firstDischargeTime) {
        this.firstDischargeTime = firstDischargeTime;
    }

    public BigDecimal getStartVol() {
        return startVol;
    }

    public void setStartVol(BigDecimal startVol) {
        this.startVol = startVol;
    }

    public BigDecimal getEndVol() {
        return endVol;
    }

    public void setEndVol(BigDecimal endVol) {
        this.endVol = endVol;
    }

    public Integer getIsCorrect() {
        return isCorrect;
    }

    public void setIsCorrect(Integer isCorrect) {
        this.isCorrect = isCorrect;
    }

    public Date getCheckDate() {
        return checkDate;
    }

    public void setCheckDate(Date checkDate) {
        this.checkDate = checkDate;
    }

    public Date getCorrectDate() {
        return correctDate;
    }

    public void setCorrectDate(Date correctDate) {
        this.correctDate = correctDate;
    }
	
	
}
