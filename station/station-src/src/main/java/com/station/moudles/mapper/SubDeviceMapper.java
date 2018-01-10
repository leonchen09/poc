package com.station.moudles.mapper;

import com.station.moudles.entity.SubDevice;

public interface SubDeviceMapper extends BaseMapper<SubDevice, Integer> {
	int selectListCountSelective(SubDevice subDevice);
	//----10/16 add 根据sub_device_id 修改
	void updateByPrimaryKeySelectiveBySubDdviceId(SubDevice subDevice);
}