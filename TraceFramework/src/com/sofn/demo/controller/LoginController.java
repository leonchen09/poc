package com.sofn.demo.controller;

import java.util.Calendar;
import java.util.concurrent.atomic.AtomicInteger;

import javax.servlet.http.HttpServletRequest;

import org.springframework.context.ApplicationContext;
import org.springframework.context.support.FileSystemXmlApplicationContext;
import org.springframework.expression.Expression;
import org.springframework.expression.ExpressionParser;
import org.springframework.expression.spel.standard.SpelExpressionParser;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.RequestMapping;

import com.alibaba.fastjson.JSON;
import com.alibaba.fastjson.parser.Feature;
import com.sofn.demo.entity.Demo;
import com.sofn.demo.service.CacheTestService;
import com.sofn.demo.service.DemoService;

@Controller
public class LoginController {

	@RequestMapping("login.do")
	public String login(String userName, HttpServletRequest request){
		request.getSession().setAttribute("loginuser", userName);
		return "main";
	}
	
    public static void main(String[] args) throws Exception {
    	
//        String json = "{\"name\":\"22323\", \"age\": 1234," +
//                " \"birthday\": \"2012-12/12 12:12:12\",\"salary\":50.06}";
//        Demo t = JSON.parseObject(json, Demo.class,JSON.DEFAULT_PARSER_FEATURE, new Feature[0]);
//        System.out.println(t.getName());
//        System.out.println(t.getAge());
//        System.out.println(t.getBirthday());
//        System.out.println(t.getSalary());
    	
		String[] configs = new String[]{"f:\\OneDrive\\source\\POC\\TraceFramework\\config\\spring-context.xml","F:\\OneDrive\\source\\POC\\TraceFramework\\config\\spring-memcached.xml",
				"F:\\OneDrive\\source\\POC\\TraceFramework\\config\\spring-mybatis.xml"};
		ApplicationContext context = new FileSystemXmlApplicationContext(configs);
		DemoService demoService = context.getBean(DemoService.class);
		demoService.load(new Long(1));
////		CacheTestService service = context.getBean(CacheTestService.class);
//		CacheTestService service = (CacheTestService) context.getBean("cacheTestServiceImpl");
//		System.out.println(service.getObj("2")); 
//		System.out.println(service.getObj("2")); 
//		System.out.println(service.getObj("2")); 
//		System.out.println(service.getObj("2")); 
//		System.out.println(service.getObj("2")); 
//		System.out.println(service.testCacheList("9"));
//		System.out.println(service.testCacheList("9"));
////    	LoginController test = new LoginController();
////    	System.out.println(test.jsonConvert());
//        
    }
    
    
    private String jsonConvert(){
    	Demo d = new Demo();
    	d.setAge(99);
    	d.setBirthday(Calendar.getInstance().getTime());
    	d.setId(new Long(1));
    	d.setName("name99");
    	d.setSalary(99.99);
    	long startTime = System.currentTimeMillis();
    	String dstr = JSON.toJSONString(d);
    	long endTime = System.currentTimeMillis();
    	System.out.println("escape time:" + (endTime - startTime));
    	return dstr;
    }
}
