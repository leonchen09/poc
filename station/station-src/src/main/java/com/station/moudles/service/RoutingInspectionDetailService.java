package com.station.moudles.service;

import java.io.File;
import java.io.IOException;
import java.util.List;
import java.util.TreeMap;

import org.apache.poi.EncryptedDocumentException;
import org.apache.poi.openxml4j.exceptions.InvalidFormatException;
import org.apache.poi.ss.usermodel.Row;

import com.station.moudles.entity.RoutingInspectionDetail;
import com.station.moudles.vo.AjaxResponse;

public interface RoutingInspectionDetailService extends BaseService<RoutingInspectionDetail, Integer> {
	//----10/16 add 显示24个单体详细信息
	List<RoutingInspectionDetail> selectStationSelective(Integer routingId);
	//----10/18 add 查询出app提交到后台的记录
	List<RoutingInspectionDetail> selectListSelectiveApp(RoutingInspectionDetail routingInspectionDetail);
	//----10/26 add巡检记录主机修改 从机修改 标记单体导入 
//	boolean routingInsepectionExcelFile(File file, AjaxResponse ajaxResponse) throws EncryptedDocumentException, InvalidFormatException, IOException;
	
//	boolean routingInsepectionExcelFileCell(File file, AjaxResponse ajaxResponse) throws EncryptedDocumentException, InvalidFormatException, IOException;
	
	void routingInsepectionExcelFile(Row row);

	void routingInsepectionExcelFileCell(Row row);
}