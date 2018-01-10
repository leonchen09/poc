package com.station.moudles.mapper;

import java.util.Map;

import org.apache.ibatis.annotations.Param;

import com.station.moudles.entity.SubDevice;

public interface SubDeviceMapper extends BaseMapper<SubDevice, Integer> {
	int selectListCountSelective(SubDevice subDevice);
	//----10/16 add 根据sub_device_id 修改
	void updateByPrimaryKeySelectiveBySubDdviceId(SubDevice subDevice);
	
	//删除多余的从机
	void deleteMoreSubDevice(@Param("map")Map map);
	//根据gprsid修改
	void updateSubTypeByGprsId(SubDevice subDevice);
}