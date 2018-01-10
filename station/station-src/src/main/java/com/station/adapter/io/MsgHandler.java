package com.station.adapter.io;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.station.adapter.biz.WorkQuene;
import com.station.adapter.biz.WorkQuene.DataPack;

import io.netty.channel.Channel;
import io.netty.channel.ChannelHandlerContext;
import io.netty.channel.ChannelInboundHandlerAdapter;

public class MsgHandler extends ChannelInboundHandlerAdapter{ 
	
	private static final Logger logger = LoggerFactory.getLogger(MsgHandler.class);
	
	@Override
    public void channelRead(ChannelHandlerContext ctx, Object msg) {   
		String id = ctx.channel().id().asLongText();
    	saveChannel(id, ctx);

    	byte[] data = (byte[]) msg;
    	DataPack dataPackage = new DataPack(id, data);
    	WorkQuene.add(dataPackage);
    }
	
	@Override
    public void channelReadComplete(ChannelHandlerContext ctx) {   
//        ctx.writeAndFlush(Unpooled.EMPTY_BUFFER) //flush掉所有写回的数据  
//        .addListener(ChannelFutureListener.CLOSE); //当flush完成后关闭channel  
    }
    

	/**
	 * 
	 * @param ctx
	 * @return
	 */
	private boolean saveChannel(String id, ChannelHandlerContext ctx) {
		if(AdapterServer.channels.get(id) == null) {
			AdapterServer.channels.put(id, ctx.channel());
			return true;
		}
		return false;
	}
	
}
