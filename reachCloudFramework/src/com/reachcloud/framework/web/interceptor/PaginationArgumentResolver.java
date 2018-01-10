package com.reachcloud.framework.web.interceptor;

import java.util.HashMap;
import java.util.Iterator;
import java.util.Map;

import org.apache.log4j.Logger;
import org.json.JSONObject;
import org.springframework.core.MethodParameter;
import org.springframework.web.bind.support.WebDataBinderFactory;
import org.springframework.web.context.request.NativeWebRequest;
import org.springframework.web.method.support.HandlerMethodArgumentResolver;
import org.springframework.web.method.support.ModelAndViewContainer;

import com.reachcloud.framework.base.entity.PaginationSearch;
import com.reachcloud.framework.web.RequestDataType;
import com.reachcloud.framework.web.annotation.Pagination;
/**
 * 为controller插入分页查询对象。
 * @author Chenwl
 * @date 2016年5月3日
 */
public class PaginationArgumentResolver implements HandlerMethodArgumentResolver {
	
	private static Logger logger = Logger.getLogger(PaginationArgumentResolver.class);

	@Override
	public boolean supportsParameter(MethodParameter parameter) {
		return parameter.hasParameterAnnotation(Pagination.class);
	}
	
	/**
	 * Resolve argument of controller's method annotation with @Pagination
	 */
	@Override
	public Object resolveArgument(MethodParameter parameters,
			ModelAndViewContainer container, NativeWebRequest request,
			WebDataBinderFactory factory) throws Exception {
		PaginationSearch pagination = new PaginationSearch();
		String pageNoStr = "";
		String pageSizeStr = "";
		
		Pagination paginationAnnotation = parameters.getParameterAnnotation(Pagination.class);
		if(paginationAnnotation.dataType() == RequestDataType.JSON){//客户端提交json对象
//		    HttpServletRequest servletRequest = request.getNativeRequest(HttpServletRequest.class);
//			ApplicationContext ac1 = WebApplicationContextUtils.getRequiredWebApplicationContext(servletRequest.getServletContext());
//			MappingJackson2HttpMessageConverter converter = ac1.getBean(MappingJackson2HttpMessageConverter.class);
//			ServletServerHttpRequest inputMessage = new ServletServerHttpRequest(servletRequest);
//			PaginationSearch search = (PaginationSearch) converter.read(PaginationSearch.class, inputMessage);

			String jsonData = request.getParameter("data");
			Map<String, String> parameterMap = jsonToObject(jsonData);
			//处理提交的参数
			for(Iterator<String> it = parameterMap.keySet().iterator(); it.hasNext(); ){
				String name = it.next();
				String value = parameterMap.get(name);
				if(name.equals("pageNo")){
					pageNoStr = value.toString();
				}else if(name.equals("pageSize")){
					pageSizeStr = value.toString();
				}
				else{//其余参数加入map供mybatis使用。
					pagination.addParameter(name, value);
				}
			}
		}else{ //normal form data
			for(Iterator<String> it = request.getParameterNames(); it.hasNext(); ){
				String name = it.next();
				String value = request.getParameter(name);
				if(name.equals("pageNo")){
					pageNoStr = value.toString();
				}else if(name.equals("pageSize")){
					pageSizeStr = value.toString();
				}
				else{
					pagination.addParameter(name, value);
				}
			}
		}

		try{
			pagination.setPageNo(Integer.parseInt(pageNoStr));
			pagination.setPageSize(Integer.parseInt(pageSizeStr));
		}
		catch(Exception ex){
			logger.warn("Failed to retrieve page information, use default values.", ex);
		}
		
		return pagination;
	}

	private Map<String, String> jsonToObject(String jsonStr) throws Exception {
		JSONObject jsonObj = new JSONObject(jsonStr);
		Iterator<String> nameItr = jsonObj.keys();
		String name;
		Map<String, String> outMap = new HashMap<String, String>();
		while (nameItr.hasNext()) {
			name = nameItr.next();
			outMap.put(name, jsonObj.getString(name));
		}
		return outMap;
	}
	
}
