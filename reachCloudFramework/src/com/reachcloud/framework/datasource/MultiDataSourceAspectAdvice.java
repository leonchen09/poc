package com.reachcloud.framework.datasource;

import java.lang.annotation.Annotation;
import java.lang.reflect.Method;

import org.aspectj.lang.JoinPoint;
import org.aspectj.lang.Signature;
import org.aspectj.lang.reflect.MethodSignature;
import org.springframework.beans.factory.annotation.Autowired;

import com.reachcloud.framework.datasource.cluster.AvailableRDS;
import com.reachcloud.framework.web.annotation.QueryOnly;
/**
 * 读写分离时，切换datasource的切片程序。
 * 
 * @author chenwl
 *
 */
public class MultiDataSourceAspectAdvice {
	
	@Autowired
	private AvailableRDS rds;
	
    public void doBefore(JoinPoint jp) throws Throwable {
    	Signature sig = jp.getSignature();
        MethodSignature msig = (MethodSignature) sig;
        Object target = jp.getTarget();
        Method currentMethod = target.getClass().getMethod(msig.getName(), msig.getParameterTypes());
        
        QueryOnly queryOnly = currentMethod.getAnnotation(QueryOnly.class);

//    	String signature = jp.getSignature().toString();
//        if(signature.matches("^\\S* com.*\\.query\\..+$")){//方法签名中的package包括".query.",意味着只读
//            MultiDataSource.setDataSourceKey("readDataSource");
        if(queryOnly != null){
        	MultiDataSource.setDataSourceKey(rds.getNextDatasourceKey());
        } else {
        	MultiDataSource.setDataSourceKey("writeDataSource");
        }
    }
    
    public static void main(String[] args) {
		String value = "java.query.n com.sofn.demo.dao.ReadDemoMapper.getcount(int)";
		System.out.println(value.matches("^\\S* com.*\\.query\\..+$"));
	}
    
}
