package com.station.moudles.service.impl;

import com.station.moudles.entity.StationDurationHistory;
import com.station.moudles.mapper.StationDurationHistoryMapper;
import com.station.moudles.service.StationDurationHistoryService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

@Service
public class StationDurationHistoryServiceImpl extends BaseServiceImpl<StationDurationHistory, Integer> implements StationDurationHistoryService {
    @Autowired
    StationDurationHistoryMapper stationDurationHistoryMapper;
}