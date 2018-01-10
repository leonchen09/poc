package com.station.common.utils;

import java.util.Random;

/**
 * num位随机数产生
 * @author windows
 *
 */
public class RandomUtil {
	public static int randomdigit(int num) {
		int m = (int) Math.pow(10, num);
		int n = 0;
		while (n < m) {
			n = (int) ((Math.random() * 9 + 1) * m);
		}
		return n;
	}

	/**
	 * 生成指定范围的double
	 * @param min 最小值
	 * @param max 最大值
	 * @param num 指定小数位数
	 * @return
	 */
	public static double randomDouble(double min, double max, int num) {
		double result = 0.0;
		double multiplier = Math.pow(10, num);
		min = min * multiplier;
		max = max * multiplier;
		long minl = Math.round(min);
		long maxl = Math.round(max);
		int resulti = randomInt((int) minl, (int) maxl);
		result = (double) resulti / multiplier;
		return result;
	}

	/**
	 * 生成指定范围的整数，包含min和max
	 * @param min
	 * @param max
	 * @return
	 */
	public static int randomInt(int min, int max) {
		int result = 0;
		Random random = new Random();
		result = random.nextInt(max - min + 1) + min;
		return result;
	}

}
