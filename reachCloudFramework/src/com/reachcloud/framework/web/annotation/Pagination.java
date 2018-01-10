package com.reachcloud.framework.web.annotation;

import java.lang.annotation.Documented;
import java.lang.annotation.ElementType;  
import java.lang.annotation.Retention;  
import java.lang.annotation.RetentionPolicy;  
import java.lang.annotation.Target;

import com.reachcloud.framework.web.RequestDataType;
  
/**
 * 分页参数注解。使用在controller中的参数中，可以自动组装分页查询对象，作为controller的参数使用。
 * @author Chenwl
 * @date 2016年4月22日
 */
@Target(ElementType.PARAMETER)  
@Retention(RetentionPolicy.RUNTIME)  
@Documented
public @interface Pagination {
	
	public RequestDataType dataType() default RequestDataType.FORM;
}
