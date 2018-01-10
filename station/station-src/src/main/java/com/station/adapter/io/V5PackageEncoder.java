package com.station.adapter.io;

import java.util.List;

import io.netty.channel.ChannelHandlerContext;
import io.netty.handler.codec.MessageToMessageEncoder;

/**
 * 将完整的消息byte[]进行拆包。
 *
 */
public class V5PackageEncoder extends  MessageToMessageEncoder<PackageMessage> {

	@Override
	protected void encode(ChannelHandlerContext ctx, PackageMessage msg, List<Object> out) throws Exception {
		//判断消息长度，如果需要拆包，在这处理，否则直接转换，交个下一个encoder处理。
		
	}

}
