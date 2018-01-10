package com.reachcloud.demo.controller;

import javax.servlet.http.HttpServletRequest;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.RequestMapping;

@Controller
public class LoginController {
	
	@RequestMapping("login.do")
	public String login(String userName, HttpServletRequest request){
		request.getSession().setAttribute("loginuser", userName);
		return "main";
	}
}
