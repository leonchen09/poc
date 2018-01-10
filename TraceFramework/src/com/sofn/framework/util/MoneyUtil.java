package com.sofn.framework.util;

import com.sofn.framework.exception.BizException;

/**
 * @Description: TODO(金额格式化)
 * @author lilong
 * @date 2016-4-28
 * 
 */
public class MoneyUtil {

	private static final String[] num = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "七", "捌", "玖" };
	private static final String[] unit = { "角", "分" };
	public static String toUpper(String amount) {
		StringBuffer result = new StringBuffer("");
		
		
		String[] tmp = amount.replaceAll(",", "").split("\\.");
		String integer = tmp[0];
		
		
		int LEN = integer.length();
		if (LEN > 12) {
			throw new BizException("金额超出计算范围！");
		}
		for (int k = 12; k > LEN; --k) {
			integer = "0" + integer;
		}
		
		int part1 = Converter.str2int((integer.substring(0, 4)));
		int part2 = Converter.str2int((integer.substring(4, 8)));
		int part3 = Converter.str2int((integer.substring(8, 12)));


		if (part1 != 0) {
			result.append(parseInt(part1) + "亿");
		}
		if (part2 != 0) {
			result.append(parseInt(part2) + "万");
		}
		if (part3 != 0) {
			result.append(parseInt(part3));
		}
		result.append("元");
		if (tmp.length == 2) {
			result.append(parseFloat(tmp[1]));
		}
		return result.toString();
	}

	public static String parseInt(int i) {
		
		String result = "";
		int tmp = i;
		if (tmp / 1000 != 0) {
			result = result + num[(tmp / 1000)] + "仟";
			tmp -= tmp / 1000 * 1000;
		}
		if (tmp / 100 != 0) {
			result = result + num[(tmp / 100)] + "佰";
			tmp -= tmp / 100 * 100;
		}
		if (tmp / 10 != 0) {
			result = result + num[(tmp / 10)] + "拾";
			tmp -= tmp / 10 * 10;
		}
		if (tmp != 0)
			result = result + num[tmp];
		return result;
	}

	public static String parseFloat(String sStr) {
		
		
		String result = "";

		if ("00".equals(sStr)) {
			return "整";
		}

		int LEN = sStr.length();
		for (int i = 0; i < LEN; ++i) {
			String tmp = sStr.substring(i, i + 1);
			int k = Integer.parseInt(tmp);
			result = result + num[k] + unit[i];
		}
		return result;
	}

	public static void main(String st[]) {
		// String s= toUpper("asdf");
		System.out.println("s2:" + toUpper("323"));
		System.out.println("s2:" + new Integer("2222222").intValue());
		
	}
}
