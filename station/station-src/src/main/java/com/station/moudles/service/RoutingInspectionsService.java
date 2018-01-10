package com.station.moudles.service;

import java.io.File;
import java.io.IOException;
import java.util.List;

import org.apache.poi.EncryptedDocumentException;
import org.apache.poi.openxml4j.exceptions.InvalidFormatException;

import com.station.moudles.entity.CellInfo;
import com.station.moudles.entity.RoutingInspectionDetail;
import com.station.moudles.entity.RoutingInspectionStationDetail;
import com.station.moudles.entity.RoutingInspections;
import com.station.moudles.entity.RoutingInspectionsStation;
import com.station.moudles.vo.AjaxResponse;
import com.station.moudles.vo.search.SearchStationInfoPagingVo;

public interface RoutingInspectionsService extends BaseService<RoutingInspections, Integer> {
	boolean parseRoutingInspectionExcelFile(File file, AjaxResponse ajaxResponse) throws EncryptedDocumentException, InvalidFormatException, IOException;

	RoutingInspectionStationDetail selectStationDetailByPrimaryKey(Integer pk);

	List<RoutingInspectionsStation> selectStationListSelectivePaging(SearchStationInfoPagingVo searchStationInfoPagingVo);

	void updateRoutingInspectionStationDetail(RoutingInspectionStationDetail routingInspectionStationDetail);

	//查看整治信息
	List<RoutingInspections> selectCellInspace(RoutingInspections routingInspections);
	
}