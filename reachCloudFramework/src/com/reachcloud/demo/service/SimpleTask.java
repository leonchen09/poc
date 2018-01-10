package com.reachcloud.demo.service;

import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Component;

@Component
public class SimpleTask {

	@Scheduled(cron = "0/5 * * * * ?")  
	public void testTask(){
		System.out.println("task running.");
	}	
	
}
