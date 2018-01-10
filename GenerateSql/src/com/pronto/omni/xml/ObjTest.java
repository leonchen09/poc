package com.pronto.omni.xml;

import java.io.Serializable;
import java.text.DateFormat;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.List;

import javax.xml.bind.annotation.XmlElement;
import javax.xml.bind.annotation.XmlElementWrapper;
import javax.xml.bind.annotation.XmlElements;
import javax.xml.bind.annotation.XmlRootElement;

@XmlRootElement(name = "ObjTest")
public class ObjTest implements Serializable{
	
	public static void main(String[] argv) throws ParseException, ClassNotFoundException, InstantiationException, IllegalAccessException{
		DateFormat sf = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSSSSSSz");
		Date d = sf.parse("2013-01-01T12:59:59.1230000GMT+08:00");
		System.out.println("date:"+d);
		Child cc = new Child();
		cc.ttt = "343";
		Class c = Class.forName("com.pronto.omni.xml.Child");
		Child cc2 = (Child) c.newInstance();
		System.out.println(cc2.children.get(1).ttt);
	}
	
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	public int age;
	public String name;
	public Date birthday;
	@xmlIngore
	public boolean sex;
//	@XmlElementWrapper(name="children")
//	@XmlElements({ @XmlElement(name = "Child", type = Child.class) })
//	public List<Child> children;
	@XmlElementWrapper(name="strs")
	@XmlElements({ @XmlElement(name = "string", type = String.class) })
	public List<String> strs;
	
//	public int getAge() {
//		return age;
//	}
//	public void setAge(int age) {
//		this.age = age;
//	}
//	public String getName() {
//		return name;
//	}
//	public void setName(String name) {
//		this.name = name;
//	}
//	public Date getBirthday() {
//		return birthday;
//	}
//	public void setBirthday(Date birthday) {
//		this.birthday = birthday;
//	}
//	public boolean isSex() {
//		return sex;
//	}
//	public void setSex(boolean sex) {
//		this.sex = sex;
//	}
	public String toString(){
		return "age:"+age+","+name+","+birthday+","+sex;
	}
}