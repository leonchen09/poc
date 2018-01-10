package com.pronto.omni.xml;

import java.io.FileInputStream;
import java.sql.SQLException;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

import org.w3c.dom.Document;

public class XmlEncoding {

	
	public static void main(String[] argv) throws Exception{
		DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
		DocumentBuilder dbd = factory.newDocumentBuilder();
		Document doc = dbd.parse(new FileInputStream("E:\\ProntoDir\\pocdata.xml"));
		String value = doc.getChildNodes().item(0).getTextContent();
		System.out.println("Value:"+value);
	}
}
