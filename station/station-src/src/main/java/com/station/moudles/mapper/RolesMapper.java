package com.station.moudles.mapper;

import java.util.List;

import com.station.moudles.entity.Roles;

public interface RolesMapper extends BaseMapper<Roles, Integer> {
	List<Roles> selectListByUserId(Integer userId);
}
