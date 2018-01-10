package com.station.adapter.biz;

public class MessageData {

	//设备传上来的数据
	private byte[] data;
	//帧头，固定为7FF7
	private byte[] header;
	//数据帧长度
	private byte len;
	//数据帧类型,如keepalive，设置参数，脉冲放电等。
	private byte type;
	//基站主机地址
	private byte[] addr;
	//校验域
	private byte bcc;
	//当前设备协议的版本号。整数和端口对应。
	private int version;
	
	public byte[] getData() {
		return data;
	}
	public void setData(byte[] data) {
		this.data = data;
	}
	public byte[] getHeader() {
		return header;
	}
	public void setHeader(byte[] header) {
		this.header = header;
	}
	public byte getLen() {
		return len;
	}
	public void setLen(byte len) {
		this.len = len;
	}
	public byte getType() {
		return type;
	}
	public void setType(byte type) {
		this.type = type;
	}
	public byte[] getAddr() {
		return addr;
	}
	public void setAddr(byte[] addr) {
		this.addr = addr;
	}
	public byte getBcc() {
		return bcc;
	}
	public void setBcc(byte bcc) {
		this.bcc = bcc;
	}
	public int getVersion() {
		return version;
	}
	public void setVersion(int version) {
		this.version = version;
	}
		
}
