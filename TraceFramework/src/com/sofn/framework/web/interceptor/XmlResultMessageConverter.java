package com.sofn.framework.web.interceptor;

import java.io.IOException;
import java.lang.reflect.Type;

import org.springframework.http.HttpOutputMessage;
import org.springframework.http.converter.HttpMessageNotWritableException;
import org.springframework.http.converter.xml.MappingJackson2XmlHttpMessageConverter;

import com.fasterxml.jackson.core.JsonGenerator;
import com.sofn.framework.web.response.ResponseResult;
import com.sofn.framework.web.response.ResultCode;
/**
 *将返回的xml对象加上状态。
 * @author Chenwl
 * @date 2016年5月3日
 */
public class XmlResultMessageConverter extends MappingJackson2XmlHttpMessageConverter{

	@Override
	protected void writePrefix(JsonGenerator generator, Object object) throws IOException {
//		generator.writeStartObject();
//		generator.writeObjectField("resultCode", "SUCCESS");
//		ResponseResult result = new ResponseResult();
//		result.setResultCode(ResultCode.SUCCESS);
//		generator.writeObject(result);
//		generator.writeObjectFieldStart("result");
//		generator.writeRaw("<resultCode>SUCCESS</resultCode><result>");
	}

	@Override
	protected void writeSuffix(JsonGenerator generator, Object object) throws IOException {
//		generator.writeRaw("</result>");
		
//		generator.writeEndObject();
	}
	@Override
	protected void writeInternal(Object object, Type type, HttpOutputMessage outputMessage) throws HttpMessageNotWritableException, IOException{
		//wrap the data with status code.
		ResponseResult result = new ResponseResult();
		result.setResultCode(ResultCode.SUCCESS.getCode());
		result.setResult(object);
		
		//output the full result
		super.writeInternal(result, type, outputMessage);
	}
}
