package com.reachcloud.framework.mq;

import java.io.IOException;
import java.util.concurrent.TimeoutException;

import com.rabbitmq.client.Channel;
import com.rabbitmq.client.Connection;
import com.rabbitmq.client.ConnectionFactory;
import com.rabbitmq.client.QueueingConsumer;
/**
 * 客户端基类，统一处理连接
 * @author Administrator
 *
 */
public abstract class RabbitMQClient {
	protected Channel channel;
    protected Connection connection;
    protected String exchangeName;
     
    public RabbitMQClient(String exchangeName) throws IOException, TimeoutException{
         this.exchangeName = exchangeName;
         
         //Create a connection factory
         ConnectionFactory factory = new ConnectionFactory();
         
         //hostname of your rabbitmq server
         factory.setHost("localhost");
         
         factory.setPort(5672);
         
         //getting a connection
         connection = factory.newConnection();
         
         //creating a channel
         channel = connection.createChannel();
         
         channel.exchangeDeclare(exchangeName, "fanout");
         
    }
     
     
    /**
     * 关闭channel和connection。
     * @throws IOException
     * @throws TimeoutException 
     */
     public void close() throws IOException, TimeoutException{
         this.channel.close();
         this.connection.close();
     }

}
