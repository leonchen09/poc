package com.reachcloud.demo;

import java.net.InetSocketAddress;
import java.util.concurrent.Executors;

import org.jboss.netty.bootstrap.ServerBootstrap;
import org.jboss.netty.channel.ChannelFactory;
import org.jboss.netty.channel.ChannelPipeline;
import org.jboss.netty.channel.ChannelPipelineFactory;
import org.jboss.netty.channel.Channels;
import org.jboss.netty.channel.socket.nio.NioServerSocketChannelFactory;

public class NettyServer {

	public static void main(String[] argv){
		ChannelFactory factory = new NioServerSocketChannelFactory(Executors.newCachedThreadPool(), 
				Executors.newCachedThreadPool());
		ServerBootstrap bootstrap = new ServerBootstrap(factory);
		bootstrap.setPipelineFactory(new ChannelPipelineFactory()
				{
					public ChannelPipeline getPipeline(){
						return Channels.pipeline(new ServerHandler());
					}
				});
		
		bootstrap.setOption("child.tcpNoDelay", true);
		
		bootstrap.bind(new InetSocketAddress(8000));
	}
}
