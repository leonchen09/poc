package com.sofn.demo.test;

import static org.junit.Assert.*;

import javax.annotation.Resource;

import org.junit.Test;
import org.junit.runner.RunWith;
import org.springframework.test.context.ContextConfiguration;
import org.springframework.test.context.junit4.AbstractJUnit4SpringContextTests;
import org.springframework.test.context.junit4.SpringJUnit4ClassRunner;

import com.sofn.demo.entity.StudentBean;
import com.sofn.demo.remoteService.IStudentDubboService;

@RunWith(SpringJUnit4ClassRunner.class)
@ContextConfiguration(locations={"classpath:spring-context.xml","classpath:spring-dubbo-consumer.xml","classpath:spring-memcached.xml","classpath:spring-mybatis.xml"})
public class SpringTest extends AbstractJUnit4SpringContextTests  {

	@Resource
	IStudentDubboService studentDubboService;
	
	@Test
	public void test() {
		StudentBean bean = studentDubboService.getStudent();
		System.out.println("bean:" + bean);
	}

}
