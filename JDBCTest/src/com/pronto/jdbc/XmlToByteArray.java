package com.pronto.jdbc;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.io.OutputStreamWriter;
import java.io.Serializable;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.OutputKeys;
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerException;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.dom.DOMSource;
import javax.xml.transform.stream.StreamResult;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.xml.sax.InputSource;
import org.xml.sax.SAXException;

public class XmlToByteArray implements Serializable{

	/**
	 * @param args
	 */
	private int age;
	private String name;
	private transient Document content;
	
	public static void main(String[] args) throws Exception {
		XmlToByteArray obj = new XmlToByteArray();
		obj.setAge(10);
		obj.setName("name");
		DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
		DocumentBuilder builder = factory.newDocumentBuilder();
		Document document = builder.parse( new File("e:\\oraclexml.xml") );
		Node root = document.getElementsByTagName("pdwdata").item(0);
		NodeList childs = root.getChildNodes();
		int cn = childs.getLength();
		for(int i = 0; i < 10; i ++){//2000
			for(int j = 0; j < cn; j ++){
				Node nd = childs.item(j).cloneNode(true);
				root.appendChild(nd);
			}
		}

		obj.setContent(document);
		FileOutputStream fout = new FileOutputStream("e:\\1.tmp");
		ObjectOutputStream sOut = new ObjectOutputStream(fout);
		sOut.writeObject(obj);
		sOut.flush();
		sOut.close();
		fout.close();
		
		FileInputStream fin = new FileInputStream("e:\\1.tmp");
		ObjectInputStream sIn = new ObjectInputStream(fin);
		XmlToByteArray obj2 = (XmlToByteArray) sIn.readObject();
		sIn.close();
		fin.close();
		System.out.println(obj2.age);
		System.out.println(obj2.name);
		System.out.println(obj2.content.getChildNodes().getLength());
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
	private void writeObject(ObjectOutputStream stream) throws IOException, TransformerException {
		stream.defaultWriteObject();
		TransformerFactory factory = TransformerFactory.newInstance();   
        //factory.setAttribute("indent-number", new Integer(4));// 设置缩进长度为4   
        Transformer transformer = factory.newTransformer();   
        //transformer.setOutputProperty(OutputKeys.INDENT, "yes");// 设置自动换行   
        DOMSource source = new DOMSource(this.content);   
        transformer.transform(source, new StreamResult(new BufferedWriter(   
                new OutputStreamWriter(stream, "UTF-8"))));   
	}
	
	private void readObject(ObjectInputStream stream) throws Exception {
		stream.defaultReadObject();
		DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance(); 
		DocumentBuilder builder = factory.newDocumentBuilder();
		//Document doc = builder.parse(new InputSource(stream));
		Document doc  =  builder.parse(new InputSource(new BufferedReader(new InputStreamReader(stream))));
		this.content = doc;
	}	
}
