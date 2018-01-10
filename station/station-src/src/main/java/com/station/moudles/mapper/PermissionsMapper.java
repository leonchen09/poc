package com.station.moudles.mapper;

import java.util.List;

import com.station.moudles.entity.Permissions;

public interface PermissionsMapper extends BaseMapper<Permissions, Integer> {
	List<Permissions> selectListByUserId(Integer userId);

	List<Permissions> selectListByRoleId(Integer roleId);
}
