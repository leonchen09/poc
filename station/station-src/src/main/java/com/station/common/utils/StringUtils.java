package com.station.common.utils;

public class StringUtils {
	public static boolean isNull(Object obj) {
		if (obj == null || obj.toString().trim().equals("")) {
			return true;
		} else {
			return false;
		}
	}

	public static String getString(Object obj) {
		if (obj == null) {
			return null;
		} else {
			String result = obj.toString().trim();
			if (result.equals("")) {
				result = null;
			}
			return result;
		}
	}
}
