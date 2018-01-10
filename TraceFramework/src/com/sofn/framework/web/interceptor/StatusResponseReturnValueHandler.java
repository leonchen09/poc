package com.sofn.framework.web.interceptor;

import java.io.IOException;
import java.util.List;

import org.springframework.core.MethodParameter;
import org.springframework.core.annotation.AnnotationUtils;
import org.springframework.http.converter.HttpMessageConverter;
import org.springframework.http.converter.HttpMessageNotWritableException;
import org.springframework.web.HttpMediaTypeNotAcceptableException;
import org.springframework.web.bind.annotation.ResponseBody;
import org.springframework.web.context.request.NativeWebRequest;
import org.springframework.web.method.support.ModelAndViewContainer;
import org.springframework.web.servlet.mvc.method.annotation.RequestResponseBodyMethodProcessor;

import com.sofn.framework.web.annotation.StatusResponse;
import com.sofn.framework.web.response.ResponseResult;
import com.sofn.framework.web.response.ResultCode;
/**
 * 处理controller方法返回的结果，加上"SUCCESS"状态。
 * @author Chenwl
 * @date 2016年5月5日
 */
public class StatusResponseReturnValueHandler extends RequestResponseBodyMethodProcessor{

	public StatusResponseReturnValueHandler(
			List<HttpMessageConverter<?>> converters) {
		super(converters);
	}

	@Override
	public boolean supportsReturnType(MethodParameter returnType) {
		return (AnnotationUtils.findAnnotation(returnType.getContainingClass(), StatusResponse.class) != null ||
				returnType.getMethodAnnotation(StatusResponse.class) != null);
	}
	
	@Override
	public void handleReturnValue(Object returnValue, MethodParameter returnType,
			ModelAndViewContainer mavContainer, NativeWebRequest webRequest)
			throws IOException, HttpMediaTypeNotAcceptableException, HttpMessageNotWritableException {
		ResponseResult result = new ResponseResult();
		result.setResultCode(ResultCode.SUCCESS.getCode());
		result.setResult(returnValue);
		
		super.handleReturnValue(result, returnType, mavContainer, webRequest);
	}
}
