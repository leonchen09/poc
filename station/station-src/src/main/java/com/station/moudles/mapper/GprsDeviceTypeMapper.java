/*
 * @ClassName gprsDeviceTypeMapper
 * @Description 
 * @version 1.0
 * @Date 2017-12-25 13:45:08
 */
package com.station.moudles.mapper;

import com.station.moudles.entity.GprsDeviceType;

public interface GprsDeviceTypeMapper  extends BaseMapper<GprsDeviceType, Integer>{
	
	GprsDeviceType selectDevieceType(String typeName);
	
	GprsDeviceType selectVolLevelAanCellCount(String gprsId);
}