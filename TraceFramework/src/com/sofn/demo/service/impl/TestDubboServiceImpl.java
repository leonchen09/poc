package com.sofn.demo.service.impl;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.ApplicationContext;
import org.springframework.context.annotation.Lazy;
import org.springframework.context.support.FileSystemXmlApplicationContext;
import org.springframework.stereotype.Service;

import com.sofn.demo.entity.StudentBean;
import com.sofn.demo.remoteService.IStudentDubboService;
import com.sofn.demo.service.ITestDubboService;

/**
 * @ClassName:TestDubboServiceImpl
 * @Description TODO(���Ա��ص���dubboԶ�̷���ӿ�ʵ����)
 * @author xinglai.chen
 * @date 2016��6��8������5:16:44
 */
@Service
public class TestDubboServiceImpl implements ITestDubboService {

	@Lazy
	@Autowired
	private IStudentDubboService studentDubboService;

	@Override
	public StudentBean getStudent() {
		// TODO Auto-generated method stub
		StudentBean bean;

		bean = studentDubboService.getStudent();
		System.out.println("consumer-->" + bean.toString());
		return bean;
	}
	
	public static void main(String[] args) {
		String[] configs = new String[]{"D:\\OneDrive\\source\\POC\\TraceFramework\\config\\spring-context.xml","D:\\OneDrive\\source\\POC\\TraceFramework\\config\\spring-memcached.xml",
		"D:\\OneDrive\\source\\POC\\TraceFramework\\config\\spring-mybatis.xml","D:\\OneDrive\\source\\POC\\TraceFramework\\config\\spring-dubbo-consumer.xml"};
		ApplicationContext context = new FileSystemXmlApplicationContext(configs);
		ITestDubboService bean = (ITestDubboService) context.getBean(ITestDubboService.class);
		System.out.println(bean.getStudent().toString());
	}

}
