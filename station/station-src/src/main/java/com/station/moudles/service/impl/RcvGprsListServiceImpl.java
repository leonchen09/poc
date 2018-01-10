package com.station.moudles.service.impl;

import com.station.moudles.entity.RcvGprsList;
import com.station.moudles.mapper.RcvGprsListMapper;
import com.station.moudles.service.RcvGprsListService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

@Service
public class RcvGprsListServiceImpl extends BaseServiceImpl<RcvGprsList, Integer> implements RcvGprsListService {
    @Autowired
    RcvGprsListMapper rcvGprsListMapper;
}