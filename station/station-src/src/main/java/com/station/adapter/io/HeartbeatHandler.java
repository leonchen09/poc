package com.station.adapter.io;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import io.netty.buffer.ByteBuf;
import io.netty.buffer.Unpooled;
import io.netty.channel.Channel;
import io.netty.channel.ChannelFutureListener;
import io.netty.channel.ChannelHandlerContext;
import io.netty.channel.ChannelInboundHandlerAdapter;
import io.netty.handler.timeout.IdleStateEvent;
import io.netty.util.CharsetUtil;

/**
 * 处理链接空闲，断开等
 * @author admin
 *
 */
public class HeartbeatHandler extends ChannelInboundHandlerAdapter {
	
	private static final Logger logger = LoggerFactory.getLogger(HeartbeatHandler.class);
	
	private static final ByteBuf HEARTBEAT_SEQUENCE = Unpooled.unreleasableBuffer(Unpooled.copiedBuffer("Heartbeat", CharsetUtil.UTF_8));

    @Override
    public void userEventTriggered(ChannelHandlerContext ctx, Object evt) throws Exception {
        // IdleStateHandler 所产生的 IdleStateEvent 的处理逻辑.
        if (evt instanceof IdleStateEvent) {
            IdleStateEvent e = (IdleStateEvent) evt;
            switch (e.state()) {
                case READER_IDLE:
                    handleReaderIdle(ctx);
                    break;
                case WRITER_IDLE:
                    handleWriterIdle(ctx);
                    break;
                case ALL_IDLE:
                    handleAllIdle(ctx);
                    break;
                default:
                    break;
            }
        }
    }

    @Override
    public void channelActive(ChannelHandlerContext ctx) throws Exception {
    	super.channelActive(ctx);
    	logger.debug("New socket was accepted." + ctx.channel().id().asLongText());
    }

    /**
     * channel关闭时，清除server中保存的channel。
     */
    @Override
    public void channelInactive(ChannelHandlerContext ctx) throws Exception {
    	super.channelInactive(ctx);
    	removeChannel(ctx);
    }

    @Override
    public void exceptionCaught(ChannelHandlerContext ctx,Throwable cause) {   
        logger.error(cause.getMessage()+", channel id:"+ctx.channel().id().asLongText());  
        ctx.close();//出现异常时关闭channel   
    } 
    
    protected void handleReaderIdle(ChannelHandlerContext ctx) {
        //do nothing.
    }

    protected void handleWriterIdle(ChannelHandlerContext ctx) {
    	//do nothing.
    }

    /**
     * 空闲超时，关闭channel。
     * @param ctx
     */
    protected void handleAllIdle(ChannelHandlerContext ctx) {
//        ctx.writeAndFlush(HEARTBEAT_SEQUENCE.duplicate()).addListener(ChannelFutureListener.CLOSE_ON_FAILURE);
    	logger.debug("Channel is idled, close it." + ctx.channel().id().asLongText());
    	ctx.close();
    }

	private boolean removeChannel(ChannelHandlerContext ctx) {
		String id = ctx.channel().id().asLongText();
		Channel channel = AdapterServer.channels.remove(id);
		if(channel != null && channel != ctx.channel()) {
			logger.error("channel关闭时，id一致，但是对象不一致。");
			return false;
		}
		return true;
	}
}