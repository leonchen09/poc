package com.reachcloud.framework.web.response;

public class ResponseResult {
	
	private String resultCode;
	
	private Object result;
	
	public ResponseResult(){
		
	}
	
	public ResponseResult(String resultCode, Object result){
		this.resultCode = resultCode;
		this.result = result;
	}

	public String getResultCode() {
		return resultCode;
	}

	public void setResultCode(String resultCode) {
		this.resultCode = resultCode;
	}

	public Object getResult() {
		return result;
	}

	public void setResult(Object result) {
		this.result = result;
	}

	
}
