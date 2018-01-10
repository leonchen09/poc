package com.sofn.demo.controller;

import java.sql.Date;
import java.util.ArrayList;
import java.util.List;

import javax.annotation.Resource;
import javax.servlet.http.HttpServletRequest;

import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.WebDataBinder;
import org.springframework.web.bind.annotation.InitBinder;
import org.springframework.web.bind.annotation.ModelAttribute;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.ResponseBody;
import org.springframework.web.context.WebApplicationContext;
import org.springframework.web.context.support.WebApplicationContextUtils;
import org.springframework.web.util.WebUtils;

import com.sofn.demo.entity.Demo;
import com.sofn.demo.query.service.QueryDemoService;
import com.sofn.demo.service.CacheTestService;
import com.sofn.demo.service.CacheTestServiceImpl;
import com.sofn.demo.service.DemoServiceImpl;
import com.sofn.demo.service.DemoService;
import com.sofn.framework.base.entity.PaginationSearch;
import com.sofn.framework.base.entity.Searcher;
import com.sofn.framework.exception.BizException;
import com.sofn.framework.exception.SysException;
import com.sofn.framework.util.JsonUtil;
import com.sofn.framework.web.RequestDataType;
import com.sofn.framework.web.annotation.ActionCode;
import com.sofn.framework.web.annotation.Pagination;
import com.sofn.framework.web.annotation.StatusResponse;

import org.springframework.http.converter.json.MappingJackson2HttpMessageConverter;
import org.springframework.http.converter.xml.MappingJackson2XmlHttpMessageConverter;
import org.springframework.http.converter.StringHttpMessageConverter;

import com.fasterxml.jackson.dataformat.xml.XmlMapper;

import org.springframework.web.servlet.view.json.MappingJackson2JsonView;
import org.mybatis.spring.SqlSessionFactoryBean;

import com.fasterxml.jackson.databind.ObjectMapper;

import org.springframework.web.servlet.mvc.method.annotation.ServletModelAttributeMethodProcessor;
import org.springframework.web.servlet.mvc.method.annotation.RequestMappingHandlerAdapter;
import org.springframework.web.method.annotation.ModelAttributeMethodProcessor;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.propertyeditors.CustomDateEditor;
import org.springframework.cache.annotation.Cacheable;
import org.springframework.context.ApplicationContext;
import org.springframework.context.support.ClassPathXmlApplicationContext;
import org.springframework.context.support.ConversionServiceFactoryBean;
import org.springframework.context.support.FileSystemXmlApplicationContext;
import org.springframework.web.servlet.mvc.method.annotation.ExceptionHandlerExceptionResolver;
import org.springframework.web.servlet.mvc.annotation.ResponseStatusExceptionResolver;
import org.springframework.web.servlet.mvc.support.DefaultHandlerExceptionResolver;
import org.springframework.web.servlet.mvc.method.annotation.RequestResponseBodyMethodProcessor;
import org.springframework.web.servlet.mvc.method.annotation.ViewNameMethodReturnValueHandler;
import org.mybatis.spring.mapper.MapperScannerConfigurer;
import org.springframework.beans.factory.annotation.InjectionMetadata;

import com.mchange.v2.c3p0.ComboPooledDataSource;
import com.danga.MemCached.SockIOPool;

import org.springframework.jdbc.datasource.DataSourceTransactionManager;

@Controller
public class DemoController {

	@Resource
	DemoService demoService;
	
	@Resource
	CacheTestService cacheTestService;
	
	@Resource
	QueryDemoService queryDemoService;
	
//	@InitBinder
//	public void initBinder(WebDataBinder binder) {
//		binder.registerCustomEditor(Date.class, new CustomDateEditor());
//	}
	
	public static void main(String[] argv){
		
		String[] configs = new String[]{"D:\\OneDrive\\source\\POC\\TraceFramework\\config\\spring-context.xml","D:\\OneDrive\\source\\POC\\TraceFramework\\config\\spring-memcached.xml",
		"D:\\OneDrive\\source\\POC\\TraceFramework\\config\\spring-mybatis.xml"};
		ApplicationContext context = new FileSystemXmlApplicationContext(configs);
		DemoService service = context.getBean(DemoService.class);
		System.out.println(service.getObj(222));
		System.out.println(service.getObj(222));
//		service.testCacheList("92");
//		service.testCacheList("92");
//		Demo demo1 = new Demo();
//		demo1.setId(new Long(99));
//		demo1.setName("name99");
//		demo1.setAge(99);
//		service.addtestServiceTx(demo1);
	}
	
	@ActionCode("AC0001")
	@RequestMapping("list1.do")
	public String list1(Model model, int id){
		demoService.getObj(3);
		demoService.getObj(3);
		demoService.testCacheList("9");
		demoService.testCacheList("9");
//		Demo demo = demoService.load(new Long(id));
//		demo = memcacher.get(demo.genKey());
//		model.addAttribute("demo", demo);
		return "list1";
	}
	
	@ActionCode("AC0002")
	@RequestMapping("list2.do")
	public String list2(Demo demo, Model model){
		Searcher<Demo> search = new Searcher<Demo>();
		search.addParameter("name", "%");
		Searcher<Demo> result = demoService.searchByParam(search);
		model.addAttribute("demos", result.getResult());
		return "list2";
	}
	
	@RequestMapping("list3")
	public String list3(Demo demo, Model model, HttpServletRequest request){
		PaginationSearch<Demo> page = new PaginationSearch<Demo>();
		page.setPageNo(Integer.parseInt(WebUtils.findParameterValue(request, "pageNo")));
		page.setPageSize(Integer.parseInt(WebUtils.findParameterValue(request, "pageSize")));
		page.addParameter("name", "n%");
		page = demoService.pageingByParam(page);
//		model.addAttribute("demos", page.getResult());
		model.addAttribute("data", page);
		return "list2";
	}
	
	@RequestMapping("list4")
	public String list4(@Pagination PaginationSearch<Demo> search, Model model){
//		model.addAttribute("demos", demoService.pageingByParam(search).getResult());
		System.out.println("parameter value:" + search.getParameterValue("name"));
		//ģ����ѯ
		search.updateParameterValue("name", search.getParameterValue("name") + "%");
		PaginationSearch<Demo> data = demoService.pageingByParam(search);
		model.addAttribute("data", data);
		return "list2";
	}
	
	
	@RequestMapping("detailJson")
	@StatusResponse
	public Demo detailJson(Demo demo1, HttpServletRequest request){
		System.out.println("result:" + request.getParameter("pageNo"));
		System.out.println("demo1:"+demo1);
//		Demo demo = demoService.load(new Long(2));
		Demo demo = cacheTestService.getObj("2");
		return demo;
	}
	
	@RequestMapping("listJson")
	@StatusResponse
	public List<Demo> listJson(@Pagination PaginationSearch<Demo> search, Model model){
//		search.updateParameterValue("name", search.getParameterValue("name") + "%");
//		PaginationSearch<Demo> data = demoService.pageingByParam(search);
		
//		return data.getResult();
		List<Demo> result = new ArrayList<Demo>();
		Demo d = new Demo();
		d.setAge(1);
		d.setId(new Long(1));
		d.setName("name1");
		d.setSalary(1.1);
		result.add(d);
		Demo d2 = new Demo();
		d2.setAge(1);
		d2.setId(new Long(1));
		d2.setName("name1");
		d2.setSalary(1.1);
		result.add(d2);
		
		
		return result;
	}
	
	@RequestMapping("saveJsonDemo")
	public String saveJsonDemo(@RequestBody Demo demo, HttpServletRequest request){
//		ApplicationContext ac1 = WebApplicationContextUtils.getRequiredWebApplicationContext(request.getServletContext());
//		RequestResponseBodyMethodProcessor processor = ac1.getBean(RequestResponseBodyMethodProcessor.class);
		System.out.println("demo:" + demo.getName());
		//demoService.updateByID(demo);
		return "list1";
	}
	
	@RequestMapping("saveDemo")
	public String saveDemo(Demo demo){
		System.out.println("demo:" + demo.getName());
		demo.setId(new Long(1));
//		memcacher.put(demo);
		demoService.addtestServiceTx(demo);
		return "list1";
	}
	@RequestMapping("addData")
	public String addData(){
		return "addData";
	}
	
	@RequestMapping("testException")
	public String testException(String name) throws Exception{
		if("biz".equals(name)){
			throw new BizException("biz exception message");
		}else if("sys".equals(name)){
			throw new SysException("system exception message");
		}else if("ex".equals(name)){
			throw new Exception("common exception message");
		}
		return "index";
	}
	
	@RequestMapping("testExceptionJson")
	@ResponseBody
	public String testExceptionJson(String name) throws Exception{
		if("biz".equals(name)){
			throw new BizException("biz exception message");
		}else if("sys".equals(name)){
			throw new SysException("system exception message");
		}else if("ex".equals(name)){
			throw new Exception("common exception message");
		}
		return "index";
	}
	
	@RequestMapping("readlist")//和listjson一致。
	@StatusResponse
	public List<Demo> readlist(@Pagination(dataType=RequestDataType.JSON) PaginationSearch<Demo> search, Model model){
		search.updateParameterValue("name", search.getParameterValue("name") + "%");
//		PaginationSearch<Demo> data = demoService.readList(search);
		PaginationSearch<Demo> data = queryDemoService.readList(search);
		return data.getResult();
	}
	
	@RequestMapping("testJosnDirect")
	@ResponseBody
	public String testJsonDirect(Demo demo){
		Searcher<Demo> search = new Searcher<Demo>();
		search.addParameter("name", "%");
		Searcher<Demo> result = demoService.searchByParam(search);
		String resultstr = JsonUtil.toJsonStr(result);
		return resultstr;
	}
	
	@RequestMapping("testmvccache")
	@ResponseBody
	@Cacheable("mvccache")
//	public String testMVCCache(Demo demo){
	public String testMVCCache(@RequestBody Demo demo){
		System.out.println("get object from controller");
		Demo demoresult = demoService.load(new Long(2));
		return JsonUtil.toJsonStr(demoresult);
	}
	
	
}
