package com.station.moudles.entity;

import java.util.List;

import io.swagger.annotations.ApiModelProperty;

public class UserDetail extends User {

	@ApiModelProperty(value = "用户的角色", example = "一个用户有多个角色", required = true)
	private List<Roles> rolesList;

	public List<Roles> getRolesList() {
		return rolesList;
	}

	public void setRolesList(List<Roles> rolesList) {
		this.rolesList = rolesList;
	}
	
}
