package com.reachcloud.framework.mq;

import java.io.IOException;
import java.util.concurrent.TimeoutException;

import com.rabbitmq.client.AMQP.BasicProperties;
import com.rabbitmq.client.Consumer;
import com.rabbitmq.client.Envelope;
import com.rabbitmq.client.QueueingConsumer;
import com.rabbitmq.client.ShutdownSignalException;

/**
 * 消息消费端。接受广播消息。
 * @author Administrator
 *
 */
public class MsgSubscriber extends RabbitMQClient implements Consumer{

	public MsgSubscriber(String exchangeName) throws IOException, TimeoutException {
		super(exchangeName);
	}
	
	/**
	 * 开始消费信息。
	 * @throws IOException
	 */
	public void start() throws IOException{
		//获得临时队列，消费端退出后，自动关闭。
		String queueName = channel.queueDeclare().getQueue();
        // binding
        channel.queueBind(queueName, exchangeName, "");
        //指定队列消费者
        channel.basicConsume(queueName, true, this);
	}

	@Override
	public void handleCancel(String arg0) throws IOException {
	}

	@Override
	public void handleCancelOk(String arg0) {
				
	}

	@Override
	public void handleConsumeOk(String consumerTag) {
		System.out.println("Consumer "+consumerTag +" registered");     
		
	}

	@Override
	public void handleDelivery(String consumerTag, Envelope arg1, BasicProperties arg2, byte[] arg3) throws IOException {
		System.out.println("client:" + consumerTag +", Message received:" + new String(arg3));		
	}

	@Override
	public void handleRecoverOk(String arg0) {
		
	}

	@Override
	public void handleShutdownSignal(String arg0, ShutdownSignalException arg1) {
		
	}

//	@Override
//	public void run() {
//		try {
//		      // 由RabbitMQ自行创建的临时队列,唯一且随消费者的中止而自动删除的队列
//	         String queueName = channel.queueDeclare().getQueue();
//	         // binding
//	         channel.queueBind(queueName, exchangeName, "");
//	         //指定队列消费者
//	         channel.basicConsume(queueName, true, this);
//		} catch (IOException e) {
//			e.printStackTrace();
//		}
//		
//	}

}
