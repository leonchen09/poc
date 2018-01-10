package com.station.adapter.io;

import org.apache.log4j.Logger;

import io.netty.buffer.ByteBuf;
import io.netty.channel.ChannelHandlerContext;
import io.netty.handler.codec.MessageToByteEncoder;

public class V5FrameEncoder extends MessageToByteEncoder<FrameMessage>{

	private final static Logger logger = Logger.getLogger(V5FrameEncoder.class);
	//数据包的长度
	private final byte dataLength;
	//报文头标识
	private final byte[] frameHeader;
	
	public V5FrameEncoder(byte dataLength, byte[] frameHeader) {
		this.dataLength = dataLength;
		this.frameHeader = frameHeader;
	}
	

	@Override
	protected void encode(ChannelHandlerContext ctx, FrameMessage msg, ByteBuf out) throws Exception {
		if(msg == null)
			return;
		byte length = (byte)msg.getData().length;
		if(length > dataLength) {
			logger.error(String.format("数据包长度{}超过规定值{}，数据抛弃。", length, dataLength));
		}
		//报文头
		out.writeBytes(frameHeader);
		//报文长度
		out.writeByte(length);
		//数据类型
		out.writeByte(msg.getDataType());
		//数据
		out.writeBytes(msg.getData());
		//bcc校验
		generateBcc(msg);
		out.writeByte(msg.getBcc());
	}
	
	private void generateBcc(FrameMessage msg) {
		byte[] bytes = msg.getData();
		byte left = bytes[0];
        for(int i = 1; i <= bytes.length - 1; i ++) {
        	left = (byte) (left ^ bytes[i]);// 从头到最后第二位的异或
        }
        msg.setBcc(left);
	}

}
