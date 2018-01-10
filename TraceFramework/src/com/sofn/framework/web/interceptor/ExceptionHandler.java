package com.sofn.framework.web.interceptor;

import java.io.PrintWriter;
import java.net.BindException;
import java.util.HashMap;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;




import org.apache.log4j.Logger;
import org.springframework.web.bind.annotation.ResponseBody;
import org.springframework.web.method.HandlerMethod;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.handler.AbstractHandlerExceptionResolver;

import com.sofn.framework.exception.BizException;
import com.sofn.framework.exception.SysException;
import com.sofn.framework.exception.ValidationException;
import com.sofn.framework.util.JsonUtil;
import com.sofn.framework.util.XStreamUtil;
import com.sofn.framework.web.annotation.StatusResponse;
import com.sofn.framework.web.response.ResponseResult;
import com.sofn.framework.web.response.ResultCode;

/**
 * 统一的exception处理，根据异常类型，返回不同的页面。
 * 对bizException，根据返回的类型，要么返回json，要么返回xml，要么给出统一的exception页面。
 * 
 * @author Chenwl
 * @date 2016年4月28日
 */
public class ExceptionHandler extends AbstractHandlerExceptionResolver {

	private static Logger logger = Logger.getLogger(ExceptionHandler.class);

	// 统一的bizException页面，除了json和xml格式。
	private String bizExceptionView;
	// 统一的sysException页面
	private String sysExceptionView;
	// 统一的其他exception页面
	private String exceptionView;

	// 统一的其他validationException页面，除了json和xml格式。
	private String validationExceptionView;

		
	

	public ExceptionHandler() {
		setOrder(1);
	}

	/**
	 * 通过PrintWriter写入返回内容
	 * @param response
	 * @param writerStr
	 * @return
	 */
	private ModelAndView writer(HttpServletResponse response,String writerStr){
		
		try {
			PrintWriter writer = response.getWriter();
			writer.write(writerStr);
			writer.flush();
			return new ModelAndView();// null ModelAndView indicate the response
										// processed already.
		} catch (Exception e) {
			logger.error(e);
		}
		return null;
	}
	
	
	/**
	 * 将需要返回的信息转换成json返回
	 * 
	 * @param response
	 * @param resultCode
	 * @param resultMsg
	 * @return
	 */
	private  ModelAndView toJson(HttpServletResponse response,
			ResultCode resultCode, Object resultMsg) {
		ResponseResult result = new ResponseResult(resultCode.getCode(), resultMsg);
		String json = JsonUtil.toJsonStr(result);

		return 	writer(response,json);
	}

	/**
	 * 将需要返回的信息转换成xml返回
	 * 
	 * @param response
	 * @param resultCode
	 * @param resultMsg
	 * @return
	 */
	private  ModelAndView toXml(HttpServletResponse response,
			ResultCode resultCode, Object resultMsg) {
			ResponseResult result = new ResponseResult(resultCode.getCode(), resultMsg);
			String xml = XStreamUtil.toXml(result);
			return 	writer(response,xml);
	}

	/**
	 * 对异常结果进行相应的格式化处理
	 * 
	 * @param view
	 * @param resultCode
	 * @param model
	 * @param method
	 * @param response
	 * @param resultMsg
	 * @return
	 */
	private  ModelAndView doException(String view,
			ResultCode resultCode, Map<String, Object> model,
			HandlerMethod method,HttpServletRequest request, HttpServletResponse response, Object resultMsg) {

		StatusResponse statusResponse = method.getMethodAnnotation(StatusResponse.class);

		if (statusResponse == null) {// 非json/xml格式
			return new ModelAndView(view, model);
		} else {
			// TODO,根据Accept来判断是返回xml还是json。
			
			String accept = request.getHeader("Accept");
			if(accept==null){ return new ModelAndView(view, model); }
			String [] accepts=accept.split(",");
			String contentType=accepts[0].trim();
			
			if (contentType.equals("application/json")) {
				// dojson
				return toJson(response, resultCode, resultMsg);
			}
			if (contentType.equals("application/xml")||contentType.equals("text/xml")) {
				// doxml
				return toXml(response, resultCode, resultMsg);
			}
			return new ModelAndView(view, model);
		}
	}

	/**
	 * 处理BizException
	 * @param view
	 * @param model
	 * @param method
	 * @param response
	 * @param resultMsg
	 * @return
	 */
	private  ModelAndView doBizException(String view,
			Map<String, Object> model, HandlerMethod method,
			HttpServletRequest request,HttpServletResponse response, Object resultMsg) {
		
		return doException(view, ResultCode.BIZEXCEPTION, model,
				method,request, response, resultMsg);

	}
	/**
	 * 处理ValidatException
	 * @param view
	 * @param model
	 * @param method
	 * @param response
	 * @param resultMsg
	 * @return
	 */
	private  ModelAndView doValidatException(String view,
			Map<String, Object> model, HandlerMethod method,
			HttpServletRequest request,HttpServletResponse response, Object resultMsg) {
		
		return doException(view, ResultCode.VALIDATIONEXCEPTION, model,
				method,request, response, resultMsg);

	}
	
	@Override
	protected ModelAndView doResolveException(HttpServletRequest request,
			HttpServletResponse response, Object handler, Exception ex) {
		Map<String, Object> model = new HashMap<String, Object>();
		model.put("ex", ex);
		if (handler instanceof HandlerMethod) {
			HandlerMethod method = (HandlerMethod) handler;
			if (ex instanceof SysException) {
				return new ModelAndView(sysExceptionView, model);
			} 
			if (ex instanceof BizException) {

				return doBizException(bizExceptionView, model, method,
						request,response, ex.getMessage());

			} 
			if (ex instanceof ValidationException) {
				return doValidatException(validationExceptionView, model, method,
						request,response, ((ValidationException) ex).getList());
			} 
			if (ex instanceof BindException) {
				
				return  doValidatException(validationExceptionView,model, method,
						request,response, ex.getMessage());
				
			} 
				
			return new ModelAndView(exceptionView, model);
		}
		return null;
	}

	public String getBizExceptionView() {
		return bizExceptionView;
	}

	public void setBizExceptionView(String bizExceptionView) {
		this.bizExceptionView = bizExceptionView;
	}

	public String getSysExceptionView() {
		return sysExceptionView;
	}

	public void setSysExceptionView(String sysExceptionView) {
		this.sysExceptionView = sysExceptionView;
	}

	public String getExceptionView() {
		return exceptionView;
	}

	public void setExceptionView(String exceptionView) {
		this.exceptionView = exceptionView;
	}

	public String getValidationExceptionView() {
		return validationExceptionView;
	}

	public void setValidationExceptionView(String validationExceptionView) {
		this.validationExceptionView = validationExceptionView;
	}
	
}
