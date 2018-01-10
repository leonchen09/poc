package com.test.poc;

public interface Itest {
	
	String method1();
	
	default int method2(int n1){
		return n1+2;
	}
}
