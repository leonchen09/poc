package com.reachcloud.framework.util;

import static org.junit.Assert.*;

import java.util.ArrayList;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;




import org.junit.Test;

import net.sf.json.JSONArray;

import com.alibaba.fastjson.JSON;
import com.alibaba.fastjson.serializer.SerializeConfig;
import com.alibaba.fastjson.serializer.SerializerFeature;
import com.alibaba.fastjson.serializer.SimpleDateFormatSerializer;
import com.reachcloud.framework.exception.BizException;

/**
 * @Description: json转换
 * @author chenwl
 * @date 2016-4-28
 * 
 */
public class JsonUtil {
	
	
	
	private static SerializeConfig mapping = new SerializeConfig();  
	
	
	/**
	 * java对象转换成json.默认的日期格式(yyyy-MM-dd HH:mm:ss)
	 * @param o
	 * @return
	 */
	public static  String toJsonStr(Object o) {
		try {
			return JSON.toJSONString(o,SerializerFeature.WriteDateUseDateFormat);  
		} catch (Exception e) {
			throw new BizException("转换错误");
		}
	}
	
	/**
	 * java对象转换成json，日期格式自定义
	 * @param o
	 * @param format
	 * @return
	 */
	public static String toJsonStr(Object o,String format) {
		try {
			mapping.put(Date.class, new SimpleDateFormatSerializer(format));  
	        return JSON.toJSONString(o, mapping);  
		} catch (Exception e) {
			throw new BizException("转换错误");
		}
	}
	
	
	/**
	 * json转换成对象.默认的日期格式(yyyy-MM-dd HH:mm:ss)
	 * @param t
	 * @param json
	 * @return
	 */
	public static <T>T toObject(Class<T> t,String json) {
		try { 
			return JSON.parseObject(json,t); 
		} catch (Exception e) {
			throw new BizException("转换错误");
		}
	}
	
	
	
	/**
	 * json转换成List
	 * @param t
	 * @param json
	 * @return
	 */
	public static <T>List<T> toList(Class<T> t,String json) {
		try {
			
			return JSON.parseArray(json,t);  
		} catch (Exception e) {
			throw new BizException("转换错误");
		}
	}
	/**
	 * List转换成json字符串
	 */
	public static String toJsonString(List list) {  
        JSONArray jsonarray = JSONArray.fromObject(list);  
        return jsonarray.toString();
    }  
	
	
	@Test
	public void testToJsonString() throws Exception {
		List list=new ArrayList();
		list.add("xiaoming");
		list.add("xiaohong");
		list.add("xiaoqiang");
		System.out.println(this.toJsonString(list));
	}
	
	
	
	public static void main(String str[]) {
		Map map = new HashMap();
		map.put("name", "json");
		map.put("bool", Boolean.TRUE);
		System.out.println("toJsonStr:" + toJsonStr(map));

		List list = new ArrayList();
		list.add("张三");
		list.add("李四");
		List listn = new ArrayList();
		listn.add(list);
		System.out.println("toJsonStr:" + toJsonStr(listn));

		
		
		
		Bean bean=new Bean();
		bean.setD(2423445.2334234D);
		bean.setDate(new Date());
		//bean.setDate2()
		bean.setS("张三历史");
		System.out.println("toJsonStr:" + toJsonStr(bean));
		System.out.println("toJsonStr:" + toJsonStr(bean,"yyyy-MM-dd"));
		System.out.println("toJsonStr:" + toJsonStr(5));
		
		
		
		
		String json2 = "[{id:\"110000\",\"city\":\"北#001京市\"}]"; 
		List<Map> list2 = toList(Map.class, json2);
		System.out.println("toArray:" + list2.get(0).get("id"));
		
		/*String json3 = "{'d':222,'date':'2015年15月20日 12点12分12秒',\"city\":\"北#001京市\"}"; 
		Bean map2 = toObject(Bean.class, json3,"yyyy年MM月dd日 HH点mm分ss秒");
		System.out.println("toObject:" +map2.getDate());
		System.out.println("toObject:" +DateUtil.getStrFormat(map2.getDate(), "yyyy/MM/dd HH/mm/ss") );*/
		
		String json3 = "{'d':222,'date':'2015-15-20 12:12:12',\"city\":\"北#001京市\"}"; 
		Bean map2 = toObject(Bean.class, json3);
		System.out.println("toObject:" +map2.getDate());
		
		
		String json4 = "[{'d':222,'date':'2015-15-20 12:12:12',\"city\":\"北#001京市\"}]"; 
		
		List<Bean> list3 = toList(Bean.class, json4);
		System.out.println("toArray:" + list3.get(0).getDate());
		
		
		
		
		String json5 = "['f','ss']"; 
		
		List<String> list4 = toList(String.class, json5);
		System.out.println("toArray:" + list4.get(1));
		
		/*

		String jsonArray2 = "['userId','userName']";
		String[] ss2 = (String[]) jsonToArray(jsonArray2, String.class);
		System.out.println("jsonToArray:" + ss2[0] + ":" + ss2[1]);

		

		String json3 = "['userId','userName','userI2d','user2Name']";
		List list3 = (List) jsonToList(json3, Map.class);

		System.out.println("jsonToList:" + list3.get(3).toString());*/
	}
}

class Bean{
	Date date;
	double d;
	String s;
	Date date2;
	
	public Date getDate2() {
		return date2;
	}
	public void setDate2(Date date2) {
		this.date2 = date2;
	}
	public Date getDate() {
		return date;
	}
	public void setDate(Date date) {
		this.date = date;
	}
	public double getD() {
		return d;
	}
	public void setD(double d) {
		this.d = d;
	}
	public String getS() {
		return s;
	}
	public void setS(String s) {
		this.s = s;
	}
}