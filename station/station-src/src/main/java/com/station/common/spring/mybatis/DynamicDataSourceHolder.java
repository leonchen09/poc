package com.station.common.spring.mybatis;

import org.springframework.util.Assert;

public class DynamicDataSourceHolder {

	 private static final ThreadLocal<DataSourceType> contextHolder = new ThreadLocal<DataSourceType>(); 
	 
	 //设置数据源类型  
	 public static void setDataSourceType(DataSourceType dataSourceType) {  
	     Assert.notNull(dataSourceType, "DataSourceType cannot be null");  
	     contextHolder.set(dataSourceType);  
	 }  
	   
	 // 获取数据源类型  
	 public static DataSourceType getDataSourceType() { 
		 DataSourceType dataSource = contextHolder.get();
		 //System.out.println(dataSource+"##########################");
		 contextHolder.remove();
	     return dataSource; 
	 }  
	   
	 // 清除数据源类型  
	 public static void clearDataSourceType() {  
	     contextHolder.remove();  
	 }  
}
