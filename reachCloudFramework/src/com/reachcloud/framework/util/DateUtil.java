package com.reachcloud.framework.util;

import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Date;

import com.reachcloud.framework.exception.BizException;

/**
 * @Description: 日期格式化
 * @author chenwl
 * @date 2016-4-27
 * 
 */
public class DateUtil {

	private static DateFormat ddMMyyyyFormat = new SimpleDateFormat("dd/MM/yyyy");

	private static DateFormat yyyyMMddFormat = new SimpleDateFormat("yyyy-MM-dd");
	
	private static DateFormat yyyyMMddHHmmFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm");
	  
	private static DateFormat yyyyMMddHHmmssFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
	
	private static String errMsg="数据项非日期格式！";
	
	
	
	private static String doFormat(DateFormat dateFormat,Date date){
		if(date!=null){
			return dateFormat.format(date);
		}
		return "";
	}
	
	private static Date doParse(DateFormat dateFormat,String str){
		 Date date = null;
		 try {
		      date = dateFormat.parse(str);
		 } catch (Exception e) {
		    	throw new BizException(errMsg);
		 }
		 return date;
		
	}
	
	
	/**
	 * 日期转字符串格式化(dd/MM/yyyy)
	 * @param date
	 * @return
	 */
	public static String getStrDDMMYYYY(Date date) {
		return doFormat(ddMMyyyyFormat,date);
	}

	/**
	 * 日期转字符串格式化(yyyy-MM-dd)
	 * @param date
	 * @return
	 */
	public static String getStrYYYYMMDD(Date date) {
	    return doFormat(yyyyMMddFormat,date);
	}
	
	/**
	 * 日期转字符串格式化(yyyy-MM-dd HH:mm)
	 * @param date
	 * @return
	 */
	public static String getStrYYYYMMDDHHMM(Date date) {
	    return doFormat(yyyyMMddHHmmFormat,date);
	}
	
	/**
	 * 日期转字符串格式化(yyyy-MM-dd HH:mm:ss)
	 * @param date
	 * @return
	 */
	public static String getStrYYYYMMDDHHMMSS(Date date) {
	    return doFormat(yyyyMMddHHmmssFormat,date);
	}
	
	/**
	 * 日期转字符串，自定义格式
	 * @param date
	 * @param format
	 * @return
	 */
	public static String getStrFormat(Date date,String format){
		DateFormat dateFormat = new SimpleDateFormat(format);
		return doFormat(dateFormat,date);
		
	}
	
	
	
	
	
	
	/**
	 * 字符串转日期格式化(dd/MM/yyyy)
	 * @param strDate
	 * @return
	 */
	public static Date getDateDDMMYYYY(String strDate) {
	    return doParse(ddMMyyyyFormat,strDate);
	}
	
	
	/**
	 * 字符串转日期格式化(yyyy-MM-dd)
	 * @param strDate
	 * @return
	 */
	public static Date getDateYYYYMMDD(String strDate) {

	    return doParse(yyyyMMddFormat,strDate);
	}
	
	/**
	 * 字符串转日期格式化(yyyy-MM-dd HH:mm)
	 * @param strDate
	 * @return
	 */
	public static Date getDateYYYYMMDDHHMM(String strDate) {

	    return doParse(yyyyMMddHHmmFormat,strDate);
	}
	
	/**
	 * 字符串转日期格式化(yyyy-MM-dd HH:mm:ss)
	 * @param strDate
	 * @return
	 */
	public static Date getDateYYYYMMDDHHMMSS(String strDate) {
	    return doParse(yyyyMMddHHmmssFormat,strDate);
	}
	
	
	
	
	/**
	 * 字符串转日期，自定义格式
	 * @param strDate
	 * @param format
	 * @return
	 */
	public static Date getDateFormat(String strDate,String format){
		DateFormat dateFormat = new SimpleDateFormat(format);
		 
		return doParse(dateFormat,strDate);
		
	}
	
	
	
	
	
	
	public static void main(String str[]){
		
		System.out.println("getStrDDMMYYYY:"+getStrDDMMYYYY(new Date()));
		
		System.out.println("getStrYYYYMMDD:"+getStrYYYYMMDD(new Date()));
		
		System.out.println("getStrFormat:"+getStrFormat(new Date(),"yyyy年mm月dd"));
		
		
		
		//System.out.println("getDateDDMMYYYY:"+getDateDDMMYYYY("fasfd"));
		
		
		System.out.println("getDateDDMMYYYY:"+ddMMyyyyFormat.format(new Date()));
		
		try{
			System.out.println("getDateFormat:"+getDateYYYYMMDDHHMMSS("2015-01-01 23:22:12"));
		}catch(BizException b){
			System.out.println(b);
		}
		
	}
	
	
}
