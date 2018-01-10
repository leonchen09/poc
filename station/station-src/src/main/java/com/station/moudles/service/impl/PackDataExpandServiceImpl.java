package com.station.moudles.service.impl;

import com.station.moudles.entity.PackDataExpand;
import com.station.moudles.mapper.PackDataExpandMapper;
import com.station.moudles.service.PackDataExpandService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

@Service
public class PackDataExpandServiceImpl extends BaseServiceImpl<PackDataExpand, Integer> implements PackDataExpandService {
    @Autowired
    PackDataExpandMapper packDataExpandMapper;
}