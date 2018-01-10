package com.station.moudles.service.impl;

import com.station.moudles.entity.RawRcvData;
import com.station.moudles.mapper.RawRcvDataMapper;
import com.station.moudles.service.RawRcvDataService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

@Service
public class RawRcvDataServiceImpl extends BaseServiceImpl<RawRcvData, Integer> implements RawRcvDataService {
    @Autowired
    RawRcvDataMapper rawRcvDataMapper;
}