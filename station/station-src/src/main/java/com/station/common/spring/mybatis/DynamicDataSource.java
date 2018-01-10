package com.station.common.spring.mybatis;

import org.springframework.jdbc.datasource.lookup.AbstractRoutingDataSource;



public class DynamicDataSource extends AbstractRoutingDataSource {
	
	@Override
	protected Object determineCurrentLookupKey() {
		DataSourceType type = DynamicDataSourceHolder.getDataSourceType();
		if(type != null) {
			return type.toString();
		}
		return null;
	}

}
