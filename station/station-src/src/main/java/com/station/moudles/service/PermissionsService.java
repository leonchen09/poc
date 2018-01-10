package com.station.moudles.service;

import java.util.List;

import com.station.moudles.entity.Permissions;

public interface PermissionsService extends BaseService<Permissions, Integer> {
	List<Permissions> selectListByUserId(Integer userId);

	List<Permissions> selectListByRoleId(Integer roleId);
}
