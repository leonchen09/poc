/*
 * @ClassName cellVolLevel
 * @Description 
 * @version 1.0
 * @Date 2017-12-25 13:45:08
 */
package com.station.moudles.entity;

import java.util.Date;
public class CellVolLevel {
    /**
     * @Fields id 
     */
    private Integer id;
    /**
     * @Fields volLevelName 电压平台名称
     */
    private String volLevelName;
    /**
     * @Fields volLevelCode 电压平台编码
     */
    private Integer volLevelCode;
    /**
     * @Fields createId 用户id FK
     */
    private String createId;
    /**
     * @Fields createTime 创建时间
     */
    private Date createTime;

    public Integer getId() {
        return id;
    }

    public void setId(Integer id) {
        this.id = id;
    }

    public String getVolLevelName() {
        return volLevelName;
    }

    public void setVolLevelName(String volLevelName) {
        this.volLevelName = volLevelName == null ? null : volLevelName.trim();
    }

    public Integer getVolLevelCode() {
        return volLevelCode;
    }

    public void setVolLevelCode(Integer volLevelCode) {
        this.volLevelCode = volLevelCode;
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