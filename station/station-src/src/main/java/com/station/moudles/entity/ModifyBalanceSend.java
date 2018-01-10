package com.station.moudles.entity;

import java.util.Date;

public class ModifyBalanceSend {
    private Integer id;

    private String gprsId;

    private Integer para1;

    private Integer para2;

    private Integer para3;

    private Integer para4;

    private Integer para5;

    private Integer para6;

    private Integer para7;

    private Integer sendDone;


    private Date sendTime;

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

    public Integer getPara1() {
        return para1;
    }

    public void setPara1(Integer para1) {
        this.para1 = para1;
    }

    public Integer getPara2() {
        return para2;
    }

    public void setPara2(Integer para2) {
        this.para2 = para2;
    }

    public Integer getPara3() {
        return para3;
    }

    public void setPara3(Integer para3) {
        this.para3 = para3;
    }

    public Integer getPara4() {
        return para4;
    }

    public void setPara4(Integer para4) {
        this.para4 = para4;
    }

    public Integer getPara5() {
        return para5;
    }

    public void setPara5(Integer para5) {
        this.para5 = para5;
    }

    public Integer getPara6() {
        return para6;
    }

    public void setPara6(Integer para6) {
        this.para6 = para6;
    }

    public Integer getPara7() {
        return para7;
    }

    public void setPara7(Integer para7) {
        this.para7 = para7;
    }

	public Integer getSendDone() {
		return sendDone;
	}

	public void setSendDone(Integer sendDone) {
		this.sendDone = sendDone;
	}

	public Date getSendTime() {
		return sendTime;
	}

	public void setSendTime(Date sendTime) {
		this.sendTime = sendTime;
	}

  
}