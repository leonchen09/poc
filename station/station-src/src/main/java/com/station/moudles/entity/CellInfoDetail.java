package com.station.moudles.entity;

import java.math.BigDecimal;

import io.swagger.annotations.ApiModelProperty;

public class CellInfoDetail extends CellInfo {
	@ApiModelProperty(value = "单体电池内阻", required = false)
	private BigDecimal cellResist;
	@ApiModelProperty(value = "单体电池单体电压 ", required = false)
	private BigDecimal cellVol;
	@ApiModelProperty(value = "电池单体性能 0:正常,1:较差, 2: 故障", required = false)
	private Integer cellStatus;
	private String cellStatusStr;
	@ApiModelProperty(value = "从机id", example = "从机id", required = false)
	private String subDeviceIdOut;
	@ApiModelProperty(value = "从机id", example = "从机id", required = false)
	private String subDeviceId;
	@ApiModelProperty(value = "电池单体容量", required = false)
	private BigDecimal cellCap;
	// 电池序号根据容量大小从大到小依次排序，1表示电池容量最大的
	private Integer cellCapIndex;
	// 0 未执行任何均衡;1 被动均衡中;2 主动均衡放电中;3 主动均衡充电中
	private Byte cellEqu;
	// 单体温度
	private Integer cellTem;

	private BigDecimal capPercent;

	private BigDecimal startVol;

	private BigDecimal endVol;

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

	public BigDecimal getCapPercent() {
		return capPercent;
	}

	public void setCapPercent(BigDecimal capPercent) {
		this.capPercent = capPercent;
	}

	public String getCellStatusStr() {
		return cellStatusStr;
	}

	public void setCellStatusStr(String cellStatusStr) {
		this.cellStatusStr = cellStatusStr;
	}

	/**
	 * @return the cellTem
	 */
	public Integer getCellTem() {
		return cellTem;
	}

	/**
	 * @param cellTem
	 *            the cellTem to set
	 */
	public void setCellTem(Integer cellTem) {
		this.cellTem = cellTem;
	}

	/**
	 * @return the cellEqu
	 */
	public Byte getCellEqu() {
		return cellEqu;
	}

	/**
	 * @param cellEqu
	 *            the cellEqu to set
	 */
	public void setCellEqu(Byte cellEqu) {
		this.cellEqu = cellEqu;
	}

	/**
	 * @return the cellCapIndex
	 */
	public Integer getCellCapIndex() {
		return cellCapIndex;
	}

	/**
	 * @param cellCapIndex
	 *            the cellCapIndex to set
	 */
	public void setCellCapIndex(Integer cellCapIndex) {
		this.cellCapIndex = cellCapIndex;
	}

	/**
	 * @return the cellCap
	 */
	public BigDecimal getCellCap() {
		return cellCap;
	}

	/**
	 * @param cellCap
	 *            the cellCap to set
	 */
	public void setCellCap(BigDecimal cellCap) {
		this.cellCap = cellCap;
	}

	/**
	 * @return the subDeviceIdOut
	 */
	public String getSubDeviceIdOut() {
		return subDeviceIdOut;
	}

	/**
	 * @param subDeviceIdOut
	 *            the subDeviceIdOut to set
	 */
	public void setSubDeviceIdOut(String subDeviceIdOut) {
		this.subDeviceIdOut = subDeviceIdOut;
	}

	/**
	 * @return the subDeviceId
	 */
	public String getSubDeviceId() {
		return subDeviceId;
	}

	/**
	 * @param subDeviceId
	 *            the subDeviceId to set
	 */
	public void setSubDeviceId(String subDeviceId) {
		this.subDeviceId = subDeviceId;
	}

	/**
	 * @return the cellResist
	 */
	public BigDecimal getCellResist() {
		return cellResist;
	}

	/**
	 * @param cellResist
	 *            the cellResist to set
	 */
	public void setCellResist(BigDecimal cellResist) {
		this.cellResist = cellResist;
	}

	/**
	 * @return the cellVol
	 */
	public BigDecimal getCellVol() {
		return cellVol;
	}

	/**
	 * @param cellVol
	 *            the cellVol to set
	 */
	public void setCellVol(BigDecimal cellVol) {
		this.cellVol = cellVol;
	}

	/**
	 * @return the cellStatus
	 */
	public Integer getCellStatus() {
		return cellStatus;
	}

	/**
	 * @param cellStatus
	 *            the cellStatus to set
	 */
	public void setCellStatus(Integer cellStatus) {
		this.cellStatus = cellStatus;
	}
}
