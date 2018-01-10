package com.station.moudles.service;

import java.io.File;
import java.io.IOException;

import org.apache.poi.EncryptedDocumentException;
import org.apache.poi.openxml4j.exceptions.InvalidFormatException;

import com.station.moudles.entity.RoutingInspectionStationDetail;
import com.station.moudles.entity.SubDevice;
import com.station.moudles.vo.AjaxResponse;

public interface SubDeviceService extends BaseService<SubDevice, Integer> {
	void changeSubDevice(SubDevice subDevice);

	boolean parseSubDeviceExcelFile(File file, AjaxResponse ajaxResponse) throws EncryptedDocumentException, InvalidFormatException, IOException;
	
	int selectListCountSelective(SubDevice subDevice);
	//---10/16 add 根据sub_device_id 修改 
	void updateByPrimaryKeySelectiveBySubDdviceId(SubDevice subDevice);
	// app端修改从机外部id
	void updateSubDevice(RoutingInspectionStationDetail routingInspectionStationDetail,int row) throws Exception;
}