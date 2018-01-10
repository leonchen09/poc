package com.pronto.jdbc;

import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.StringWriter;

import javax.xml.transform.OutputKeys;
import javax.xml.transform.Result;
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerConfigurationException;
import javax.xml.transform.TransformerException;
import javax.xml.transform.sax.SAXSource;
import javax.xml.transform.sax.SAXTransformerFactory;
import javax.xml.transform.sax.TransformerHandler;
import javax.xml.transform.stream.StreamResult;

import org.xml.sax.InputSource;
import org.xml.sax.SAXException;
import org.xml.sax.helpers.AttributesImpl;

public class CreateBigXml {

	/**
	 * @param args
	 * @throws TransformerException 
	 * @throws FileNotFoundException 
	 */
	public static void main(String[] args) throws FileNotFoundException, TransformerException {
        CreateBigXml saxxml = new CreateBigXml();
        System.out.println("开始生成。。。");
        long start = System.currentTimeMillis();
        saxxml.saxToXml();
        //saxxml.fileToSaxXml();
        long end = System.currentTimeMillis();
        System.out.println("完成。。。");
        System.out.println("共用时:"+ (end - start)/1000);
    }
	
	public void fileToSaxXml() throws FileNotFoundException, TransformerException{
		Result resultXml = new StreamResult(new FileOutputStream("E:\\test1.xml"));   
           
        // 创建SAX转换工厂 
        SAXTransformerFactory sff = (SAXTransformerFactory)SAXTransformerFactory.newInstance();   
        // 转换处理器，侦听 SAX ContentHandler    
        //解析事件，并将它们转换为结果树 Result   
        TransformerHandler th = sff.newTransformerHandler();   
        // 将源树转换为结果树  
        Transformer transformer = th.getTransformer();   
        // 设置字符编码  
        transformer.setOutputProperty(OutputKeys.ENCODING, "UTF-8");   
        // 是否缩进
        transformer.setOutputProperty(OutputKeys.INDENT, "yes");   
            
        //设置与用于转换的此 TransformerHandler 关联的 Result   
        th.setResult(resultXml);            
        
        
        FileInputStream in = new FileInputStream("E:\\test.xml");
        InputSource is = new InputSource(in);
        SAXSource xs = new SAXSource(is);
        transformer.transform(xs, resultXml);

	}

    //public String saxToXml(List list) {
    public String saxToXml() {
        String xmlStr = null;   
        try {   
            //用来生成XML文件   
            // 要生成文件需构造PrintWriter的writer   
            // 实现此接口的对象包含构建转换结果树所需的信息 
            Result resultXml = new StreamResult(new FileOutputStream("E:\\test2.xml"));   
            //用来得到XML字符串形式   
            // 一个字符流，可以用其回收在字符串缓冲区中的输出来构造字符串
            StringWriter writerStr = new StringWriter();   
            // 构建转换结果树所需的信息。 
            Result resultStr = new StreamResult(writerStr);   
               
            // 创建SAX转换工厂 
            SAXTransformerFactory sff = (SAXTransformerFactory)SAXTransformerFactory.newInstance();   
            // 转换处理器，侦听 SAX ContentHandler    
            //解析事件，并将它们转换为结果树 Result   
            TransformerHandler th = sff.newTransformerHandler();   
            // 将源树转换为结果树  
            Transformer transformer = th.getTransformer();   
            // 设置字符编码  
            transformer.setOutputProperty(OutputKeys.ENCODING, "UTF-8");   
            // 是否缩进
            transformer.setOutputProperty(OutputKeys.INDENT, "yes");   
                
            //设置与用于转换的此 TransformerHandler 关联的 Result   
            //注：这两个th.setResult不能同时启用
            th.setResult(resultXml);            
            //th.setResult(resultStr);   
               
            
            
            th.startDocument();
            AttributesImpl attr = new AttributesImpl();   
            th.startElement("", "", "PEOPLE", attr);   
            //if (null != list && !list.isEmpty()) {   
                //for (int i = 0; i < list.size(); i++) {
                for (int i = 0; i <5000000; i++) {
                    attr.addAttribute("","","PERSONID","","E0"+i);
                    th.startElement("", "", "PERSON", attr);
                    attr.clear();
                    
                    th.startElement("", "", "NAME", attr);   
                    String id = String.valueOf("xuehui"+ i);   
                    th.characters(id.toCharArray(), 0, id.length());   
                    th.endElement("", "", "NAME");   
  
                    th.startElement("", "", "ADDRESS", attr);   
                    String address = String.valueOf("address"+i);   
                    th.characters(address.toCharArray(), 0, address.length());   
                    th.endElement("", "", "ADDRESS");
                    
                    th.startElement("", "", "TEL", attr);   
                    String tel = String.valueOf("tel"+i);   
                    th.characters(tel.toCharArray(), 0, tel.length());   
                    th.endElement("", "", "TEL"); 
                    
                    th.startElement("", "", "FAX", attr);   
                    String fax = String.valueOf("fax"+i);   
                    th.characters(fax.toCharArray(), 0, fax.length());   
                    th.endElement("", "", "FAX"); 
                    
                    th.startElement("", "", "EMAIL", attr);   
                    String email = String.valueOf("chenfu_20"+i+"@163.com");   
                    th.characters(email.toCharArray(), 0, email.length());   
                    th.endElement("", "", "EMAIL"); 
                    
                    th.endElement("", "", "PERSON");  
                    //attr.setAttribute(i, "", "", "PERSONID", "", ""+i);
                }   
            //}
            th.endElement("", "", "PEOPLE");   
            th.endDocument();   
            xmlStr = writerStr.getBuffer().toString();
        } catch (TransformerConfigurationException e) {   
            e.printStackTrace();   
        } catch (SAXException e) {   
            e.printStackTrace();   
        } catch (Exception e) {   
            e.printStackTrace();   
        }   
        System.out.println("SAX:" + xmlStr);   
        return xmlStr;   
    }


}
