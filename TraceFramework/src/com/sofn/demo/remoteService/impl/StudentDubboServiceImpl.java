package com.sofn.demo.remoteService.impl;

import org.springframework.stereotype.Service;

import com.sofn.demo.entity.StudentBean;
import com.sofn.demo.remoteService.IStudentDubboService;

/**
 * @ClassName:StudentDubboServiceImpl
 * @Description TODO(dubboԶ�̷�����Խӿ�ʵ����)
 * @author xinglai.chen
 * @date 2016��6��8������5:15:20
 */
@Service("studentDubboService")
public class StudentDubboServiceImpl implements IStudentDubboService{
	
	private static Integer EXECUTE_NUM=0;
	@Override
	public StudentBean getStudent (){
		// TODO Auto-generated method stub
		
		StudentBean bean = new StudentBean("ZHANG三",20,"男");
		
		System.out.println("provider-->"+bean.toString());
		
		EXECUTE_NUM++;
		
		System.out.println("ִ�д���Ϊ��"+EXECUTE_NUM);
		
		return bean;
	}

}
