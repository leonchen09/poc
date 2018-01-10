package com.sofn.demo.entity;

import java.io.Serializable;

/**
 * @ClassName:StudentBean
 * @Description TODO(用于测试dubbo远程调用的实体类)
 * @author xinglai.chen
 * @date 2016年6月8日下午5:13:56
 */
public class StudentBean implements Serializable{

	/**
	 * serialVersionUID
	 */
	private static final long serialVersionUID = 8611398789846083301L;
	
	private String name;
	private Integer age;
	private String gender;
	
	public StudentBean() {
		super();
	}
	public StudentBean(String name, Integer age, String gender) {
		super();
		this.name = name;
		this.age = age;
		this.gender = gender;
	}
	public String getName() {
		return name;
	}
	public void setName(String name) {
		this.name = name;
	}
	public Integer getAge() {
		return age;
	}
	public void setAge(Integer age) {
		this.age = age;
	}
	public String getGender() {
		return gender;
	}
	public void setGender(String gender) {
		this.gender = gender;
	}
	@Override
	public String toString() {
		return "StudentBean [name=" + name + ", age=" + age + ", gender="
				+ gender + "]";
	}

}
