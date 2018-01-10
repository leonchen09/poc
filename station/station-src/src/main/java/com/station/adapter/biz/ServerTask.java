package com.station.adapter.biz;

import org.springframework.context.annotation.Scope;
import org.springframework.stereotype.Repository;

import com.station.adapter.io.AdapterServer;

@Repository
@Scope("prototype")
public class ServerTask implements Runnable{

	//消息体
	private byte[] msgData;
	//对应channel的id
	private String endPoint;

	public byte[] getMsgData() {
		return msgData;
	}

	public void setMsgData(byte[] msgData) {
		this.msgData = msgData;
	}

	public String getEndPoint() {
		return endPoint;
	}

	public void setEndPoint(String endPoint) {
		this.endPoint = endPoint;
	}

	@Override
	public void run() {
		parseData();
		
	}

	protected void parseData() {
		//解析协议。
		System.out.println("Server received:"+new String(msgData));
		AdapterServer.sendMsg(endPoint, "Server response".getBytes());
	}
}
