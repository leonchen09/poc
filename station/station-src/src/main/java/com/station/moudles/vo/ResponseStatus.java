package com.station.moudles.vo;

public class ResponseStatus {

    private Integer status;
    private String statusText;
    private String tookTime;
    private String message;
    private String extMessage;

    public Integer getStatus() {
        return status;
    }

    public void setStatus(Integer status) {
        this.status = status;
    }

    public String getStatusText() {
        return statusText;
    }

    public void setStatusText(String statusText) {
        this.statusText = statusText;
    }

    public String getTookTime() {
        return tookTime;
    }

    public void setTookTime(String tookTime) {
        this.tookTime = tookTime;
    }

    public String getMessage() {
        return message;
    }

    public void setMessage(String message) {
        this.message = message;
    }

    public String getExtMessage() {
        return extMessage;
    }

    public void setExtMessage(String extMessage) {
        this.extMessage = extMessage;
    }
}
