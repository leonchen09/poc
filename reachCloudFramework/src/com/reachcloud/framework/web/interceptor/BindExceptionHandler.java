package com.reachcloud.framework.web.interceptor;

import java.io.IOException;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.validation.BindException;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.support.DefaultHandlerExceptionResolver;

public class BindExceptionHandler extends DefaultHandlerExceptionResolver{
	
	public BindExceptionHandler(){
		setOrder(1);//make sure this handler can be called early than defaultExceptionHandler.
	}
	
	@Override
	protected ModelAndView handleBindException(BindException ex, HttpServletRequest request,
			HttpServletResponse response, Object handler) throws IOException {
		return null;//return null, indicate this exception will be processed in next handler.
	}
}
