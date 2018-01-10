package com.reachcloud.framework.web.interceptor;

import java.io.PrintWriter;

import javax.annotation.Resource;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import net.sf.json.JSONObject;

import org.apache.log4j.Logger;
import org.springframework.web.method.HandlerMethod;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.handler.HandlerInterceptorAdapter;

import com.reachcloud.framework.exception.BizException;
import com.reachcloud.framework.security.IAccessValidator;
import com.reachcloud.framework.web.annotation.ActionCode;

/**
 * controller权限注解，标注某个方法（对应url）能否的权限代码。
 * @author Chenwl
 * @date 2016年4月22日
 */
public class AccessInterceptor extends HandlerInterceptorAdapter{

	private static Logger logger = Logger.getLogger(AccessInterceptor.class);
	
	@Resource
	IAccessValidator accessValidator;
	// 配置前置拦截：
	//其是在调用控制层（controller）的方法之前执行，
	// 返回值为false，代表拦截， 返回值为true，代表放行，进入到下一个拦截器。。最后到controller层的方法
	@Override
	public boolean preHandle(HttpServletRequest request,
			HttpServletResponse response, Object handler) throws Exception {
		
		if(handler instanceof HandlerMethod){
			HandlerMethod method = (HandlerMethod)handler;
			
			//获取方法上的ActionCode注解
			ActionCode actionCode = method.getMethodAnnotation(ActionCode.class);
			if(actionCode==null||actionCode.value()==""){
				//获取类上的ActionCode注解
				actionCode=method.getBean().getClass().getAnnotation(ActionCode.class);
			}
			
			//无注解，或者代码为空，直接跳过，不进行拦截。
			if(actionCode == null || actionCode.value() == ""){
				return true;
			}
			
			boolean allow = accessValidator.validateActionCode(request, actionCode.value());
			System.out.println(allow);
			if(!allow){
				logger.error("Illegal access");
				throw new BizException("Illegal access.");
			}
		}
		return true;
	}
	
	// 配置拦截器处理方法
	//其是在调用控制层（controller）的方法之后，在跳转至视图之前执行
	@Override
	public void postHandle(
			HttpServletRequest request, HttpServletResponse response, Object handler, ModelAndView modelAndView)
			throws Exception {
	}

}
