package com.station.moudles.service.impl;

import com.station.moudles.entity.PackDataExpandLatest;
import com.station.moudles.mapper.PackDataExpandLatestMapper;
import com.station.moudles.service.PackDataExpandLatestService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

@Service
public class PackDataExpandLatestServiceImpl extends BaseServiceImpl<PackDataExpandLatest, String> implements PackDataExpandLatestService {
    @Autowired
    PackDataExpandLatestMapper packDataExpandLatestMapper;
}