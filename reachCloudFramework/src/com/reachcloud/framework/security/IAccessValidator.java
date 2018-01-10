package com.reachcloud.framework.security;

import javax.servlet.http.HttpServletRequest;
/**
 * 
 * @author Chenwl
 * @date 2016年4月22日
 */
public interface IAccessValidator {
	
	boolean validateActionCode(HttpServletRequest request, String actionCode);
	
}
