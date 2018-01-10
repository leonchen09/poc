package com.reachcloud.demo.controller;

import java.util.ArrayList;
import java.util.List;

import javax.annotation.Resource;
import javax.servlet.http.HttpServletRequest;

import org.springframework.cache.annotation.Cacheable;
import org.springframework.context.ApplicationContext;
import org.springframework.context.support.FileSystemXmlApplicationContext;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.ResponseBody;
import org.springframework.web.util.WebUtils;

import com.reachcloud.demo.entity.Demo;
import com.reachcloud.demo.service.DemoService;
import com.reachcloud.framework.base.entity.PaginationSearch;
import com.reachcloud.framework.base.entity.Searcher;
import com.reachcloud.framework.exception.BizException;
import com.reachcloud.framework.exception.SysException;
import com.reachcloud.framework.util.JsonUtil;
import com.reachcloud.framework.web.RequestDataType;
import com.reachcloud.framework.web.annotation.ActionCode;
import com.reachcloud.framework.web.annotation.Pagination;
import com.reachcloud.framework.web.annotation.StatusResponse;

@Controller
public class DemoController {

	@Resource
	DemoService demoService;
	
	
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
		Demo demo = demoService.load(new Long(2));
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
		PaginationSearch<Demo> data = demoService.readList(search);
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
//	public String testMVCCache(Demo demo){
	public String testMVCCache(@RequestBody Demo demo){
		System.out.println("get object from controller");
		Demo demoresult = demoService.load(new Long(2));
		return JsonUtil.toJsonStr(demoresult);
	}
	
	
}
