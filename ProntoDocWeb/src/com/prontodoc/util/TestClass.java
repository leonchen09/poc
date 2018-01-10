package com.prontodoc.util;

import java.sql.Date;
import java.text.DateFormat;
import java.text.ParseException;
import java.text.SimpleDateFormat;

public class TestClass {

  /**
   * test method
 * @throws ParseException 
   */
  public static void main(String[] args) throws ParseException {
//    Date d = Date.valueOf("1573-01-01");
//    System.out.println(d);
//    DateFormat df = new SimpleDateFormat("yyyy-MM-dd hh:mm:ss");
//    java.util.Date date = df.parse("1573-01-01 00:00:00");
//    System.out.println(date);
	  Parent p = new Child();
	  p.method1();
	  
  }
}

class Parent{
	public Parent(){
		System.out.println("parent constructor");
	}
	
	protected void method1(){
		System.out.println("parent method1");
	}
}

class Child extends Parent{
	public Child(){
		System.out.println("child constructor");
	}
	@Override
	protected void method1(){
//		super.method1();
		System.out.println("child method1");
	}
}