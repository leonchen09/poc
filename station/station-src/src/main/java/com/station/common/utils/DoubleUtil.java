package com.station.common.utils;

import java.math.BigDecimal;
import java.util.List;

public class DoubleUtil {

	public static void main(String[] argv) {
		List<Double> orgValues = ConvertUtils.separateStringIntoDoubleList("19.208-0-2.258-2.242-2.236-2.23-2.23-2.23-2.23-2.238-2.228-2.226-2.236-2.23-2.234-2.224-2.226-2.232-2.224-2.23-2.224-2.234-2.222-2.232-2.232-2.234-2.224-2.23-2.236-2.226-2.206-2.232-2.226-2.22-2.23-2.226-2.226-2.224-2.22-2.226-2.218-2.226-2.228-2.232-2.226-2.218-2.22-2.226-2.228-2.22-2.22-2.226-2.218-2.226-2.226-2.228-2.23-2.232-2.22-2.222-2.18-2.216-2.22-2.216-2.22-2.2-2.226-2.216-2.224-2.226-2.226-2.228-2.218-2.218-2.226-2.22-2.226-2.216-2.224-2.226-2.214-2.226-2.218-2.216-2.22-2.224-2.22-2.22-2.216-2.214-2.222-2.226-2.216-2.214-2.214-2.226-2.214-2.226-2.214-2.22-2.22-2.226-2.214-2.212-2.226-2.226-2.22-2.214-2.226-2.226-2.224-2.214-2.226-2.224-2.222-2.218-2.226-2.214-2.216-2.224-2.224-2.214-2.214-2.22-2.214-2.222-2.226-2.22-2.226-2.214-2.224-2.218-2.22-2.22-2.218-2.22-2.22-2.214-2.212-2.216-2.218-2.214-2.214-2.22-2.22-2.214-2.22-2.224-2.224-2.22-2.216-2.22-2.218-2.22-2.216-2.214-2.214-2.212-2.22-2.214-2.218-2.216-2.214-2.216-2.214-2.218-2.218-2.212-2.22-2.212-2.216-2.224-2.226-2.22-2.212-2.222-2.212-2.22-2.212-2.214-2.22-2.21-2.212-2.212-2.214-2.22-2.216-2.212-2.214-2.214-2.22-2.212-2.2-2.212-2.214-2.222-2.212-2.214-2.214-2.216-2.216-2.214-2.224-2.218-2.2-2.214-2.218-2.22-2.22-2.21-2.22-2.21-2.21-2.21-2.22-2.21-2.21-2.222-2.212-2.222-2.206-2.218-2.214-2.208-2.214-2.22-2.21-2.22-2.21-2.214-2.22-2.22-2.21-2.222-2.212-2.212-2.22-2.21-2.216-2.2-2.22-2.21-2.21-2.216-2.212-2.216-2.212-2.224-2.21-2.216-2.222-2.21-2.206-2.212-2.21-2.216-2.22-2.22-2.22-2.218-2.222-2.216-2.21-2.218-2.222-2.22-2.214-2.214-2.21-2.22-2.22-2.216-2.216-2.22-2.212-2.216-2.208-2.218-2.214-2.222-2.216-2.21-2.214-2.208-2.212-2.212-2.218-2.216-2.22-2.22-2.22-2.24-2.22-2.208-2.216-2.22-2.24-2.21-2.214-2.22-2.208-2.214-2.208-2.208-2.21-2.22-2.216-2.214-2.21-2.218-2.214-2.21-2.214-2.232-2.228-2.23-2.24-2.232-2.222-2.238-2.238-2.25-2.236-2.236-2.236-2.238-2.226-2.234-2.226-2.23-2.232-2.24-2.232-2.24-2.238-2.23-2.23-2.238-2.23-2.24-2.228-2.236-2.226-2.236-2.236-2.234-2.236-2.22-2.23-2.238-2.23-2.23-2.23-2.23-2.24-2.234-2.238-2.232-2.23-2.23-2.236-2.23-2.23-2.23-2.24-2.242-2.234-2.24-2.234-2.242-2.232-2.24-2.24-2.238-2.23-2.26-2.26-2.24-2.236-2.234-2.238-2.24-2.236-2.24-2.232-2.24-2.238-2.24-2.234-2.232-2.23-2.24-2.236-2.232-2.238-2.24-2.236-2.242-2.24-2.238-2.24-2.242-2.236-2.24-2.24-2.23-2.24-2.242-2.24-2.244-2.242-2.232-2.242-2.242-2.242-2.232-2.236-2.246-2.234-2.232-2.24-2.234-2.246-2.24-2.234-2.236-2.236-2.226-2.244-2.24-2.236-2.244-2.244-2.25-2.242-2.246-2.246-2.238-2.246-2.246-2.242-2.244-2.232-2.24-2.26-2.244-2.24-2.236-2.236-2.24-2.24-2.236-2.238-2.246-2.236-2.238-2.242-2.246-2.246-2.244-2.26-2.244-2.236-2.236-2.248-2.24-2.246-2.244-2.246-2.24-2.24-2.242-2.238-2.244-2.238-2.25-2.244-2.246-2.238-2.244-2.24-2.24-2.244-2.24-2.246-2.246-2.244-2.24-2.24-2.244-2.244-2.246-2.246-2.244-2.244-2.24-2.246-2.248-2.256-2.24-2.246-2.242-2.246-2.248-2.25-2.242-2.25-2.24-2.244-2.248-2.244-2.242-2.248-2.242-2.248-2.248-2.246-2.244-2.248-2.246");
		orgValues = filter(orgValues, 2.5, 1.5, 0.1);
		orgValues.forEach(System.out::println);
	}
	
	   
    public static List<Double> filter(List<Double> orgValues, double maxValue, double minValue, double DValue) {
    	if(orgValues.size() < 5)
    		return orgValues;
    	
    	double firstValue = orgValues.get(0);
    	if(firstValue > maxValue)
    		findAndSetAvg(0, maxValue, minValue, maxValue, maxValue, orgValues);
    	else if(firstValue < minValue)
    		findAndSetAvg(0, maxValue, minValue, minValue, minValue, orgValues);

    	double preValue = orgValues.get(1);
    	if(preValue > maxValue)
    		findAndSetAvg(1, maxValue, minValue, orgValues.get(0), maxValue, orgValues);
    	else if(preValue < minValue)
    		findAndSetAvg(1, maxValue, minValue, orgValues.get(0), minValue, orgValues);
    	
    	for(int i = 2; i < orgValues.size() - 2; i ++) {
    		preValue = orgValues.get(i - 1);
    		double curValue = orgValues.get(i);
    		if(curValue < minValue) {
    			i = findAndSetAvg(i, maxValue, minValue, preValue, minValue, orgValues);
//    			double nextValue = minValue;
//    			int j = i + 1;
//    			for(; j < orgValues.size(); j ++) {
//    				nextValue = orgValues.get(j);
//    				if(nextValue >= minValue && nextValue <= maxValue) {
//    					break;
//    				}
//    			}
//    			double avgDValue = (nextValue - preValue)/(j - i + 1);
//    			for(; i < j; i ++) {
//    				orgValues.set(i, nextValue - avgDValue * (j - i));
//    			}
    		}else if(curValue > maxValue) {
    			i = findAndSetAvg(i, maxValue, minValue, preValue, maxValue, orgValues);
//    			double nextValue = maxValue;
//    			int j = i + 1;
//    			for(; j < orgValues.size(); j ++) {
//    				nextValue = orgValues.get(j);
//    				if(nextValue >= minValue && nextValue <= maxValue) {
//    					break;
//    				}
//    			}
//    			double avgDValue = (nextValue - preValue)/(j - i + 1);
//    			for(; i < j; i ++) {
//    				orgValues.set(i, nextValue - avgDValue * (j - i));
//    			}
    		}else {
    			double one = orgValues.get(i - 2);
    			double two = orgValues.get(i - 1);
    			double four = orgValues.get(i + 1);
    			double five = orgValues.get(i + 2);
    			double min = one;
    			double max = one;
    			min = min > two ? two : min;
    			max = max < two ? two : max;
    			min = min > four ? four : min;
    			max = max < four ? four : max;
    			min = min > five ? five : min;
    			max = max < five ? five : max;
    			if(curValue > (max + DValue) || curValue < (min - DValue))
    				orgValues.set(i, new BigDecimal((one + two + four + five)/4.0).setScale(4, BigDecimal.ROUND_HALF_UP).doubleValue());
    		}
    	}
    	return orgValues;
    }
    
    private static int findAndSetAvg(int i, double maxValue, double minValue, double preValue, double nextValue,List<Double> orgValues) {
    	int j = i + 1;
		for(; j < orgValues.size(); j ++) {
			nextValue = orgValues.get(j);
			if(nextValue >= minValue && nextValue <= maxValue) {
				break;
			}
		}
		double avgDValue = (nextValue - preValue)/(j - i + 1);
		for(; i < j; i ++) {
			orgValues.set(i, new BigDecimal(nextValue - avgDValue * (j - i)).setScale(4, BigDecimal.ROUND_HALF_UP).doubleValue());
		}
		return i;
    }
}
