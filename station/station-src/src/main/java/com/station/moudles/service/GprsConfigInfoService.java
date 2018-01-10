package com.station.moudles.service;

import java.io.File;
import java.io.IOException;
import java.util.List;

import org.apache.poi.EncryptedDocumentException;
import org.apache.poi.openxml4j.exceptions.InvalidFormatException;

import com.station.moudles.entity.GprsConfigInfo;
import com.station.moudles.entity.GprsConfigInfoDetail;
import com.station.moudles.entity.GprsConfigInfoStation;
import com.station.moudles.entity.RoutingInspectionStationDetail;
import com.station.moudles.entity.StationInfo;
import com.station.moudles.vo.AjaxResponse;
import com.station.moudles.vo.PulseVo;
import com.station.moudles.vo.search.PageEntity;

public interface GprsConfigInfoService extends BaseService<GprsConfigInfo, Integer> {
	void updateByCompanyId(GprsConfigInfo gprsConfigInfo);

	boolean parseMasterDeviceExcelFile(File file, AjaxResponse ajaxResponse) throws EncryptedDocumentException, InvalidFormatException, IOException;

	GprsConfigInfoDetail selectDetailById(Integer id);

	void updateByGprsSelective(GprsConfigInfo gprsConfigInfo);

	void updateById(GprsConfigInfo gprsConfigInfo);

	boolean parseBackupDeviceExcelFile(File file, AjaxResponse ajaxResponse) throws EncryptedDocumentException, InvalidFormatException, IOException;

	List<GprsConfigInfoStation> selectStationListSelectivePaging(PageEntity pageEntity);

	void createSubDeviceByGprsId(String gprsId);
	//----add 新增主机同时新增24个从机 并且设备类型和规格匹配
	//现在通过设备类型来决定从机的个数
	void createSubDevice(String gprsId,Integer deviceType,String gprsSpec,Integer count,Integer subDeviceCount );

	List selectUnbindGprsList(String gprsId);
	
	//----ADD
	List selectBindGprsList(GprsConfigInfo gprsConfigInfo);

	void updateAndSend(PulseVo pulse);
	//-----app 端修改主机ID
	void updateGprsIdApp(RoutingInspectionStationDetail stationDetail);
	// 修改霍尔感应
	void updateHallFlag(RoutingInspectionStationDetail stationDetail);
	//查询 rcv_time 不等于null 的数据
	List<GprsConfigInfo> selectRcvTimeNotNull(GprsConfigInfo gprsConfigInfo);
}