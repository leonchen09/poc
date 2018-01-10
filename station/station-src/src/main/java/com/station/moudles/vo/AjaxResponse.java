/*** Eclipse Class Decompiler plugin, copyright (c) 2012 Chao Chen (cnfree2000@hotmail.com) ***/
package com.station.moudles.vo;

import java.io.Serializable;

import com.station.common.Constant;

import io.swagger.annotations.ApiModel;
import io.swagger.annotations.ApiModelProperty;

@ApiModel(value = "返回结果", description = "返回结果描述")
public class AjaxResponse<T> implements Serializable {
	private static final long serialVersionUID = 1L;
	@ApiModelProperty(value = "错误编码", example = Constant.RS_CODE_SUCCESS, required = true)
	private String code;
	@ApiModelProperty(value = "提示信息", example = "成功", required = true)
	private String msg;
	@ApiModelProperty(value = "返回结果集")
	private T data;

	public AjaxResponse() {
		setCode(Constant.RS_CODE_SUCCESS);
		setMsg(Constant.RS_MSG_SUCCESS);
	}

	public AjaxResponse(String code, String msg, T data) {
		super();
		this.code = code;
		this.msg = msg;
		this.data = data;
	}

	public AjaxResponse(String code, String msg) {
		super();
		this.code = code;
		this.msg = msg;
	}

	public AjaxResponse(T data) {
		setData(data);
		setCode(Constant.RS_CODE_SUCCESS);
		setMsg(Constant.RS_MSG_SUCCESS);
	}

	public void isError() {
		setCode(Constant.RS_CODE_ERROR);
		setMsg(Constant.RS_MSG_ERROR);
	}

	public void isSuccess() {
		setCode(Constant.RS_CODE_SUCCESS);
		setMsg(Constant.RS_MSG_SUCCESS);
	}

	public String getCode() {
		return code;
	}

	public void setCode(String code) {
		this.code = code;
	}

	public String getMsg() {
		return msg;
	}

	public void setMsg(String msg) {
		this.msg = msg;
	}

	public T getData() {
		return data;
	}

	public void setData(T data) {
		this.data = data;
	}

}
