package com.station.moudles.vo.report;

import java.math.BigDecimal;

import io.swagger.annotations.ApiModelProperty;

public class StationReportItem {
	@ApiModelProperty(value = "三级公司")
	private String companyName3;

	@ApiModelProperty(value = "基站gprs ID", example = "基站gprs ID", required = false)
	private String gprsId;

	@ApiModelProperty(value = "基站名称", example = "基站名称", required = false)
	private String name;
	
	@ApiModelProperty(value = "基站地址", example = "基站地址", required = false)
	private String address;

	@ApiModelProperty(value = "电池状态： 充电,放电,浮充")
	private String state;

	@ApiModelProperty(value = "设备在线状态,0离线,1在线")
	private Byte linkStatus;

	/*@ApiModelProperty(value = "总电压", required = false)
	private BigDecimal genVol;

	@ApiModelProperty(value = "总电流", required = false)
	private BigDecimal genCur;

	@ApiModelProperty(value = "温度", required = false)
	private Integer environTem;*/
	
	@ApiModelProperty(value = "备注", required = false)
	private String remark;
	
	@ApiModelProperty(value = "总电压", required = false)
	private String genVolStr;

	@ApiModelProperty(value = "总电流", required = false)
	private String genCurStr;

	@ApiModelProperty(value = "温度", required = false)
	private String environTemStr;

	@ApiModelProperty(value = "单体电压、通讯成功率、温度信息", required = false)
	private String cellStr1;
	private String cellStr2;
	private String cellStr3;
	private String cellStr4;
	private String cellStr5;
	private String cellStr6;
	private String cellStr7;
	private String cellStr8;
	private String cellStr9;
	private String cellStr10;
	private String cellStr11;
	private String cellStr12;
	private String cellStr13;
	private String cellStr14;
	private String cellStr15;
	private String cellStr16;
	private String cellStr17;
	private String cellStr18;
	private String cellStr19;
	private String cellStr20;
	private String cellStr21;
	private String cellStr22;
	private String cellStr23;
	private String cellStr24;

	public String getCompanyName3() {
		return companyName3;
	}

	public void setCompanyName3(String companyName3) {
		this.companyName3 = companyName3;
	}

	public String getGprsId() {
		return gprsId;
	}

	public void setGprsId(String gprsId) {
		this.gprsId = gprsId;
	}

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public String getAddress() {
		return address;
	}

	public void setAddress(String address) {
		this.address = address;
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

	public String getRemark() {
		return remark;
	}

	public void setRemark(String remark) {
		this.remark = remark;
	}

	public String getGenVolStr() {
		return genVolStr;
	}

	public void setGenVolStr(String genVolStr) {
		this.genVolStr = genVolStr;
	}

	public String getGenCurStr() {
		return genCurStr;
	}

	public void setGenCurStr(String genCurStr) {
		this.genCurStr = genCurStr;
	}

	public String getEnvironTemStr() {
		return environTemStr;
	}

	public void setEnvironTemStr(String environTemStr) {
		this.environTemStr = environTemStr;
	}

	public String getCellStr1() {
		return cellStr1;
	}

	public void setCellStr1(String cellStr1) {
		this.cellStr1 = cellStr1;
	}

	public String getCellStr2() {
		return cellStr2;
	}

	public void setCellStr2(String cellStr2) {
		this.cellStr2 = cellStr2;
	}

	public String getCellStr3() {
		return cellStr3;
	}

	public void setCellStr3(String cellStr3) {
		this.cellStr3 = cellStr3;
	}

	public String getCellStr4() {
		return cellStr4;
	}

	public void setCellStr4(String cellStr4) {
		this.cellStr4 = cellStr4;
	}

	public String getCellStr5() {
		return cellStr5;
	}

	public void setCellStr5(String cellStr5) {
		this.cellStr5 = cellStr5;
	}

	public String getCellStr6() {
		return cellStr6;
	}

	public void setCellStr6(String cellStr6) {
		this.cellStr6 = cellStr6;
	}

	public String getCellStr7() {
		return cellStr7;
	}

	public void setCellStr7(String cellStr7) {
		this.cellStr7 = cellStr7;
	}

	public String getCellStr8() {
		return cellStr8;
	}

	public void setCellStr8(String cellStr8) {
		this.cellStr8 = cellStr8;
	}

	public String getCellStr9() {
		return cellStr9;
	}

	public void setCellStr9(String cellStr9) {
		this.cellStr9 = cellStr9;
	}

	public String getCellStr10() {
		return cellStr10;
	}

	public void setCellStr10(String cellStr10) {
		this.cellStr10 = cellStr10;
	}

	public String getCellStr11() {
		return cellStr11;
	}

	public void setCellStr11(String cellStr11) {
		this.cellStr11 = cellStr11;
	}

	public String getCellStr12() {
		return cellStr12;
	}

	public void setCellStr12(String cellStr12) {
		this.cellStr12 = cellStr12;
	}

	public String getCellStr13() {
		return cellStr13;
	}

	public void setCellStr13(String cellStr13) {
		this.cellStr13 = cellStr13;
	}

	public String getCellStr14() {
		return cellStr14;
	}

	public void setCellStr14(String cellStr14) {
		this.cellStr14 = cellStr14;
	}

	public String getCellStr15() {
		return cellStr15;
	}

	public void setCellStr15(String cellStr15) {
		this.cellStr15 = cellStr15;
	}

	public String getCellStr16() {
		return cellStr16;
	}

	public void setCellStr16(String cellStr16) {
		this.cellStr16 = cellStr16;
	}

	public String getCellStr17() {
		return cellStr17;
	}

	public void setCellStr17(String cellStr17) {
		this.cellStr17 = cellStr17;
	}

	public String getCellStr18() {
		return cellStr18;
	}

	public void setCellStr18(String cellStr18) {
		this.cellStr18 = cellStr18;
	}

	public String getCellStr19() {
		return cellStr19;
	}

	public void setCellStr19(String cellStr19) {
		this.cellStr19 = cellStr19;
	}

	public String getCellStr20() {
		return cellStr20;
	}

	public void setCellStr20(String cellStr20) {
		this.cellStr20 = cellStr20;
	}

	public String getCellStr21() {
		return cellStr21;
	}

	public void setCellStr21(String cellStr21) {
		this.cellStr21 = cellStr21;
	}

	public String getCellStr22() {
		return cellStr22;
	}

	public void setCellStr22(String cellStr22) {
		this.cellStr22 = cellStr22;
	}

	public String getCellStr23() {
		return cellStr23;
	}

	public void setCellStr23(String cellStr23) {
		this.cellStr23 = cellStr23;
	}

	public String getCellStr24() {
		return cellStr24;
	}

	public void setCellStr24(String cellStr24) {
		this.cellStr24 = cellStr24;
	}

}
