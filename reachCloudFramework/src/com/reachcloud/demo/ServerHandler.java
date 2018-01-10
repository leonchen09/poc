package com.reachcloud.demo;

import org.jboss.netty.buffer.ChannelBuffer;
import org.jboss.netty.buffer.ChannelBuffers;
import org.jboss.netty.channel.ChannelHandlerContext;
import org.jboss.netty.channel.ChannelStateEvent;
import org.jboss.netty.channel.MessageEvent;
import org.jboss.netty.channel.SimpleChannelHandler;

public class ServerHandler extends SimpleChannelHandler{
	
	@Override
	public void messageReceived(ChannelHandlerContext ctx, MessageEvent e){
		ChannelBuffer buf = (ChannelBuffer) e.getMessage();
		while(buf.readable())
			buf.readByte();
//			System.out.print((char)buf.readByte());
//		System.out.println();
		String msg = "server response1\r";
		ChannelBuffer buffer = ChannelBuffers.buffer(msg.length());
		buffer.writeBytes(msg.getBytes());
		e.getChannel().write(buffer);
	}
	
	@Override
	public void channelConnected(ChannelHandlerContext ctx, ChannelStateEvent e){
//		String msg = "server response2";
//		ChannelBuffer buffer = ChannelBuffers.buffer(msg.length());
//		buffer.writeBytes(msg.getBytes());
//		e.getChannel().write(buffer);
	}

}
