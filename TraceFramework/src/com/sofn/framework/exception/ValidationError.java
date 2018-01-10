package com.sofn.framework.exception;

/** 
* @Description: TODO(存放验证结果) 
* @author lilong
* @date 2016年5月5日 
*  
*/
public class ValidationError {

	//验证字段
	private String field;
	//验证结果编码
	private String code;
	//验证结果默认提示信息
	private String defultMsg;
	
	
	@Override
	public String toString() {
		return "ValidationError [field=" + field + ", code=" + code
				+ ", defultMsg=" + defultMsg + "]";
	}

	public ValidationError(String field, String code, String defultMsg) {
		this.field = field;
		this.code = code;
		this.defultMsg = defultMsg;
	}
	
	public String getField() {
		return field;
	}
	public void setField(String field) {
		this.field = field;
	}
	public String getCode() {
		return code;
	}
	public void setCode(String code) {
		this.code = code;
	}
	public String getDefultMsg() {
		return defultMsg;
	}
	public void setDefultMsg(String defultMsg) {
		this.defultMsg = defultMsg;
	}
	
}
