package com.sofn.framework.exception;


/** 
* @Description: TODO(系统异常) 
* @author lilong
* @date 2016-4-27 
*  
*/
public class SysException extends RuntimeException{
	private static final long serialVersionUID = 1L;

	public SysException(String paramString, Throwable paramThrowable) {
		super(paramString, paramThrowable);
	}

	public SysException(String paramString) {
		super(paramString);
	}

	public SysException() {
	}

	public String toString() {
		return getMessage();
	}
}
