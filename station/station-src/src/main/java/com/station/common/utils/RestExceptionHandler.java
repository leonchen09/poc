package com.station.common.utils;

import java.io.IOException;

import org.apache.log4j.Logger;
import org.springframework.beans.ConversionNotSupportedException;
import org.springframework.beans.TypeMismatchException;
import org.springframework.http.converter.HttpMessageNotReadableException;
import org.springframework.http.converter.HttpMessageNotWritableException;
import org.springframework.web.HttpMediaTypeNotAcceptableException;
import org.springframework.web.HttpRequestMethodNotSupportedException;
import org.springframework.web.bind.MissingServletRequestParameterException;
import org.springframework.web.bind.annotation.ControllerAdvice;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.ResponseBody;

import com.station.common.Constant;
import com.station.moudles.vo.AjaxResponse;

/**
 * 异常增强，以JSON的形式返回给客服端
 * 异常增强类型：NullPointerException,RunTimeException,ClassCastException,
 * NoSuchMethodException,IOException,IndexOutOfBoundsException
 * 以及springmvc自定义异常等，如下： SpringMVC自定义异常对应的status code
 * 
 * Exception HTTP Status Code ConversionNotSupportedException 500 (Internal
 * Server Error) HttpMessageNotWritableException 500 (Internal Server Error)
 * HttpMediaTypeNotSupportedException 415 (Unsupported Media Type)
 * HttpMediaTypeNotAcceptableException 406 (Not Acceptable)
 * HttpRequestMethodNotSupportedException 405 (Method Not Allowed)
 * NoSuchRequestHandlingMethodException 404 (Not Found) TypeMismatchException
 * 400 (Bad Request) HttpMessageNotReadableException 400 (Bad Request)
 * MissingServletRequestParameterException 400 (Bad Request)
 *
 */
@ControllerAdvice
public class RestExceptionHandler {
	private static Logger logger = Logger.getLogger(RestExceptionHandler.class);

/*	// 运行时异常
	@ExceptionHandler(RuntimeException.class)
	@ResponseBody
	public AjaxResponse<Object> runtimeExceptionHandler(RuntimeException runtimeException) {
		logger.error(runtimeException.toString());
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>();
		ajaxResponse.setCode(Constant.RS_CODE_ERROR);
		ajaxResponse.setMsg("输入的参数错误或者后台运行错误，异常编号：1000");
		return ajaxResponse;
	}*/

	// 空指针异常
	@ExceptionHandler(NullPointerException.class)
	@ResponseBody
	public AjaxResponse<Object> nullPointerExceptionHandler(NullPointerException ex) {
		logger.error(ex);
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>();
		ajaxResponse.setCode(Constant.RS_CODE_ERROR);
		ajaxResponse.setMsg("空指针异常，异常编号：1001");
		return ajaxResponse;
	}

	// 类型转换异常
	@ExceptionHandler(ClassCastException.class)
	@ResponseBody
	public AjaxResponse<Object> classCastExceptionHandler(ClassCastException ex) {
		logger.error(ex);
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>();
		ajaxResponse.setCode(Constant.RS_CODE_ERROR);
		ajaxResponse.setMsg("类型转换异常，异常编号：1002");
		return ajaxResponse;
	}

	// IO异常
	@ExceptionHandler(IOException.class)
	@ResponseBody
	public AjaxResponse<Object> iOExceptionHandler(IOException ex) {
		logger.error(ex);
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>();
		ajaxResponse.setCode(Constant.RS_CODE_ERROR);
		ajaxResponse.setMsg("Io异常，异常编号：1003");
		return ajaxResponse;
	}

	// 未知方法异常
	@ExceptionHandler(NoSuchMethodException.class)
	@ResponseBody
	public AjaxResponse<Object> noSuchMethodExceptionHandler(NoSuchMethodException ex) {
		logger.error(ex);
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>();
		ajaxResponse.setCode(Constant.RS_CODE_ERROR);
		ajaxResponse.setMsg("未知方法错误，异常编号：1004");
		return ajaxResponse;
	}

	// 数组越界异常
	@ExceptionHandler(IndexOutOfBoundsException.class)
	@ResponseBody
	public AjaxResponse<Object> indexOutOfBoundsExceptionHandler(IndexOutOfBoundsException ex) {
		logger.error(ex);
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>();
		ajaxResponse.setCode(Constant.RS_CODE_ERROR);
		ajaxResponse.setMsg("数组越界错误，异常编号：1005");
		return ajaxResponse;
	}

	// 400错误
	@ExceptionHandler({ HttpMessageNotReadableException.class })
	@ResponseBody
	public AjaxResponse<Object> requestNotReadable(HttpMessageNotReadableException ex) {
		logger.error(ex);
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>();
		ajaxResponse.setCode(Constant.RS_CODE_ERROR);
		ajaxResponse.setMsg("输入的参数不符合要求，异常编号：400");
		return ajaxResponse;
	}

	// 400错误
	@ExceptionHandler({ TypeMismatchException.class })
	@ResponseBody
	public AjaxResponse<Object> requestTypeMismatch(TypeMismatchException ex) {
		logger.error(ex);
		// String retParam = ReturnFormat.retParam(400, null);
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>();
		ajaxResponse.setCode(Constant.RS_CODE_ERROR);
		ajaxResponse.setMsg("输入的参数类型不符合要求，异常编号：400");
		return ajaxResponse;
	}

	// 400错误
	@ExceptionHandler({ MissingServletRequestParameterException.class })
	@ResponseBody
	public AjaxResponse<Object> requestMissingServletRequest(MissingServletRequestParameterException ex) {
		logger.error(ex);
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>();
		ajaxResponse.setCode(Constant.RS_CODE_ERROR);
		ajaxResponse.setMsg("输入的参数不符合要求，异常编号：400");
		return ajaxResponse;
	}

	// 405错误
	@ExceptionHandler({ HttpRequestMethodNotSupportedException.class })
	@ResponseBody
	public AjaxResponse<Object> request405() {
		
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>();
		ajaxResponse.setCode(Constant.RS_CODE_ERROR);
		ajaxResponse.setMsg("请求的方法错误，异常编号：405");
		return ajaxResponse;
	}

	// 406错误
	@ExceptionHandler({ HttpMediaTypeNotAcceptableException.class })
	@ResponseBody
	public AjaxResponse<Object> request406() {
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>();
		ajaxResponse.setCode(Constant.RS_CODE_ERROR);
		ajaxResponse.setMsg(" Web 服务器检测发现它想反馈的数据不能为客户端所接受，异常编号：406");
		return ajaxResponse;
	}

	// 500错误
	@ExceptionHandler({ ConversionNotSupportedException.class, HttpMessageNotWritableException.class })
	@ResponseBody
	public AjaxResponse<Object> server500(RuntimeException runtimeException) {
		logger.error(runtimeException.toString());
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>();
		ajaxResponse.setCode(Constant.RS_CODE_ERROR);
		ajaxResponse.setMsg("程序运行错误，异常编号：500");
		return ajaxResponse;
	}
}