package com.pronto.omni.xml;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.ObjectOutputStream;
import java.io.Serializable;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

import org.w3c.dom.Document;
import org.xml.sax.SAXException;

public class XmlToByteArray implements Serializable{

	/**
	 * @param args
	 */
	private int age;
	private String name;
	private Document content;
	
	public static void main(String[] args) throws Exception {
		XmlToByteArray obj = new XmlToByteArray();
		obj.setAge(10);
		obj.setName("name");
		DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
		DocumentBuilder builder = factory.newDocumentBuilder();
		Document document = builder.parse( new File("e:\\XMLFile1.xml") );
		FileOutputStream fout = new FileOutputStream("e:\\1.tmp");
		ObjectOutputStream sOut = new ObjectOutputStream(fout);
		sOut.writeObject(obj);
		sOut.flush();
		sOut.close();
		fout.close();
	}

	public int getAge() {
		return age;
	}

	public void setAge(int age) {
		this.age = age;
	}

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public Document getContent() {
		return content;
	}

	public void setContent(Document content) {
		this.content = content;
	}

}
