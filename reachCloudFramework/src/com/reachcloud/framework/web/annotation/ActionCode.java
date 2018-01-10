package com.reachcloud.framework.web.annotation;

import java.lang.annotation.Documented;
import java.lang.annotation.Retention;
import java.lang.annotation.Target;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.ElementType;

/**
 * 权限代码注解，用在controller的方法、类上，标注每个方法(或类)（实际对应url）的代码。
 * @author Chenwl
 * @date 2016年4月22日
 */
@Target({ElementType.METHOD, ElementType.TYPE})
@Retention(RetentionPolicy.RUNTIME)
@Documented
public @interface ActionCode {
	
	public String value() default "";
}
