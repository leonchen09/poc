package com.station.moudles.service.impl;

import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.station.moudles.entity.Roles;
import com.station.moudles.mapper.RolesMapper;
import com.station.moudles.service.RolesService;

@Service
public class RolesServiceImpl extends BaseServiceImpl<Roles, Integer> implements RolesService {

	@Autowired
	RolesMapper rolesMapper;

	@Override
	public List<Roles> selectListByUserId(Integer userId) {
		return rolesMapper.selectListByUserId(userId);
	}

}
