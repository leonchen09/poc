package com.station.moudles.service;



import java.util.List;

import com.station.moudles.entity.DeviceDischargeAutocheck;

public interface DeviceDischargeAutocheckService extends BaseService<DeviceDischargeAutocheck, Integer> {
	
	//查询 is_correct == null 的数据
	List<DeviceDischargeAutocheck> getIsDevice(DeviceDischargeAutocheck deviceDischargeAutocheck); 
}
