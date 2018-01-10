package com.pronto.omni.xml;

import java.io.File;
import java.io.StringReader;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import javax.xml.bind.JAXBContext;
import javax.xml.bind.JAXBException;
import javax.xml.bind.Marshaller;
import javax.xml.bind.Unmarshaller;

public class JAXBTest {
	
	
	public static void main(String[] argv) throws JAXBException{
		serial();
		deserial();
	}

	public static void serial() throws JAXBException{
		
		ObjTest t = new ObjTest();
		t.age = 10;
		t.name= "张三";
		t.birthday = new Date();
		t.sex = false;
		t.strs = new ArrayList<String>();
		t.strs.add("aaa");
		t.strs.add("bbb");
		List<Child> children = new ArrayList<Child>();
		children.add(new Child());
		children.add(new Child());
		t.children = children;
		
		JAXBContext con = JAXBContext.newInstance(ObjTest.class);  
		Marshaller marshaller = con.createMarshaller();  
		marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true);  
		marshaller.marshal(t, new File("e:\\210.xml")); 

	}
	
	
	public static void deserial() throws JAXBException{
		JAXBContext context = JAXBContext.newInstance(ObjTest.class);
		Unmarshaller unmarshaller = context.createUnmarshaller();
		ObjTest t = (ObjTest) unmarshaller.unmarshal(new File("e:\\220.xml"));
		System.out.println(t);
	}
	
}
