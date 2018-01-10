package com.station.common.utils;
@FunctionalInterface
public interface FilterStrategys<E> {
	
	E filterStrategy(E e1, E e2, E e3, E e4, E e5, E de);

}
