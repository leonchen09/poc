package com.station.moudles.entity;

import io.swagger.annotations.ApiModel;
import io.swagger.annotations.ApiModelProperty;

@ApiModel(value = "Permissions", description = "Permissions描述")
public class Permissions {
	@ApiModelProperty(value = "pk", example = "pk", required = true)
	private Integer permissionId;

	@ApiModelProperty(value = "权限类型：1菜单，2菜单目录，3按钮，4页面区域", required = true)
	private Integer permissionType;

	@ApiModelProperty(value = "权限名", required = true)
	private String permissionName;

	@ApiModelProperty(value = "权限码", required = true)
	private String permissionCode;

	@ApiModelProperty(value = "父类权限ID", required = true)
	private Integer parentId;

	@ApiModelProperty(value = "url", required = true)
	private String url;

	@ApiModelProperty(value = "数字越大排序越靠前", required = true)
	private Integer permissionSort;

	@ApiModelProperty(value = "1管理后台，2支撑后台", required = true)
	private Integer permissionSystem;

	public Integer getPermissionId() {
		return permissionId;
	}

	public void setPermissionId(Integer permissionId) {
		this.permissionId = permissionId;
	}

	public Integer getPermissionType() {
		return permissionType;
	}

	public void setPermissionType(Integer permissionType) {
		this.permissionType = permissionType;
	}

	public String getPermissionName() {
		return permissionName;
	}

	public void setPermissionName(String permissionName) {
		this.permissionName = permissionName;
	}

	public String getPermissionCode() {
		return permissionCode;
	}

	public void setPermissionCode(String permissionCode) {
		this.permissionCode = permissionCode;
	}

	public Integer getParentId() {
		return parentId;
	}

	public void setParentId(Integer parentId) {
		this.parentId = parentId;
	}

	public String getUrl() {
		return url;
	}

	public void setUrl(String url) {
		this.url = url;
	}

	public Integer getPermissionSort() {
		return permissionSort;
	}

	public void setPermissionSort(Integer permissionSort) {
		this.permissionSort = permissionSort;
	}

	public Integer getPermissionSystem() {
		return permissionSystem;
	}

	public void setPermissionSystem(Integer permissionSystem) {
		this.permissionSystem = permissionSystem;
	}

}
