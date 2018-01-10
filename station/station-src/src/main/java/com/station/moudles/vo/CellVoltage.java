package com.station.moudles.vo;

import java.math.BigDecimal;

public class CellVoltage {
    private Integer num;
    private BigDecimal voltage;
    private double capacity;
    
    public CellVoltage(Integer num, BigDecimal voltage) {
        this.num = num;
        this.voltage = voltage;
    }
    
    public CellVoltage(Integer num, double capacity) {
    	this.num = num;
    	this.capacity = capacity;
    }

    public Integer getNum() {
        return num;
    }

    public void setNum(Integer num) {
        this.num = num;
    }

    public BigDecimal getVoltage() {
        return voltage;
    }

    public void setVoltage(BigDecimal voltage) {
        this.voltage = voltage;
    }

	public double getCapacity() {
		return capacity;
	}

	public void setCapacity(double capacity) {
		this.capacity = capacity;
	}


    
    
}
