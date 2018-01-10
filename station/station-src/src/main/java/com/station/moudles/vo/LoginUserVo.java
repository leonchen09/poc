package com.station.moudles.vo;

import javax.validation.constraints.NotNull;

import org.hibernate.validator.constraints.NotBlank;

import io.swagger.annotations.ApiModelProperty;

public class LoginUserVo {

	@NotBlank
	@ApiModelProperty(value = "账号", example = "账号", required = true)
	private String loginId;

	/**
	 * This field corresponds to the database column users.user_password  用户密码
	 */
	@NotBlank
	@ApiModelProperty(value = "用户密码", example = "用户密码", required = true)
	private String userPassword;

	@NotNull
	@ApiModelProperty(value = "用户类型:1管理后台，2客户，3app", required = true)
	private Integer userType;

	/**
	 * @return the loginId
	 */
	public String getLoginId() {
		return loginId;
	}

	/**
	 * @param loginId the loginId to set
	 */
	public void setLoginId(String loginId) {
		this.loginId = loginId;
	}

	/**
	 * @return the userPassword
	 */
	public String getUserPassword() {
		return userPassword;
	}

	/**
	 * @param userPassword the userPassword to set
	 */
	public void setUserPassword(String userPassword) {
		this.userPassword = userPassword;
	}

	/**
	 * @return the userType
	 */
	public Integer getUserType() {
		return userType;
	}

	/**
	 * @param userType the userType to set
	 */
	public void setUserType(Integer userType) {
		this.userType = userType;
	}

}
