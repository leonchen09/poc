package com.station.moudles.service.impl;

import com.station.moudles.entity.CellHistoryInfo;
import com.station.moudles.mapper.CellHistoryInfoMapper;
import com.station.moudles.service.CellHistoryInfoService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

@Service
public class CellHistoryInfoServiceImpl extends BaseServiceImpl<CellHistoryInfo, Integer> implements CellHistoryInfoService {
    @Autowired
    CellHistoryInfoMapper cellHistoryInfoMapper;
}