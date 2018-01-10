package com.reachcloud.framework.mq;

import java.io.IOException;
import java.util.concurrent.TimeoutException;

public class Test {
	public static void main(String[] argv) throws Exception, TimeoutException{
//		for(int i = 0; i < 1; i ++)
//		{
			MsgSubscriber consumer = new MsgSubscriber("queue");
			consumer.start();
//		}
         

		
        MsgPublisher producer = new MsgPublisher("queue");
         
        for (int i = 0; i < 10; i++) {
            String message = "message " + i;
            producer.sendMessage(message);
            System.out.println("Message Number "+ i +" sent.");
            if(i == 5)
            {
                MsgSubscriber consumer2 = new MsgSubscriber("queue");
                consumer2.start();
            }
        }
        
        consumer.close();
//        consumer2.close();

	}
}
