package com.station.moudles.service.impl;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.apache.commons.collections.CollectionUtils;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.station.moudles.entity.CellHistoryInfo;
import com.station.moudles.entity.CellInfo;
import com.station.moudles.entity.RoutingInspectionDetail;
import com.station.moudles.entity.RoutingInspectionStationDetail;
import com.station.moudles.mapper.CellHistoryInfoMapper;
import com.station.moudles.mapper.CellInfoMapper;
import com.station.moudles.service.CellInfoService;
import com.station.moudles.service.RoutingInspectionDetailService;

@Service
public class CellInfoServiceImpl extends BaseServiceImpl<CellInfo, Integer> implements CellInfoService {
    @Autowired
    CellInfoMapper cellInfoMapper;
    @Autowired
    CellHistoryInfoMapper cellHistoryInfoMapper;
    @Autowired
	RoutingInspectionDetailService routingInspectionDetailSer;

    private static final Logger logger = LoggerFactory.getLogger(CellInfoServiceImpl.class);

    @Override
    public void updateSendDoneByGprs(List<String> gprsList) {
        int rows = cellInfoMapper.updateSendDoneByGprs(gprsList);
        logger.debug("updateSendDoneByGprs更新条数:" + rows);
    }

    @Override
    public List<CellInfo> selectWaitForPuls() {
        return cellInfoMapper.selectWaitForPuls();
    }

    @Override
    public void updateGprsIdByStationId(CellInfo cellInfo) {
        int rows = cellInfoMapper.updateGprsIdByStationId(cellInfo);
        logger.debug("updateGprsIdByStationId更新条数:" + rows);
    }

    @Override
    public void updateSendDoneByGprsCellIndex(String gprsId, Integer cellIndex, Integer pulseSendDone) {
        Map queryMap = new HashMap();
        queryMap.put("gprsId", gprsId);
        queryMap.put("cellIndex", cellIndex);
        queryMap.put("pulseSendDone", pulseSendDone);
        cellInfoMapper.updateSendDoneByGprsCellIndex(queryMap);
    }

    //app提交更新cellInfo数据--保存原来的数据在cell_history_info 中
	@Override
	public void appUpdateCellInfo(RoutingInspectionStationDetail routingInspectionStationDetail,RoutingInspectionDetail detailList,String cellPlant) {
		CellInfo cellInfo=new CellInfo();
		cellInfo.setGprsId(routingInspectionStationDetail.getGprsId());
		cellInfo.setStationId(routingInspectionStationDetail.getStationId());
		cellInfo.setCellIndex(detailList.getCellIndex());
		List<CellInfo> cellList = cellInfoMapper.selectListSelective(cellInfo);
		if(!CollectionUtils.isEmpty(cellList)) {
			CellInfo cell = cellList.get(0);
			// 电池品牌不为null，则新增巡检记录详情
			if (cellPlant != null) {
				detailList.setDetailOperateValueOld(cell.getCellPlant());
				detailList.setDetailOperateValueNew(cellPlant);
				detailList.setDetailOperateType(6);// 更换单体_电池品牌
				// 新增详情
				routingInspectionDetailSer.insertSelective(detailList);
			}
			//将原来的数据保存在cell_history_info 中
			CellHistoryInfo cellHistory = new CellHistoryInfo();
			cellHistory.setGprsId(cell.getGprsId());
			cellHistory.setCellNumber(cell.getCellIndex());
			cellHistory.setCellPlant(cell.getCellPlant());
			cellHistory.setCellType(detailList.getDetailOperateValueOld());
			cellHistory.setUpdateTime(cell.getUpdateTime());
			cellHistoryInfoMapper.insertSelective(cellHistory);
			//更新cell_info 的数据
			if ("新电池".equals(detailList.getDetailOperateValueNew())) {
				cell.setUseFrom(routingInspectionStationDetail.getOperateTime());
			}
			cell.setUpdateTime(routingInspectionStationDetail.getOperateTime());
			cell.setCellType(detailList.getDetailOperateValueNew());
			cell.setCellPlant(cellPlant);
			cellInfoMapper.updateByPrimaryKey(cell);
			logger.debug("updateByPrimaryKey更新条数--app提交:" + cellList.size());
		}else {
			logger.debug("updateByPrimaryKey更新条数--app提交:" + cellList.size());
		}
	}
}