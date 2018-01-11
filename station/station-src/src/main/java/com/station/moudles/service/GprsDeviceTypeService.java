package com.station.moudles.service;

import com.station.moudles.entity.GprsDeviceType;

public interface GprsDeviceTypeService extends BaseService<GprsDeviceType, Integer>{
	//通过设备类型名称查询
	GprsDeviceType selectDevieceType(String typeName);
}
