package com.sofn.framework.util;

import java.util.Date;

import javax.servlet.http.HttpServletRequest;

import com.sofn.framework.exception.BizException;

/**
 * @Description: TODO(获取特定类型的parameter)
 * @author lilong
 * @date 2016-4-27
 * 
 */
public class HttpParamUtil {
	
	
	
	
	public static  String getParameterStr(HttpServletRequest request,
			String param)  {
		String value = request.getParameter(param);
		if (value == null)
			value = "";
		value = value.trim();

		request.setAttribute(param, value);
		return value.trim();
	}

	public static  String getParameterStr(HttpServletRequest request,
			String param, boolean isEmpty, String errMsg) {
		String value =getParameterStr(request,param);
		
		if ((isEmpty) && (value.length() == 0)) {
			String exMessage = errMsg;
			if ((errMsg == null) || (errMsg.length() <= 0)){
				exMessage = "提交的数据项不能为空！";
			}
			throw new BizException(exMessage);
		}
		return value;
	}

	public static String getParameterStr(HttpServletRequest request,
			String param, boolean isEmpty)  {
		return getParameterStr(request,param,isEmpty);
	}
	
	public static  int getParameterInt(HttpServletRequest request,
			String param)  {
		String valueStr =  getParameterStr(request, param, true);
		int result=0;
		try {
			result = Integer.parseInt(valueStr);
		} catch (NumberFormatException e) {
			throw new BizException("提交的数据项非整数型！");
		}
		return result;
	}
	
	
	public static long getParameterLong(HttpServletRequest request,
			String param){
		String valueStr =  getParameterStr(request, param, true);
		long result=0L;
		try{
			result=Long.parseLong(valueStr);
		}catch (NumberFormatException e) {
			throw new BizException("提交的数据项非整数型！");
		}
		
		return result;
	}
	
	
	public static double getParameterDouble(HttpServletRequest request,
			String param){
		String valueStr =  getParameterStr(request, param, true);
		double result=0L;
		try{
			result=Double.parseDouble(valueStr);
		}catch (NumberFormatException e) {
			throw new BizException("提交的数据项非整数型！");
		}
		
		return result;
	}
	
	
	public static Date getParameterDate(HttpServletRequest request,
			String param){
		String  valueStr =  getParameterStr(request, param, true);
		
		Date result=DateUtil.getDateYYYYMMDD(valueStr);
		
		
		return result;
		
	}
	
	
	
	public static String[] getParameterStringValues(HttpServletRequest request,
			String param){
		
		String[] values = request.getParameterValues(param);
	    return ((values != null) ? values : new String[0]);
		
	}
	
	
	
	
	
	
	
	public static void main(String str[]){
		
		Date result=DateUtil.getDateYYYYMMDD("asdf");
		
		System.out.println(result);
	}
	
	
	
	
	
}
