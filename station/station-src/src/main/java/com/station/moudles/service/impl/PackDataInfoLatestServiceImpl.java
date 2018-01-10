package com.station.moudles.service.impl;

import com.station.moudles.entity.PackDataInfoLatest;
import com.station.moudles.mapper.PackDataInfoLatestMapper;
import com.station.moudles.service.PackDataInfoLatestService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

@Service
public class PackDataInfoLatestServiceImpl extends BaseServiceImpl<PackDataInfoLatest, String> implements PackDataInfoLatestService {
    @Autowired
    PackDataInfoLatestMapper packDataInfoLatestMapper;
}