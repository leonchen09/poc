package com.station.adapter.biz;

import java.util.concurrent.ConcurrentHashMap;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.context.support.GenericXmlApplicationContext;
import org.springframework.scheduling.concurrent.ThreadPoolTaskExecutor;

import com.station.adapter.biz.WorkQuene.DataPack;

public class BusinessServer {
	
	private static final Logger logger = LoggerFactory.getLogger(BusinessServer.class);

	//单例
	private static final BusinessServer instance = new BusinessServer();
	
	//在线的设备id与channel id关联。
	public static final ConcurrentHashMap<String, String> onlinedGprs = new ConcurrentHashMap<String, String>();
	
	//spring 容器context
	private GenericXmlApplicationContext context;
	
	//线程池
	private ThreadPoolTaskExecutor taskExecutor;

	private BusinessServer() {
		initServer();
	}
	
	private void initServer() {
		context = new GenericXmlApplicationContext();
		context.setValidating(false);
		context.load("classpath:/config/applicationContextAdapter.xml");
		context.refresh();
		taskExecutor = context.getBean("taskExecutor", ThreadPoolTaskExecutor.class);
	}

	public static BusinessServer getInstance() {
		return instance;
	}
	/**
	 * 开启新线程，读取quene中的数据并分配给task executor.
	 */
	public void start() {
		//启动新线程，读取quene中的数据，分配给线程池执行。
		new Thread( () -> {
			while(true) {
				try {
					DataPack dataPackage = WorkQuene.take();
					ServerTask task = context.getBean(ServerTask.class);
					task.setMsgData(dataPackage.getData());
					task.setEndPoint(dataPackage.getId());
					taskExecutor.execute(task);
				} catch (Exception e) {
					logger.error("Business Server execute faild.",e);
				}
			}
		}).start();
		
	}
	
}
