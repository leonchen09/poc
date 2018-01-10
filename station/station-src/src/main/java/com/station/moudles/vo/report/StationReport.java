package com.station.moudles.vo.report;

import java.math.BigDecimal;
import java.util.Date;
import java.util.List;

import com.fasterxml.jackson.annotation.JsonFormat;

import io.swagger.annotations.ApiModelProperty;

public class StationReport {
	@ApiModelProperty(value = "三级公司id", required = false)
	private Integer companyId3;
	
	@ApiModelProperty(value = "公司名", required = false)
	private String companyName;
	
	@ApiModelProperty(value = "电池状态： 充电,放电,浮充")
	private String state;
	
	@ApiModelProperty(value = "设备在线状态,0离线,1在线")
	private Byte linkStatus;
	
	@JsonFormat(pattern = "yyyy-MM-dd HH:mm:ss", timezone = "GMT+8")
	@ApiModelProperty(value = "数据接收时间")
	private Date endRcvTime;

	@JsonFormat(pattern = "yyyy-MM-dd HH:mm:ss", timezone = "GMT+8")
	@ApiModelProperty(value = "数据接收时间")
	private Date startRcvTime;

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
	
	private List<StationReportItem> items;

	private String endRcvTimeStr;
	private String startRcvTimeStr;
	
	private Integer stationTotal;
	
	public Integer getCompanyId3() {
		return companyId3;
	}

	public void setCompanyId3(Integer companyId3) {
		this.companyId3 = companyId3;
	}

	public String getCompanyName() {
		return companyName;
	}

	public void setCompanyName(String companyName) {
		this.companyName = companyName;
	}

	public String getState() {
		return state;
	}

	public void setState(String state) {
		this.state = state;
	}

	public Byte getLinkStatus() {
		return linkStatus;
	}

	public void setLinkStatus(Byte linkStatus) {
		this.linkStatus = linkStatus;
	}

	public Date getEndRcvTime() {
		return endRcvTime;
	}

	public void setEndRcvTime(Date endRcvTime) {
		this.endRcvTime = endRcvTime;
	}

	public Date getStartRcvTime() {
		return startRcvTime;
	}

	public void setStartRcvTime(Date startRcvTime) {
		this.startRcvTime = startRcvTime;
	}

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

	public List<StationReportItem> getItems() {
		return items;
	}

	public void setItems(List<StationReportItem> items) {
		this.items = items;
	}

	public String getEndRcvTimeStr() {
		return endRcvTimeStr;
	}

	public void setEndRcvTimeStr(String endRcvTimeStr) {
		this.endRcvTimeStr = endRcvTimeStr;
	}

	public String getStartRcvTimeStr() {
		return startRcvTimeStr;
	}

	public void setStartRcvTimeStr(String startRcvTimeStr) {
		this.startRcvTimeStr = startRcvTimeStr;
	}

	public Integer getStationTotal() {
		return stationTotal;
	}

	public void setStationTotal(Integer stationTotal) {
		this.stationTotal = stationTotal;
	}

}
