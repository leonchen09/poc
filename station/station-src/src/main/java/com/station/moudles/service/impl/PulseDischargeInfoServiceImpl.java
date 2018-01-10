package com.station.moudles.service.impl;

import com.station.moudles.entity.PulseDischargeInfo;
import com.station.moudles.mapper.PulseDischargeInfoMapper;
import com.station.moudles.service.PulseDischargeInfoService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

@Service
public class PulseDischargeInfoServiceImpl extends BaseServiceImpl<PulseDischargeInfo, Integer> implements PulseDischargeInfoService {
    @Autowired
    PulseDischargeInfoMapper pulseDischargeInfoMapper;
    
 /*  public void update (PulseDischargeInfo pulseDischargeInfo) {
	   pulseDischargeInfoMapper.updateByPrimaryKeySelective(pulseDischargeInfo);
   }*/
}