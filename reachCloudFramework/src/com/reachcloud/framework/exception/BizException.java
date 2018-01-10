package com.reachcloud.framework.exception;
/** 
* @Description: TODO(业务异常) 
* @author chenwl
* @date 2016-4-27 
*  
*/
public class BizException  extends RuntimeException{
	private static final long serialVersionUID = 1L;

	public BizException(String paramString, Throwable paramThrowable) {
		super(paramString, paramThrowable);
	}

	public BizException(String paramString) {
		super(paramString);
	}

	public BizException() {
	}

	public String toString() {
		return getMessage();
	}
}

