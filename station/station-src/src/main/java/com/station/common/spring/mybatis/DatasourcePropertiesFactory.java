package com.station.common.spring.mybatis;

import java.util.Properties;

import com.station.common.utils.ThreeDES;

public class DatasourcePropertiesFactory {

	private static final String PROP_PASSWORD = "password";
	
	public static Properties getProperties(String password) {
		Properties properties = new Properties();
		properties.setProperty(PROP_PASSWORD, ThreeDES.decrypt(password));
		return properties;
	}
}
