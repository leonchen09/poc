package com.station.moudles.service.impl;

import com.station.moudles.entity.GprsConfigSend;
import com.station.moudles.mapper.GprsConfigSendMapper;
import com.station.moudles.service.GprsConfigSendService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

@Service
public class GprsConfigSendServiceImpl extends BaseServiceImpl<GprsConfigSend, Integer> implements GprsConfigSendService {
    @Autowired
    GprsConfigSendMapper gprsConfigSendMapper;
}