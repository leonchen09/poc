/*
 * @ClassName ClassesMapper
 * @Description 
 * @version 1.0
 * @Date 2017-11-23 17:49:57
 */
package com.station.moudles.mapper;

import java.util.List;

import com.station.moudles.entity.DeviceDischargeAutocheck;
import com.station.moudles.entity.DeviceDischargeAutocheckStation;
import com.station.moudles.vo.search.SearchDeviceChargeAutocheckVo;

public interface DeviceDischargeAutocheckMapper extends BaseMapper<DeviceDischargeAutocheck, Integer> {
	//查询是否存在数据
	List<DeviceDischargeAutocheck> getIsDevice(DeviceDischargeAutocheck deviceDischargeAutocheck);
	
	List<DeviceDischargeAutocheckStation> selectListSelectivePagings(SearchDeviceChargeAutocheckVo vo);

}