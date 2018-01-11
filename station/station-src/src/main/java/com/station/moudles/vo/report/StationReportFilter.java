package com.station.moudles.vo.report;

import java.math.BigDecimal;

import io.swagger.annotations.ApiModelProperty;

public class StationReportFilter {
	@ApiModelProperty(value = "总电压过滤，最大值", required = false)
	private BigDecimal maxGenVol;

	@ApiModelProperty(value = "总电压过滤，最小值", required = false)
	private BigDecimal minGenVol;

	@ApiModelProperty(value = "总电流过滤，最大值", required = false)
	private BigDecimal maxGenCur;

	@ApiModelProperty(value = "总电流过滤，最小值", required = false)
	private BigDecimal minGenCur;
	
	@ApiModelProperty(value = "温度过滤，最大值 ", required = false)
	private Integer maxEnvironTem;
	
	@ApiModelProperty(value = "温度过滤，最小值 ", required = false)
	private Integer minEnvironTem;

	@ApiModelProperty(value = "单体电压过滤，最大值", required = false)
	private BigDecimal maxCellVol;

	@ApiModelProperty(value = "单体电压过滤，最小值", required = false)
	private BigDecimal minCellVol;

	@ApiModelProperty(value = "单体温度过滤，最大值 ", required = false)
	private Integer maxCellTem;

	@ApiModelProperty(value = "单体温度过滤，最小值", required = false)
	private Integer minCellTem;
	
	@ApiModelProperty(value = "单体通讯成功率过滤，最大值 ", required = false)
	private Integer maxComSuc;

	@ApiModelProperty(value = "单体通讯成功率过滤，最小值", required = false)
	private Integer minComSuc;

	public BigDecimal getMaxGenVol() {
		return maxGenVol;
	}

	public void setMaxGenVol(BigDecimal maxGenVol) {
		this.maxGenVol = maxGenVol;
	}

	public BigDecimal getMinGenVol() {
		return minGenVol;
	}

	public void setMinGenVol(BigDecimal minGenVol) {
		this.minGenVol = minGenVol;
	}

	public BigDecimal getMaxGenCur() {
		return maxGenCur;
	}

	public void setMaxGenCur(BigDecimal maxGenCur) {
		this.maxGenCur = maxGenCur;
	}

	public BigDecimal getMinGenCur() {
		return minGenCur;
	}

	public void setMinGenCur(BigDecimal minGenCur) {
		this.minGenCur = minGenCur;
	}

	public Integer getMaxEnvironTem() {
		return maxEnvironTem;
	}

	public void setMaxEnvironTem(Integer maxEnvironTem) {
		this.maxEnvironTem = maxEnvironTem;
	}

	public Integer getMinEnvironTem() {
		return minEnvironTem;
	}

	public void setMinEnvironTem(Integer minEnvironTem) {
		this.minEnvironTem = minEnvironTem;
	}

	public BigDecimal getMaxCellVol() {
		return maxCellVol;
	}

	public void setMaxCellVol(BigDecimal maxCellVol) {
		this.maxCellVol = maxCellVol;
	}

	public BigDecimal getMinCellVol() {
		return minCellVol;
	}

	public void setMinCellVol(BigDecimal minCellVol) {
		this.minCellVol = minCellVol;
	}

	public Integer getMaxCellTem() {
		return maxCellTem;
	}

	public void setMaxCellTem(Integer maxCellTem) {
		this.maxCellTem = maxCellTem;
	}

	public Integer getMinCellTem() {
		return minCellTem;
	}

	public void setMinCellTem(Integer minCellTem) {
		this.minCellTem = minCellTem;
	}

	public Integer getMaxComSuc() {
		return maxComSuc;
	}

	public void setMaxComSuc(Integer maxComSuc) {
		this.maxComSuc = maxComSuc;
	}

	public Integer getMinComSuc() {
		return minComSuc;
	}

	public void setMinComSuc(Integer minComSuc) {
		this.minComSuc = minComSuc;
	}
	
}
