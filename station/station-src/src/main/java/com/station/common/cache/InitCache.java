package com.station.common.cache;

import java.util.Date;
import java.util.HashMap;
import java.util.Map;

import javax.servlet.ServletContext;

import org.springframework.beans.factory.InitializingBean;
import org.springframework.web.context.ServletContextAware;

import com.station.common.utils.GetProperty;

public class InitCache implements InitializingBean, ServletContextAware {
	public static String applicationId;
	public static Map<Integer, Map<String, Integer>> allUserMenuMap = new HashMap<Integer, Map<String, Integer>>();
	public static Map<Integer, Map<String, Integer>> allUserOtherPermissionsMap = new HashMap<Integer, Map<String, Integer>>();

	public static boolean pulseDischargeProgressFlag=false;
	//public static boolean StationInfoFile = true;//电池组导入标志
	public static boolean stationProgressFlag=false;
	
	//上次检查上传数据的时间。
	public static Date lastPackDataCheckTime;
	
	// @Autowired
	// public RedisClientTemplate redisClient;
	@Override
	public void afterPropertiesSet() throws Exception {
	}

	@Override
	public void setServletContext(ServletContext paramServletContext) {
		// redisClient.set("zdmTest", "test1111");
		// System.out.println(redisClient.get("zdmTest"));
		// System.out.println(redisClient.get("zdmtest"));
		String path = GetProperty.GetConfPath(null);
		applicationId = GetProperty.readValue(path, "applicationId");
		paramServletContext.setAttribute("applicationId", applicationId);
		System.out.println("启动ServletContext");
	}
}
