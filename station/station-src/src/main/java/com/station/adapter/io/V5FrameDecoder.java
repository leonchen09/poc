package com.station.adapter.io;

import java.util.List;

import io.netty.buffer.ByteBuf;
import io.netty.channel.ChannelHandlerContext;
import io.netty.handler.codec.ByteToMessageDecoder;

/**
 * 按应用层协议对tcp包进行分包、合包操作。
 *
 */
public class V5FrameDecoder extends ByteToMessageDecoder{
	
//	private final int

	@Override
	protected void decode(ChannelHandlerContext ctx, ByteBuf in, List<Object> out) throws Exception {
		// TODO Auto-generated method stub
		
	}

}
