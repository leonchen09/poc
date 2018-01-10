package com.station.moudles.entity;

import java.util.Date;
import java.util.List;

import com.fasterxml.jackson.annotation.JsonFormat;

import io.swagger.annotations.ApiModelProperty;

public class Roles {
	@ApiModelProperty(value = "pk", example = "pk", required = true)
	private Integer roleId;

	@ApiModelProperty(value = "roleName", required = true)
	private String roleName;

	@JsonFormat(pattern = "yyyy-MM-dd HH:mm:ss", timezone = "GMT+8")
	@ApiModelProperty(value = "时间", required = true)
	private Date createTime;

	@ApiModelProperty(value = "createId", required = true)
	private Integer createId;

	@ApiModelProperty(value = "createName", required = true)
	private String createName;

	@ApiModelProperty(value = "1管理后台，2支撑后台", required = true)
	private Integer roleSystem;

	public Integer getRoleId() {
		return roleId;
	}

	public void setRoleId(Integer roleId) {
		this.roleId = roleId;
	}

	public String getRoleName() {
		return roleName;
	}

	public void setRoleName(String roleName) {
		this.roleName = roleName == null ? null : roleName.trim();
	}

	public Date getCreateTime() {
		return createTime;
	}

	public void setCreateTime(Date createTime) {
		this.createTime = createTime;
	}

	public Integer getCreateId() {
		return createId;
	}

	public void setCreateId(Integer createId) {
		this.createId = createId;
	}

	public String getCreateName() {
		return createName;
	}

	public void setCreateName(String createName) {
		this.createName = createName == null ? null : createName.trim();
	}

	public Integer getRoleSystem() {
		return roleSystem;
	}

	public void setRoleSystem(Integer roleSystem) {
		this.roleSystem = roleSystem;
	}
}