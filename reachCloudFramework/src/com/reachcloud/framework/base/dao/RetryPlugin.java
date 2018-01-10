package com.reachcloud.framework.base.dao;

import java.util.Properties;

import org.apache.ibatis.executor.Executor;
import org.apache.ibatis.mapping.MappedStatement;
import org.apache.ibatis.plugin.Interceptor;
import org.apache.ibatis.plugin.Intercepts;
import org.apache.ibatis.plugin.Invocation;
import org.apache.ibatis.plugin.Plugin;
import org.apache.ibatis.plugin.Signature;
import org.apache.ibatis.session.ResultHandler;
import org.apache.ibatis.session.RowBounds;
import org.springframework.beans.factory.annotation.Autowired;

import com.reachcloud.framework.datasource.MultiDataSource;
import com.reachcloud.framework.datasource.cluster.AvailableRDS;
//这个类是做什么的？与读写分离相关？
@Intercepts({@Signature(
		  type= Executor.class,
		  method = "query",
		  args = {MappedStatement.class,Object.class, RowBounds.class, ResultHandler.class})})
public class RetryPlugin implements Interceptor{
	
	@Autowired
	private AvailableRDS rds;
	//重试次数
	private int retryCount = 1;
	
	public int getRetryCount() {
		return retryCount;
	}
	
	public void setRetryCount(int retryCount) {
		this.retryCount = retryCount;
	}

	@Override
	public Object intercept(Invocation invocation) throws Throwable {
		Object result = null;
		try{
			result = invocation.proceed();
			return result;
		}catch(Exception ex){
			//修改datasoruce key，然后重试。
			String currentDSkey = MultiDataSource.getCurDSkey();
			if(rds.getDatasourceKeys().contains(currentDSkey)){//当前是read操作
				return retry(invocation, retryCount);
			}
			throw ex;
		}
	}
	
	private Object retry(Invocation invocation, int count) throws Throwable{
		Object result = null;
		count --;
		MultiDataSource.setDataSourceKey(rds.getNextDatasourceKey());
		try{
			result = invocation.proceed();
			return result;
		}catch(Exception ex){
			if(count > 0){
				return retry(invocation, count);
			}else{
				throw ex;
			}
		}
	}

	@Override
	public Object plugin(Object target) {
		return Plugin.wrap(target, this); 
	}

	@Override
	public void setProperties(Properties properties) {
	}

}
