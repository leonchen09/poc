package com.reachcloud.framework.mq;

import java.io.IOException;
import java.util.concurrent.TimeoutException;
/**
 * 消息发布端。
 * @author Administrator
 *
 */
public class MsgPublisher extends RabbitMQClient {

	public MsgPublisher(String exchangeName) throws IOException, TimeoutException {
		super(exchangeName);
	}

    public void sendMessage(String message) throws IOException {
        channel.basicPublish(exchangeName, "", null, message.getBytes());
    }
    

}
