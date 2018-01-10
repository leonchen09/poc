package com.sofn.framework.util;

import java.text.DecimalFormat;

/**
 * @Description: TODO(数据类型转换、金额转换)
 * @author lilong
 * @date 2016-4-26
 * 
 */
public class Converter {
	/**
	 * float2int(四舍五入)
	 * 
	 * @param in
	 * @return
	 */
	public static int float2int(float in) {
		int flag = 1;
		if (in < 0.0F)
			flag = -1;
		int result = (int) (Math.abs(in) + 0.501F);
		return (result * flag);
	}

	/**
	 * double2long(四舍五入)
	 * 
	 * @param in
	 * @return
	 */
	public static long double2long(double in) {
		long flag = 1L;
		if (in < 0.0D)
			flag = -1L;
		long result = (long) (Math.abs(in) + 0.501D);
		return (result * flag);
	}

	/**
	 * str2int
	 * 
	 * @param str
	 * @param defaultValue
	 * @return
	 */
	public static int str2int(String str, int defaultValue) {
		try {
			return new Integer(str.trim()).intValue();
		} catch (Exception e) {
		}
		return defaultValue;
	}

	/**
	 * str2int
	 * 
	 * @param str
	 * @return
	 */
	public static int str2int(String str) {
		return str2int(str, 0);
	}

	/**
	 * str2float
	 * 
	 * @param str
	 * @param defaultValue
	 * @return
	 */
	public static float str2float(String str, float defaultValue) {
		try {
			return new Float(str.trim()).floatValue();
		} catch (Exception e) {
		}
		return defaultValue;
	}

	/**
	 * str2double(四舍五入)
	 * 
	 * @param str
	 * @param defaultValue
	 * @return
	 */
	public static double str2double(String str, double defaultValue) {
		try {
			str = str.trim();
			return new Double(str).doubleValue();
		} catch (Exception e) {
		}
		return defaultValue;
	}

	/**
	 * str2long
	 * 
	 * @param str
	 * @param defaultValue
	 * @return
	 */
	public static long str2long(String str, long defaultValue) {
		try {
			return new Long(str.trim()).longValue();
		} catch (Exception e) {

		}
		return defaultValue;
	}

	public static long str2long(String str) {
		return str2long(str, 0L);
	}

	/**
	 * obj to string
	 * 
	 * @param obj
	 * @return
	 */
	public static String gstring(Object obj) {
		String s = "";
		if (obj != null) {
			if (obj instanceof String) {
				s = (String) obj;
			} else {
				s = obj.toString();
			}
		}
		return s;
	}

	/**
	 * 元转换成分
	 * 
	 * @param yuan
	 * @return
	 */
	public static long yuan2fen(double yuan) {
		return Math.round(yuan * 100.0D);
	}

	/**
	 * 分转换成元
	 * 
	 * @param fen
	 * @return
	 */
	public static double fen2yuan(long fen) {
		return (fen / 100.0D);
	}

	private static DecimalFormat getDecimalFormat(String format) {
		if (format == null) {
			format = "";
		}
		DecimalFormat db = new DecimalFormat(format);
		return db;
	}

	/**
	 * 元转分(自定义格式)
	 * 
	 * @param fen
	 * @param format
	 * @return
	 */
	public static String formatFen2yuan(long fen, String format) {
		DecimalFormat db = getDecimalFormat(format);
		return db.format(fen2yuan(fen));
	}

	/**
	 * 分转元(自定义格式)
	 * 
	 * @param fen
	 * @param format
	 * @return
	 */
	public static String formatYuan2fen(double yuan, String format) {
		DecimalFormat db = getDecimalFormat(format);

		return db.format(yuan2fen(yuan));
	}

	public static void main(String s[]) {

		System.out.println("float2int:"
				+ float2int(21231313.23232123123213123123F));

		System.out.println("double2long:" + double2long(0.499D));

		System.out.println("str2int:" + str2int(null, 5));

		System.out.println("fen2yuan:" + fen2yuan(5356));

		System.out.println("formatFen2yuan:" + formatFen2yuan(5356, "#0.00"));
		System.out.println("formatFen2yuan:" + formatFen2yuan(5356, null));
		System.out.println("yuan2fen:" + yuan2fen(5.12003));
		System.out.println("formatYuan2fen:" + formatYuan2fen(5.12003, "#0.0"));
	}
}
