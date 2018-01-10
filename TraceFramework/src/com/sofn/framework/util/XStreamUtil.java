package com.sofn.framework.util;

import java.io.BufferedReader;
import java.io.FileReader;
import java.util.ArrayList;
import java.util.Date;


import java.util.HashMap;
import java.util.List;
import java.util.Map;

import com.sofn.framework.exception.BizException;
import com.thoughtworks.xstream.XStream;
import com.thoughtworks.xstream.io.xml.DomDriver;

/** 
* @Description: TODO(xml转换) 
* @author lilong
* @date 2016年5月5日 
*  
*/
public class XStreamUtil {
	
	
	/**
	 * 对象转换成xml
	 * @param obj
	 * @return
	 */
	public static String toXml(Object obj){  
		
        XStream xs=new XStream();  
        xs.alias("datasource",obj.getClass());  
        return xs.toXML(obj);  
    }  
	
	
	public static void main(String strs[]) {
		Bean bean=new Bean();
		bean.setD(2423445.2334234D);
		bean.setDate(new Date());
		System.out.println(toXml(bean));
		
		List list =new ArrayList();
		list.add("张三");
		list.add("李四");
		list.add(222);
		Map map=new HashMap();
		map.put("bean", "obj.cms.bean");
		list.add(map);
		list.add(bean);
		
		System.out.println(toXml(list));
		
		
	}
}
