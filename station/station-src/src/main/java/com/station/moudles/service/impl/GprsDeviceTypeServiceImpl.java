package com.station.moudles.service.impl;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.station.moudles.entity.GprsDeviceType;
import com.station.moudles.mapper.GprsDeviceTypeMapper;
import com.station.moudles.service.GprsDeviceTypeService;

@Service
public class GprsDeviceTypeServiceImpl extends BaseServiceImpl<GprsDeviceType,Integer> implements GprsDeviceTypeService{
	@Autowired
	GprsDeviceTypeMapper gprsDeviceTypeMapper;
	
	@Override
	public GprsDeviceType selectDevieceType(String typeName) {
		return gprsDeviceTypeMapper.selectDevieceType(typeName);
	}

}
