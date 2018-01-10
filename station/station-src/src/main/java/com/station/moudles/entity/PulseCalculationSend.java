package com.station.moudles.entity;

import java.util.Date;

/**
 * Created by Jack on 9/9/2017.
 */
public class PulseCalculationSend {
    private Integer id;
    private String gprsId;
    private Integer sendDone;
    private String sendDoneMessage;
    private Integer resistanceStatus;
    private String resistanceStatusMessage;
    private Integer capacityStatus;
    private String capacityStatusMessage;
    private Date sendTime;
    private Date endTime;

    private String name;

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
        this.gprsId = gprsId;
    }

    public Integer getSendDone() {
        return sendDone;
    }

    public void setSendDone(Integer sendDone) {
        this.sendDone = sendDone;
    }

    public String getSendDoneMessage() {
        return sendDoneMessage;
    }

    public void setSendDoneMessage(String sendDoneMessage) {
        this.sendDoneMessage = sendDoneMessage;
    }

    public Integer getResistanceStatus() {
        return resistanceStatus;
    }

    public void setResistanceStatus(Integer resistanceStatus) {
        this.resistanceStatus = resistanceStatus;
    }

    public String getResistanceStatusMessage() {
        return resistanceStatusMessage;
    }

    public void setResistanceStatusMessage(String resistanceStatusMessage) {
        this.resistanceStatusMessage = resistanceStatusMessage;
    }

    public Integer getCapacityStatus() {
        return capacityStatus;
    }

    public void setCapacityStatus(Integer capacityStatus) {
        this.capacityStatus = capacityStatus;
    }

    public String getCapacityStatusMessage() {
        return capacityStatusMessage;
    }

    public void setCapacityStatusMessage(String capacityStatusMessage) {
        this.capacityStatusMessage = capacityStatusMessage;
    }

    public Date getSendTime() {
        return sendTime;
    }

    public void setSendTime(Date sendTime) {
        this.sendTime = sendTime;
    }

    public Date getEndTime() {
        return endTime;
    }

    public void setEndTime(Date endTime) {
        this.endTime = endTime;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }
}
