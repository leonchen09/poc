package com.station.adapter.io;

public class FrameMessage {
	
	private byte dataType;
	
	private byte[] data;
	
	private byte Bcc;

	public byte getDataType() {
		return dataType;
	}

	public void setDataType(byte dataType) {
		this.dataType = dataType;
	}

	public byte[] getData() {
		return data;
	}

	public void setData(byte[] data) {
		this.data = data;
	}

	public byte getBcc() {
		return Bcc;
	}

	public void setBcc(byte bcc) {
		Bcc = bcc;
	}
	
	
}
