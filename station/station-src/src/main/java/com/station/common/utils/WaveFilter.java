package com.station.common.utils;

import java.math.BigDecimal;
import java.util.List;
import java.util.function.Function;
import java.util.function.ObjDoubleConsumer;

public class WaveFilter{
	
	
	public static <E> void swingFilter(List<E> elements, Function<? super E, ? extends Double> keyExtractor, ObjDoubleConsumer<? super E> propertySet,
			double maxValue, double minValue, double DValue) {
		FilterStrategys<Double> swingStrategy = (Double one, Double two, Double curValue, Double four, Double five, Double DVal) ->{
		double min = one;
		double max = one;
		min = min > two ? two : min;
		max = max < two ? two : max;
		min = min > four ? four : min;
		max = max < four ? four : max;
		min = min > five ? five : min;
		max = max < five ? five : max;
		if(curValue > (max + DVal) || curValue < (min - DVal))
			curValue = (one + two + four + five)/4.0;
		return curValue;
		};
		commonFilter(elements, keyExtractor, propertySet, maxValue, minValue, DValue, swingStrategy);		
	}
	
	public static <E> void avgFilter(List<E> elements, Function<? super E, ? extends Double> keyExtractor, ObjDoubleConsumer<? super E> propertySet,
			double maxValue, double minValue, double DValue) {
		FilterStrategys<Double> avgStrategy = (Double one, Double two, Double curValue, Double four, Double five, Double DVal) ->{
			double avg1 = (one + two) / 2.0;
			double avg2 = (four + five) / 2.0;
			double min = avg1 < avg2 ? avg1 : avg2;
			double max = avg1 > avg2 ? avg1 : avg2;
			if(curValue > (max + DVal) )
				curValue = max + DVal;
			if(curValue < (min - DValue))
				curValue = min - DValue;
			return curValue;
		};
		commonFilter(elements, keyExtractor, propertySet, maxValue, minValue, DValue, avgStrategy);
	}

	private static <E> void commonFilter(List<E> elements, Function<? super E, ? extends Double> keyExtractor, ObjDoubleConsumer<? super E> propertySet,
			Double maxValue, Double minValue, Double DValue, FilterStrategys<Double> filterStrategy) {
		if(elements.size() < 5)
			return ;
		double firstValue = keyExtractor.apply(elements.get(0));
		if(firstValue < minValue)
			findAndSetAvg(0, maxValue, minValue, minValue, minValue, elements, keyExtractor, propertySet);
		else if(firstValue > maxValue)
			findAndSetAvg(0, maxValue, minValue, maxValue, maxValue, elements, keyExtractor, propertySet);
//    	firstValue = firstValue > maxValue ? maxValue : firstValue;
//    	firstValue = firstValue < minValue ? minValue : firstValue;
//    	propertySet.accept(elements.get(0), firstValue);
    	double preValue = keyExtractor.apply(elements.get(1));
    	if(preValue > maxValue ){
    		findAndSetAvg(1, maxValue, minValue, keyExtractor.apply(elements.get(0)), maxValue, elements, keyExtractor, propertySet);
    	}else if(preValue < minValue) {
    		findAndSetAvg(1, maxValue, minValue, keyExtractor.apply(elements.get(0)), minValue, elements, keyExtractor, propertySet);
    	}
//    	preValue = preValue > maxValue ? maxValue : preValue;
//    	preValue = preValue < minValue ? minValue : preValue;
//    	propertySet.accept(elements.get(1), preValue);
    	
    	for(int i = 2; i < elements.size() - 2; i ++) {
    		preValue = keyExtractor.apply(elements.get(i - 1));
    		Double curValue = keyExtractor.apply(elements.get(i));
    		if(curValue < minValue) {
    			i = findAndSetAvg(i, maxValue, minValue, preValue, minValue, elements, keyExtractor, propertySet);
//    			double nextValue = minValue;
//    			int j = i + 1;
//    			for(; j < elements.size(); j ++) {
//    				nextValue = keyExtractor.apply(elements.get(j));
//    				if(nextValue >= minValue && nextValue <= maxValue) {
//    					break;
//    				}
//    			}
//    			double avgDValue = (nextValue - preValue)/(j - i + 1);
//    			for(; i < j; i ++) {
//    				propertySet.accept(elements.get(i), nextValue - avgDValue * (j - i));
//    			}
    		}else if(curValue > maxValue) {
    			i = findAndSetAvg(i, maxValue, minValue, preValue, maxValue, elements, keyExtractor, propertySet);
//    			double nextValue = maxValue;
//    			int j = i + 1;
//    			for(; j < elements.size(); j ++) {
//    				nextValue = keyExtractor.apply(elements.get(j));
//    				if(nextValue >= minValue && nextValue <= maxValue) {
//    					break;
//    				}
//    			}
//    			double avgDValue = (nextValue - preValue)/(j - i + 1);
//    			for(; i < j; i ++) {
//    				propertySet.accept(elements.get(i), nextValue - avgDValue * (j - i));
//    			}
    		}else {
    			Double one = keyExtractor.apply(elements.get(i - 2));
    			Double two = keyExtractor.apply(elements.get(i - 1));
    			Double four = keyExtractor.apply(elements.get(i + 1));
    			Double five = keyExtractor.apply(elements.get(i + 2));
    			curValue = filterStrategy.filterStrategy(one, two, curValue, four, five, DValue);
    			propertySet.accept(elements.get(i), curValue);
    		}
    	}
	}
	
    private static <E> int findAndSetAvg(int i, double maxValue, double minValue, double preValue, double nextValue, List<E> elements, Function<? super E, ? extends Double> keyExtractor, ObjDoubleConsumer<? super E> propertySet) {
		int j = i + 1;
		for(; j < elements.size(); j ++) {
			nextValue = keyExtractor.apply(elements.get(j));
			if(nextValue >= minValue && nextValue <= maxValue) {
				break;
			}
		}
		double avgDValue = (nextValue - preValue)/(j - i + 1);
		for(; i < j; i ++) {
			propertySet.accept(elements.get(i), new BigDecimal(nextValue - avgDValue * (j - i)).setScale(4, BigDecimal.ROUND_HALF_UP).doubleValue());
		}
		return i;
    }
}
