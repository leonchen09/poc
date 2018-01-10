package com.sofn.framework.web.interceptor;

import java.io.PrintWriter;

import javax.annotation.Resource;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import net.sf.json.JSONObject;

import org.apache.log4j.Logger;
import org.springframework.web.method.HandlerMethod;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.handler.HandlerInterceptorAdapter;

import com.sofn.framework.exception.BizException;
import com.sofn.framework.security.IAccessValidator;
import com.sofn.framework.web.annotation.ActionCode;

/**
 * controller权限注解，标注某个方法（对应url）能否的权限代码。
 * @author Chenwl
 * @date 2016年4月22日
 */
public class AccessInterceptor extends HandlerInterceptorAdapter{

	private static Logger logger = Logger.getLogger(AccessInterceptor.class);
	
	@Resource
	IAccessValidator accessValidator;
	
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
			
			
			if(actionCode == null || actionCode.value() == ""){//无注解，或者代码为空，直接跳过。
				return true;
			}
			
			boolean allow = accessValidator.validateActionCode(request, actionCode.value());
			if(!allow){
				logger.error("Illegal access");
				throw new BizException("Illegal access.");
			}
		}
		return true;
	}
	
	@Override
	public void postHandle(
			HttpServletRequest request, HttpServletResponse response, Object handler, ModelAndView modelAndView)
			throws Exception {
//		if(handler instanceof HandlerMethod){
//			HandlerMethod method = (HandlerMethod)handler;
//			ResponseJson responseJson = method.getMethodAnnotation(ResponseJson.class);
//			if(responseJson == null){
//				return;
//			}
//			Object data = modelAndView.getModel().get(modelAndView.getModel().keySet().iterator().next());
//			JsonResult result = new JsonResult();
//			result.setResult(data);
//			result.setResultCode(ResultCode.SUCCESS);
//			
//			try {
//				PrintWriter writer = response.getWriter();
//				String json = JsonUtil.toJsonStr(result);
//				writer.write(json);
//				writer.flush();
//				
//				return;
//			} catch (Exception e) {
//				logger.error(e);
//			}
//		}
//		Object data = modelAndView.getModel().get(modelAndView.getModel().keySet().iterator().next());
//		System.out.println("json result: " + JsonUtil.toJsonStr(data));
//		System.out.println(modelAndView.getModelMap());
	}

}
