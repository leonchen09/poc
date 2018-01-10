package com.station.common.listener;

import java.util.List;

import javax.servlet.ServletContext;
import javax.servlet.ServletContextEvent;
import javax.servlet.ServletContextListener;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.context.ApplicationContext;
import org.springframework.web.context.support.WebApplicationContextUtils;

import com.station.moudles.entity.User;
import com.station.moudles.service.UserService;

public class MyContextListener implements ServletContextListener {
	private static final Logger logger = LoggerFactory.getLogger(MyContextListener.class);

	@Override
	public void contextInitialized(ServletContextEvent sce) {
		logger.debug("MyContextListener contextInitialized");
		ServletContext context = sce.getServletContext();
		ApplicationContext ctx = WebApplicationContextUtils.getWebApplicationContext(context);
		UserService userSer = (UserService) ctx.getBean("userServiceImpl");
		List<User> userList = userSer.selectListSelective(null);
		System.out.println("contextInitialized userList size=" + userList.size());
	}

	@Override
	public void contextDestroyed(ServletContextEvent sce) {
		logger.debug("MyContextListener contextDestroyed");
		ServletContext context = sce.getServletContext();
		ApplicationContext ctx = WebApplicationContextUtils.getWebApplicationContext(context);
		UserService userSer = (UserService) ctx.getBean(UserService.class);
		List<User> userList = userSer.selectListSelective(null);
		System.out.println("contextDestroyed userList size=" + userList.size());
	}
}
