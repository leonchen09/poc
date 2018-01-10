package com.station.common.utils;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;

import org.apache.commons.lang.time.DateUtils;

public class MyDateUtils {
	private static String[] pattern = new String[] { "yy.MM.dd", "yyyy.MM.dd", "yyyy-MM", "yyyyMM", "yyyy/MM",
			"yyyyMMdd", "yyyy-MM-dd", "yyyy/MM/dd", "yyyyMMddHHmmss", "yyyy-MM-dd HH:mm:ss", "yyyy/MM/dd HH:mm:ss" };

	public static Date parseDate(String dateStr) {
		try {
			if (dateStr == null || dateStr.trim().length() == 0) {
				return null;
			}
			return DateUtils.parseDate(dateStr, pattern);
		} catch (ParseException e) {
			e.printStackTrace();
			return null;
		}
	}

	public static String getDateString(Date d) {
		if (d == null) {
			return null;
		}
		SimpleDateFormat formatter = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
		String dateString = formatter.format(d);
		return dateString;
	}

	public static String getDateString(Date d, String formatStr) {
		SimpleDateFormat formatter = new SimpleDateFormat(formatStr);
		String dateString = formatter.format(d);
		return dateString;
	}

	public static String diffDays(Date d1, Date d2) {
		if (d1 == null || d2 == null) {
			return "";
		}
		String result = "";
		long diffTime = Math.abs(d1.getTime() - d2.getTime()) / 1000;
		if (diffTime < 24 * 60 * 60) {
			result = diffTime % (24 * 60 * 60) + "天";
		} else {
			long diffDays = diffTime / (24 * 60 * 60);
			if (diffDays < 31) {
				result = diffDays + "天";
			} else {
				if (diffDays / 365 > 0) {
					result = diffDays / 365 + "年" + (diffDays % 365) / 30 + "月";
				} else {
					result = (diffDays % 365) / 30 + "月";
				}
			}
		}
		return result;
	}

	public static Date getDiffTime(long diffTime) {
		Date n = new Date();
		return new Date(n.getTime() + diffTime);
	}

	/**
	 * 获取指定日期当月第一天
	 * @param d
	 * @return
	 */
	public static Date getFirstDay(Date d) {
		Calendar c = Calendar.getInstance();
		c.setTime(d);
		c.set(Calendar.DAY_OF_MONTH, 1);
		return c.getTime();
	}
	
	/**
	 * 得到给定时间相差  diffMonth 个月的1号0点0分0秒
	 * @param currentDate
	 * @param diffMonth 
	 * @return
	 */
	public static Date getFirstDayDiffMonth(Date d, int diffMonth) {
		Calendar c = Calendar.getInstance();
		c.setTime(d);
		c.add(Calendar.MONTH, diffMonth);
		c.set(Calendar.DAY_OF_MONTH, 1);
		//将小时至0  
		c.set(Calendar.HOUR_OF_DAY, 0);  
		//将分钟至0  
		c.set(Calendar.MINUTE, 0);  
		//将秒至0  
		c.set(Calendar.SECOND,0);  
		//将毫秒至0  
		c.set(Calendar.MILLISECOND, 0);  
		return c.getTime();
	}
}
