package com.station.moudles.vo.report;

import java.util.Date;
import java.util.List;
import java.util.Map;

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
	
	@ApiModelProperty(value = "电池组电压判断过滤条件，key=设备类型，value=过滤条件")
	private Map<String, StationReportFilter> filter;

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
	
	public Map<String, StationReportFilter> getFilter() {
		return filter;
	}

	public void setFilter(Map<String, StationReportFilter> filter) {
		this.filter = filter;
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
