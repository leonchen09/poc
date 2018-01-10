package com.sofn.framework.web.interceptor;

import java.io.IOException;

import org.springframework.http.converter.json.MappingJackson2HttpMessageConverter;
import org.springframework.http.converter.json.MappingJacksonValue;

import com.fasterxml.jackson.core.JsonGenerator;
import com.sofn.framework.web.response.ResponseResult;
/**
 * 定制的json message converter，用来处理返回json并在结果上加上处理成功的状态。
 * @author ProntoDoc
 *
 */
public class JsonResultMessageConverter extends MappingJackson2HttpMessageConverter{
	
	@Override
	protected void writePrefix(JsonGenerator generator, Object object) throws IOException {
		generator.writeRaw("{\"resultCode\":\"SUCCESS\",\"result\":");
		
	}

	@Override
	protected void writeSuffix(JsonGenerator generator, Object object) throws IOException {
			generator.writeRaw("}");
	}

}
