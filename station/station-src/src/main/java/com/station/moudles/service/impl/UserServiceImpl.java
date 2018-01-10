package com.station.moudles.service.impl;

import java.util.List;
import java.util.Map;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.station.moudles.entity.Company;
import com.station.moudles.entity.User;
import com.station.moudles.mapper.UserMapper;
import com.station.moudles.service.UserService;
import com.station.moudles.vo.search.PageEntity;
import com.station.moudles.vo.search.SearchUserPagingVo;

@Service
public class UserServiceImpl extends BaseServiceImpl<User, Integer> implements UserService {
    @Autowired
    UserMapper userMapper;
    @Autowired
    UserService userSer;
	
    






	
	
}