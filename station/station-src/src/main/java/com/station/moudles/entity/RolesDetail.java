package com.station.moudles.entity;

import java.util.List;

import io.swagger.annotations.ApiModelProperty;

public class RolesDetail extends Roles {
	@ApiModelProperty(value = "角色对应的权限Code", example = "一个角色有多个权限", required = true)
	private List<String> permissionCodeList;

	public List<String> getPermissionCodeList() {
		return permissionCodeList;
	}

	public void setPermissionCodeList(List<String> permissionCodeList) {
		this.permissionCodeList = permissionCodeList;
	}
	
}
