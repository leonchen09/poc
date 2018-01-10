package com.pronto.omni.xml;

import java.beans.XMLDecoder;
import java.beans.XMLEncoder;
import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.io.Serializable;
import java.io.UnsupportedEncodingException;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import com.thoughtworks.xstream.XStream;
import com.thoughtworks.xstream.converters.extended.ISO8601DateConverter;

public class XmlObjSerial {

	/**
	 * @param args
	 * @throws IOException 
	 */
	public static void main(String[] args) throws IOException {
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
		obj2CsharpXml(t, "e:\\21.xml");
//		FileInputStream in = new FileInputStream("e:\\22.xml");
//		ObjTest t2 = csharpXml2Obj(in, new ObjTest());
//		System.out.println(t2.toString());
//		in.close();
//                         
//        FileOutputStream out = new FileOutputStream("e:\\java2.xml");
//        BufferedOutputStream bufferOut = new BufferedOutputStream(out);
//       
//        writeObjectToXML(bufferOut, t);
//       
//        out.close();
//        bufferOut.close();
//       
//       
//        FileInputStream in = new FileInputStream("e:\\java2.xml");
//        BufferedInputStream bufferIn = new BufferedInputStream(in);
//       
//        ObjTest t1 = readObjectFromXML(bufferIn);
//       
//        in.close();
//        bufferIn.close();       
//       
//        System.out.println(t1.name);
//        System.out.println(t1.birthday);


	}
	public static <T extends Serializable> void writeObjectToXML(OutputStream out, T obj){
        XMLEncoder xmlEncoder = null;
       
        try{
            xmlEncoder = new XMLEncoder(out);
            xmlEncoder.writeObject(obj);
        }finally{
            if(null != xmlEncoder)
                xmlEncoder.close();
        }
    }
   
    @SuppressWarnings("unchecked")
    public static <T extends Serializable> T readObjectFromXML(InputStream in){
        T obj = null;
        XMLDecoder xmlDecoder = null;
       
        try{
            xmlDecoder = new XMLDecoder(in);
            obj = (T) xmlDecoder.readObject();
        }finally{
            if(null != xmlDecoder)
                xmlDecoder.close();
        }
        return obj;
    }
    
    public static <T> void obj2CsharpXml(T object, OutputStream out) throws IOException{
    	XStream xStream = new XStream();
    	out.write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n".getBytes("UTF-8"));
    	xStream.alias(object.getClass().getSimpleName(), object.getClass());
    	xStream.registerConverter(new ISO8601DateConverter());
    	xStream.toXML(object , out);
    }

    public static <T> void obj2CsharpXml(T object, String fullFileName) throws IOException{
    	FileOutputStream out = new FileOutputStream(fullFileName);
    	obj2CsharpXml(object, out);
    	out.flush();
    	out.close();
    }
    
    public static <T> T csharpXml2Obj(FileInputStream in, T object){
    	XStream xStream = new XStream();
    	xStream.aliasType(object.getClass().getSimpleName(), object.getClass());
    	xStream.registerConverter(new ISO8601DateConverter());
    	T result = (T) xStream.fromXML(in);
    	return result;
    }
     
    public static <T> T csharpXml2Obj(String xml, T object){
    	XStream xStream = new XStream();
    	xStream.aliasType(object.getClass().getSimpleName(), object.getClass());
    	xStream.registerConverter(new ISO8601DateConverter());
    	T result = (T) xStream.fromXML(xml);
    	return result;
    }
}

