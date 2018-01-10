package com.sofn.framework.web.response;

public enum ResultCode {
	SUCCESS("0000"), BIZEXCEPTION("0020"), SYSEXCEPTION("0010"),VALIDATIONEXCEPTION("0021");
	
	private String _code;
	
	private ResultCode(String code){
		this._code = code;
	}
	
	public String getCode(){
		return this._code;
	}
	
}
