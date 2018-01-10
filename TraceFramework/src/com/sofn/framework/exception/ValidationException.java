package com.sofn.framework.exception;

import java.util.List;

/** 
* @Description: TODO(spring验证异常) 
* @author lilong
* @date 2016-5-4
*  
*/
public class ValidationException  extends RuntimeException{
	private static final long serialVersionUID = 1L;

	private List<ValidationError> list;
	
	
	public ValidationException(String paramString) {
		super(paramString);
	}
	public ValidationException(String paramString, Throwable paramThrowable) {
		super(paramString, paramThrowable);
	}
	
	public ValidationException(String paramString,List<ValidationError> list) {
		super(paramString);
		this.list=list;
	}

	public ValidationException() {
	}

	public String toString() {
		
		return getMessage();
	}
	
	
	public List<ValidationError> getList() {
		return list;
	}

	public void setList(List<ValidationError> list) {
		this.list = list;
	}
}
