package com.reachcloud.framework.datasource;

import org.springframework.jdbc.datasource.lookup.AbstractRoutingDataSource;
/**
 * 多数据源支持。用在读写分离模式下。
 * @author Chenwl
 * @date 2016年6月15日
 */
public class MultiDataSource extends AbstractRoutingDataSource {
	//当前线程所使用的datasource。
    private static final ThreadLocal<String> dataSourceKey = new InheritableThreadLocal<String>();

    public static void setDataSourceKey(String dataSource) {
        dataSourceKey.set(dataSource);
    }
    
    @Override
    protected Object determineCurrentLookupKey() {
        return dataSourceKey.get();
    }

    public static String getCurDSkey(){
    	return dataSourceKey.get();
    }

}
