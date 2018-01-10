package com.reachcloud.demo;

import java.net.InetSocketAddress;
import java.util.concurrent.Executors;

import org.jboss.netty.bootstrap.ClientBootstrap;
import org.jboss.netty.channel.ChannelFactory;
import org.jboss.netty.channel.ChannelPipeline;
import org.jboss.netty.channel.ChannelPipelineFactory;
import org.jboss.netty.channel.Channels;
import org.jboss.netty.channel.socket.nio.NioClientSocketChannelFactory;

public class NettyClient {
	
	public static void main(String[] argv){
		ChannelFactory factory = new NioClientSocketChannelFactory(Executors.newCachedThreadPool(), 
				Executors.newCachedThreadPool());
		ClientBootstrap bootstrap = new ClientBootstrap(factory);
		bootstrap.setPipelineFactory(new ChannelPipelineFactory()
				{
					public ChannelPipeline getPipeline(){
						return Channels.pipeline(new ClientHandler());
					}
				});
		
		bootstrap.setOption("child.tcpNoDelay", true);
		
		bootstrap.connect(new InetSocketAddress("127.0.0.1",8000));
	}
}
