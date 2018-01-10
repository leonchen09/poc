package com.station.moudles.service.impl;

import com.station.moudles.entity.GprsConfigRead;
import com.station.moudles.mapper.GprsConfigReadMapper;
import com.station.moudles.service.GprsConfigReadService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

@Service
public class GprsConfigReadServiceImpl extends BaseServiceImpl<GprsConfigRead, Integer> implements GprsConfigReadService {
    @Autowired
    GprsConfigReadMapper gprsConfigReadMapper;
}