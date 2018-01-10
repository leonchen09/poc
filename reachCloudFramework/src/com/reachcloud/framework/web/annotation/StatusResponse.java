package com.reachcloud.framework.web.annotation;

import java.lang.annotation.Documented;
import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

/**
 * ctroller返回对象后，在returnvaluehandler中给其加上状态。
 * @author Chenwl
 * @date 2016年5月5日
 */

@Target(ElementType.METHOD)
@Retention(RetentionPolicy.RUNTIME)
@Documented
public @interface StatusResponse {

}
