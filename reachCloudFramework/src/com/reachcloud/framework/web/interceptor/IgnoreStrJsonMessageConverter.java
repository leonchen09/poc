package com.reachcloud.framework.web.interceptor;

import java.lang.reflect.Type;

import org.springframework.http.MediaType;

import com.alibaba.fastjson.support.spring.FastJsonHttpMessageConverter;
/**
 * 在对controller返回值进行json处理时（即有@ResponseBody注解的情况下），不对String进行json转换。
 * @author Chenwl
 * @date 2016年7月20日
 */
public class IgnoreStrJsonMessageConverter extends
		FastJsonHttpMessageConverter {

	public IgnoreStrJsonMessageConverter() {
		super();
	}

	/**
	 * 在返回值为string时，该messageconverter不处理，交给后面的converter处理。
	 */
	@Override
	public boolean canWrite(Type type, Class<?> contextClass, MediaType mediaType) {
		
		return String.class == contextClass? false :super.canWrite(type, contextClass, mediaType);
	}
}