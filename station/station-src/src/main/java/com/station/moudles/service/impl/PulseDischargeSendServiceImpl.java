package com.station.moudles.service.impl;

import java.util.Date;
import java.util.List;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.station.common.utils.MyDateUtils;
import com.station.moudles.entity.CellInfo;
import com.station.moudles.entity.PulseDischargeSend;
import com.station.moudles.mapper.PulseDischargeSendMapper;
import com.station.moudles.service.CellInfoService;
import com.station.moudles.service.PulseDischargeSendService;

@Service
public class PulseDischargeSendServiceImpl extends BaseServiceImpl<PulseDischargeSend, Integer>
		implements PulseDischargeSendService {
	@Autowired
	PulseDischargeSendMapper pulseDischargeSendMapper;
	@Autowired
	PulseDischargeSendService pulseDischargeSendSer;
	@Autowired
	CellInfoService cellInfoSer;
	private static final Logger logger = LoggerFactory.getLogger(PulseDischargeSendServiceImpl.class);
	

	@Override
	public void sendPulseDischargeSend(List<PulseDischargeSend> waitPulseDischargeSendList, int maxNum) {
		if (waitPulseDischargeSendList.size() > 0) {
			PulseDischargeSend queryPulseDischargeSend = new PulseDischargeSend();
			queryPulseDischargeSend.setSendDone((byte) 0);
			List<PulseDischargeSend> pulseDischargeSendList = selectListSelective(queryPulseDischargeSend);
			queryPulseDischargeSend.setSendDone((byte) 1);
			List<PulseDischargeSend> pulseDischargeSendList1=selectListSelective(queryPulseDischargeSend);
			int sendNum=pulseDischargeSendList.size()+pulseDischargeSendList1.size();
			if (sendNum != 0) {
				logger.debug("特征发送忙!!!!!!!!!!!");
				for(PulseDischargeSend p:waitPulseDischargeSendList){
					CellInfo cell = new CellInfo();
					cell.setCellIndex(p.getPulseCell());
					cell.setGprsId(p.getGprsId());
					List<CellInfo> cellList = cellInfoSer.selectListSelective(cell);
					if (cellList.size() > 0) {
						CellInfo updateCell = new CellInfo();
						updateCell.setId(cellList.get(0).getId());
						updateCell.setPulseSendDone(0);
						cellInfoSer.updateByPrimaryKeySelective(updateCell);
					}
				}
				return;			
			}
			int len = waitPulseDischargeSendList.size();
			for (int i = len - 1; i > len - 1 - maxNum && i >= 0; i--) {
				insertSelective(waitPulseDischargeSendList.get(i));
				CellInfo cell = new CellInfo();
				cell.setCellIndex(waitPulseDischargeSendList.get(i).getPulseCell());
				cell.setGprsId(waitPulseDischargeSendList.get(i).getGprsId());
				List<CellInfo> cellList = cellInfoSer.selectListSelective(cell);
				if (cellList.size() > 0) {
					CellInfo updateCell = new CellInfo();
					updateCell.setId(cellList.get(0).getId());
					updateCell.setPulseSendDone(1);
					cellInfoSer.updateByPrimaryKeySelective(updateCell);
				}
//				waitPulseDischargeSendList.remove(i);
			}
//			if (waitPulseDischargeSendList.size() > 0) {
//				sendPulseDischargeSend(waitPulseDischargeSendList, maxNum);
//			}
		}
	}
	
	@Override
	public void cleanPulseDischargeSend(){
		/*PulseDischargeSend delPulseDischargeSend = new PulseDischargeSend();
		delPulseDischargeSend.setSendDone((byte) 0);
		delPulseDischargeSend.setInsertTime(MyDateUtils.getDiffTime(-5 * 60 * 1000));
		List<PulseDischargeSend> delPulseList = pulseDischargeSendSer.selectListSelective(delPulseDischargeSend);
		for (PulseDischargeSend p : delPulseList) {
			CellInfo cell = new CellInfo();
			cell.setCellIndex(p.getPulseCell());
			cell.setGprsId(p.getGprsId());
			List<CellInfo> cellList = cellInfoSer.selectListSelective(cell);
			if (cellList.size() > 0) {
				CellInfo updateCell = new CellInfo();
				updateCell.setId(cellList.get(0).getId());
				updateCell.setPulseSendDone(6);
				cellInfoSer.updateByPrimaryKeySelective(updateCell);
			}
			PulseDischargeSend updateP = new PulseDischargeSend();
			updateP.setId(p.getId());
			updateP.setSendDone((byte) 6);
			updateP.setEndTime(new Date());
			pulseDischargeSendSer.updateByPrimaryKeySelective(updateP);
		}*/
	}
}