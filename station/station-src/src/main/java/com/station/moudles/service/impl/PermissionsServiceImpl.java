package com.station.moudles.service.impl;

import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.station.moudles.entity.Permissions;
import com.station.moudles.mapper.PermissionsMapper;
import com.station.moudles.service.PermissionsService;

@Service
public class PermissionsServiceImpl extends BaseServiceImpl<Permissions, Integer> implements PermissionsService {

	@Autowired
	PermissionsMapper permissionsMapper;

	@Override
	public List<Permissions> selectListByUserId(Integer userId) {
		return permissionsMapper.selectListByUserId(userId);
	}

	@Override
	public List<Permissions> selectListByRoleId(Integer roleId) {
		return permissionsMapper.selectListByRoleId(roleId);
	}

}
