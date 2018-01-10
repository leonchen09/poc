package com.station.moudles.vo.report;

import java.math.BigDecimal;

import io.swagger.annotations.ApiModelProperty;

public class WarnReport {
	@ApiModelProperty(value = "公司", required = false)
	private String company;
	
	@ApiModelProperty(value = "公司和区域", required = false)
	private String companyAndArea;
	
	@ApiModelProperty(value = "时间", required = false)
	private String exportDateTime;
	
	@ApiModelProperty(value = "总电压过高告警阈值,V", required = false)
	private BigDecimal volHighWarningThreshold;
	
	@ApiModelProperty(value = "总电压过低告警阈值 V", required = false)
	private BigDecimal volLowWarningThreshold;
	
	@ApiModelProperty(value = "温度过高告警阈值 ℃", required = false)
	private Integer temHighWarningThreshold;
	
	@ApiModelProperty(value = "温度过低告警阈值", required = false)
	private Integer temLowWarningThreshold;
	
	@ApiModelProperty(value = "SOC过低告警阈值", required = false)
	private Integer socLowWarningThreshold;
	
	public String getCompany() {
		return company;
	}

	public void setCompany(String company) {
		this.company = company;
	}

	public String getCompanyAndArea() {
		return companyAndArea;
	}

	public void setCompanyAndArea(String companyAndArea) {
		this.companyAndArea = companyAndArea;
	}

	public String getExportDateTime() {
		return exportDateTime;
	}

	public void setExportDateTime(String exportDateTime) {
		this.exportDateTime = exportDateTime;
	}

	public BigDecimal getVolHighWarningThreshold() {
		return volHighWarningThreshold;
	}

	public void setVolHighWarningThreshold(BigDecimal volHighWarningThreshold) {
		this.volHighWarningThreshold = volHighWarningThreshold;
	}

	public BigDecimal getVolLowWarningThreshold() {
		return volLowWarningThreshold;
	}

	public void setVolLowWarningThreshold(BigDecimal volLowWarningThreshold) {
		this.volLowWarningThreshold = volLowWarningThreshold;
	}

	public Integer getTemHighWarningThreshold() {
		return temHighWarningThreshold;
	}

	public void setTemHighWarningThreshold(Integer temHighWarningThreshold) {
		this.temHighWarningThreshold = temHighWarningThreshold;
	}

	public Integer getTemLowWarningThreshold() {
		return temLowWarningThreshold;
	}

	public void setTemLowWarningThreshold(Integer temLowWarningThreshold) {
		this.temLowWarningThreshold = temLowWarningThreshold;
	}

	public Integer getSocLowWarningThreshold() {
		return socLowWarningThreshold;
	}

	public void setSocLowWarningThreshold(Integer socLowWarningThreshold) {
		this.socLowWarningThreshold = socLowWarningThreshold;
	}
}
