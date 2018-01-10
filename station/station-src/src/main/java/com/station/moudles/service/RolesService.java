package com.station.moudles.service;

import java.util.List;

import com.station.moudles.entity.Roles;

public interface RolesService extends BaseService<Roles, Integer> {
	List<Roles> selectListByUserId(Integer userId);
}
