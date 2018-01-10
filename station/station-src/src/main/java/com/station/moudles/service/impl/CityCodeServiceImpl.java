package com.station.moudles.service.impl;

import com.station.moudles.entity.CityCode;
import com.station.moudles.mapper.CityCodeMapper;
import com.station.moudles.service.CityCodeService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

@Service
public class CityCodeServiceImpl extends BaseServiceImpl<CityCode, Integer> implements CityCodeService {
    @Autowired
    CityCodeMapper cityCodeMapper;
}