package com.station.adapter.io;

import java.net.InetSocketAddress;
import java.util.concurrent.ConcurrentHashMap;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.station.adapter.biz.BusinessServer;

import io.netty.bootstrap.ServerBootstrap;
import io.netty.buffer.Unpooled;
import io.netty.channel.Channel;
import io.netty.channel.ChannelFuture;
import io.netty.channel.ChannelInitializer;
import io.netty.channel.EventLoopGroup;
import io.netty.channel.nio.NioEventLoopGroup;
import io.netty.channel.socket.SocketChannel;
import io.netty.channel.socket.nio.NioServerSocketChannel;
import io.netty.handler.codec.DelimiterBasedFrameDecoder;
import io.netty.handler.codec.LengthFieldBasedFrameDecoder;
import io.netty.handler.codec.bytes.ByteArrayDecoder;
import io.netty.handler.codec.bytes.ByteArrayEncoder;
import io.netty.handler.timeout.IdleStateHandler;


public class AdapterServer {

	//所有在线的channel
	public static final ConcurrentHashMap<String, Channel> channels = new ConcurrentHashMap<String, Channel>();
	
	private static final Logger logger = LoggerFactory.getLogger(AdapterServer.class);
	
	//当前server监听的端口号
	private int port;
	//空闲时间标准,单位秒，超过该时间，服务器将认为客户端掉线。
	private int allIdleTime;
	
	public AdapterServer(int port, int allIdleTime) {
		this.port = port;
		this.allIdleTime = allIdleTime;
	}
	
	public void start()  {
        ServerBootstrap b = new ServerBootstrap();// 引导辅助程序  
        EventLoopGroup group = new NioEventLoopGroup();// 通过nio方式来接收连接和处理连接  
        try {  
            b.group(group);  
            b.channel(NioServerSocketChannel.class);// 设置nio类型的channel  
            b.localAddress(new InetSocketAddress(port));// 设置监听端口  
            b.childHandler(new ChannelInitializer<SocketChannel>() {//有连接到达时会创建一个channel  
                        protected void initChannel(SocketChannel ch) throws Exception {  
                        	//最大数据帧1024字节，长度值在第三字节，就一位。
                        	ch.pipeline().addLast("frameDecoder",new LengthFieldBasedFrameDecoder(1024, 2, 1));
                        	ch.pipeline().addLast("byteEncoder", new ByteArrayEncoder());
                        	ch.pipeline().addLast("IdleChecker", new IdleStateHandler(allIdleTime, allIdleTime, allIdleTime));
                            // 业务逻辑handler  
                            ch.pipeline().addLast("myHandler", new MsgHandler());
                            //空闲及异常处理handler
                            ch.pipeline().addLast("IdleTrigger", new HeartbeatHandler());
                        }  
                    });  
            ChannelFuture f = b.bind().sync();// 配置完成，开始绑定server，通过调用sync同步方法阻塞直到绑定成功  
            f.channel().closeFuture().sync();// 应用程序会一直等待，直到channel关闭  
        } catch (Exception e) {  
            logger.error("Adapter server start failed, port:" + port, e);
        } finally {  
            try {
				group.shutdownGracefully().sync();
			} catch (InterruptedException e) {
				logger.error("Adapter server stop failed.", e);
			}//关闭EventLoopGroup，释放掉所有资源包括创建的线程  
        } 
	}
	
	/**
	 * 服务器推送消息。
	 * @param id
	 * @param data
	 */
	public static void sendMsg(String id, byte[] data) {
		Channel channel = channels.get(id);
		if(channel == null) {
			throw new RuntimeException("设备的连接已经断开，不能传送数据。");
		}
		channel.writeAndFlush(data);
	}
	
	
	public static void main(String[] argvs) {
		BusinessServer.getInstance().start();
		
		AdapterServer server = new AdapterServer(9008, 10);
		server.start();
	}
}
