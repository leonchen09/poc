package com.sofn.demo.entity;

import java.io.Serializable;
import java.util.Date;

import javax.xml.bind.annotation.XmlRootElement;

import org.springframework.format.annotation.DateTimeFormat;

import com.alibaba.fastjson.annotation.JSONField;

@XmlRootElement(name="Demo")
public class Demo implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = -7737471691136263963L;

	private Long id;
	
	private String name;
	
	private int age;
	
	private double salary;
	
	private int[] colors;
	
	@DateTimeFormat(pattern="yyyy-MM-dd")//form表单提交的日期格式
//	@JsonFormat(pattern = "yyyy-MM-dd")//jackson提交和输出的日期格式
	@JSONField(format="yy/MM/dd HH:mm:ss")//fastjson输出的日期格式，输入时该格式是默认的，可以不用注解。
	private Date birthday;

	public Long getId() {
		return id;
	}

	public void setId(Long id) {
		this.id = id;
	}

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public int getAge() {
		return age;
	}

	public void setAge(int age) {
		this.age = age;
	}

	public double getSalary() {
		return salary;
	}

	public void setSalary(double salary) {
		this.salary = salary;
	}

	public Date getBirthday() {
		return birthday;
	}

	public void setBirthday(Date birthday) {
		this.birthday = birthday;
	}

	public int[] getColors() {
		return colors;
	}

	public void setColors(int[] colors) {
		this.colors = colors;
	}

	public String toString(){
		return "demo, name:"+name+",id:" + id + ",salary:"+salary;
	}
	
}
