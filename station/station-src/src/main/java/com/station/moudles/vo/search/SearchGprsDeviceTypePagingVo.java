package com.station.moudles.vo.search;

import java.math.BigDecimal;
import java.util.Date;

public class SearchGprsDeviceTypePagingVo extends PageEntity {
	/**
     * @Fields id 
     */
    private Integer id;
    /**
     * @Fields typeCode 设备类型编号
     */
    private Integer typeCode;
    /**
     * @Fields typeName 设备类型
     */
    private String typeName;
    /**
     * @Fields subVol 从机电压
     */
    private BigDecimal subVol;
    /**
     * @Fields volLevel 电压平台编码 FK
     */
    private Integer volLevel;
    /**
     * @Fields createId 创建用户id FK
     */
    private String createId;
    /**
     * @Fields createTime 创建时间
     */
    private Date createTime;
    //电压平台名称
    private String volLevelName;
    //从机数量
    private Integer subDeviceCount;
    
	public Integer getSubDeviceCount() {
		return subDeviceCount;
	}
	public void setSubDeviceCount(Integer subDeviceCount) {
		this.subDeviceCount = subDeviceCount;
	}
	public String getVolLevelName() {
		return volLevelName;
	}
	public void setVolLevelName(String volLevelName) {
		this.volLevelName = volLevelName;
	}
	public Integer getId() {
		return id;
	}
	public void setId(Integer id) {
		this.id = id;
	}
	public Integer getTypeCode() {
		return typeCode;
	}
	public void setTypeCode(Integer typeCode) {
		this.typeCode = typeCode;
	}
	public String getTypeName() {
		return typeName;
	}
	public void setTypeName(String typeName) {
		this.typeName = typeName;
	}
	public BigDecimal getSubVol() {
		return subVol;
	}
	public void setSubVol(BigDecimal subVol) {
		this.subVol = subVol;
	}
	public Integer getVolLevel() {
		return volLevel;
	}
	public void setVolLevel(Integer volLevel) {
		this.volLevel = volLevel;
	}
	public String getCreateId() {
		return createId;
	}
	public void setCreateId(String createId) {
		this.createId = createId;
	}
	public Date getCreateTime() {
		return createTime;
	}
	public void setCreateTime(Date createTime) {
		this.createTime = createTime;
	}
    
    
}
