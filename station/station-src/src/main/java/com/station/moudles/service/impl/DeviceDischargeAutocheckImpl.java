package com.station.moudles.service.impl;

import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.station.moudles.entity.DeviceDischargeAutocheck;
import com.station.moudles.mapper.DeviceDischargeAutocheckMapper;
import com.station.moudles.service.DeviceDischargeAutocheckService;
@Service
public class DeviceDischargeAutocheckImpl extends BaseServiceImpl<DeviceDischargeAutocheck, Integer>
		implements DeviceDischargeAutocheckService {

	@Autowired
	DeviceDischargeAutocheckMapper deviceDischargeAutocheckMapper;
	
	@Override
	public List<DeviceDischargeAutocheck> getIsDevice(DeviceDischargeAutocheck deviceDischargeAutocheck) {
		
		return 	deviceDischargeAutocheckMapper.getIsDevice(deviceDischargeAutocheck);
		
		
	}

}
