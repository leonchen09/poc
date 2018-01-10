/*
 * @Date 2017-11-23 17:49:57
 * 设备放电自动检测类
 */
package com.station.moudles.entity;

import java.math.BigDecimal;
import java.util.Date;

public class DeviceDischargeAutocheck {
    /**
     * @Fields id 
     */
    private Integer id;
    /**
     * @Fields gprsId 
     */
    private String gprsId;
    /**
     * @Fields firstDischargeTime 第一次放电时间
     */
    private Date firstDischargeTime;
    /**
     * @Fields startVol 开始电压
     */
    private BigDecimal startVol;
    /**
     * @Fields endVol 结束电压
     */
    private BigDecimal endVol;
    /**
     * @Fields isCorrect 数据状态是否正确，放电电压降低即为正确。
     */
    private Integer isCorrect;
    /**
     * @Fields checkDate 检查时间，写入该记录的时间
     */
    private Date checkDate;
    /**
     * @Fields correctDate 设备状态修复的时间
     */
    private Date correctDate;
    /**
     * 是否已经修复 0 未修复 1 修复
     */
    private Integer dataUpdated;
    

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