package com.station.common.utils;

import java.io.BufferedInputStream;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.util.Enumeration;
import java.util.Properties;

/*
 * 从路径中的属性文件中读取单个属性或全部属性及设置属性
 */
public class GetProperty {

	// 读取信息
	public static String readValue(String filePath, String key) {
		Properties props = new Properties();
		try {
			InputStream ips = new BufferedInputStream(new FileInputStream(filePath));
			props.load(ips);
			ips.close();
			String value = props.getProperty(key);
			return value;
		} catch (FileNotFoundException e) {
			e.printStackTrace();
			return null;
		} catch (IOException e) {
			e.printStackTrace();
			return null;
		}
	}

	// 读取全部信息
	@SuppressWarnings("unchecked")
	public static void readProperties(String filePath) {
		Properties props = new Properties();
		try {
			InputStream ips = new BufferedInputStream(new FileInputStream(filePath));
			props.load(ips);
			ips.close();
			Enumeration<String> eu = (Enumeration<String>) props.propertyNames();
			while (eu.hasMoreElements()) {
				String key = (String) eu.nextElement();
				String value = props.getProperty(key);
			}
		} catch (FileNotFoundException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

	// 取配置文件路径
	public static String GetConfPath(String fileName) {
		if (fileName == null) {
			fileName = "context";
		}
		String path = GetProperty.class.getClassLoader().getResource("").getPath();
		path = path + "config/" + fileName + ".properties";
		return path;
	}

}
