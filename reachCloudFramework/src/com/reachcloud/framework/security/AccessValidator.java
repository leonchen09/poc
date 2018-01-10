package com.reachcloud.framework.security;

import javax.servlet.http.HttpServletRequest;

import org.springframework.stereotype.Component;
/**
 * 
 * @author Chenwl
 * @date 2016年4月22日
 */
@Component("accessValidator")
public class AccessValidator implements IAccessValidator {

	@Override
	public boolean validateActionCode(HttpServletRequest request,
			String actionCode) {
		String userName = (String) request.getSession().getAttribute("loginuser");
		String userCode="AC0001";//获取当前登录用户的权限
		if(actionCode.equals(userCode) && "aa".equals(userName))
			return true;
		return false;
	}
	


}
