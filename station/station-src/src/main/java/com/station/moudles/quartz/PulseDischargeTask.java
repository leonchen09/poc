package com.station.moudles.quartz;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.BeanUtils;
import org.springframework.beans.factory.annotation.Autowired;

import com.station.common.cache.InitCache;
import com.station.common.utils.ReflectUtil;
import com.station.moudles.entity.CellInfo;
import com.station.moudles.entity.GprsConfigInfo;
import com.station.moudles.entity.Parameter;
import com.station.moudles.entity.PulseDischargeSend;
import com.station.moudles.service.CellInfoService;
import com.station.moudles.service.GprsConfigInfoService;
import com.station.moudles.service.ParameterService;
import com.station.moudles.service.PulseDischargeSendService;

public class PulseDischargeTask {
	private static final Logger logger = LoggerFactory.getLogger(PulseDischargeTask.class);
	@Autowired
	CellInfoService cellInfoSer;
	@Autowired
	GprsConfigInfoService gprsConfigInfoSer;
	@Autowired
	ParameterService parameterSer;
	@Autowired
	PulseDischargeSendService pulseDischargeSendSer;

	public void execute() {
		Date sd = new Date();
		logger.debug("开始特征命令发送任务！！！！！！！" + Thread.currentThread().getName());
		if (InitCache.pulseDischargeProgressFlag) {
			logger.debug("特征命令发送处理中！！！！！！！结束");
			return;
		} else {
			InitCache.pulseDischargeProgressFlag = true;
		}
		logger.debug("处理特征命令发送开始!!!!!!!!!!!");

		/*logger.info("start clean PulseDischargeSend!!!");
		pulseDischargeSendSer.cleanPulseDischargeSend();
		logger.info("clean PulseDischargeSend over!!!");*/

		List<CellInfo> cellList = cellInfoSer.selectWaitForPuls();
		Parameter p = parameterSer.selectByPrimaryKey("PULSE_NUM");
		int maxNum = Integer.parseInt(p.getParameterValue());
		if (cellList.size() > 0) {
			List<PulseDischargeSend> waitPulseDischargeSendList = new ArrayList<PulseDischargeSend>();
			for (CellInfo cell : cellList) {
				CellInfo updateCell = new CellInfo();
				updateCell.setId(cell.getId());
				updateCell.setPulseSendDone(4);
				cellInfoSer.updateByPrimaryKeySelective(updateCell);
				GprsConfigInfo queryGprsConfigInfo = new GprsConfigInfo();
				queryGprsConfigInfo.setGprsId(cell.getGprsId());
				List<GprsConfigInfo> gprsConfigList = gprsConfigInfoSer.selectListSelective(queryGprsConfigInfo);
				if (gprsConfigList.size() > 0) {
					GprsConfigInfo gprsConfigInfo = gprsConfigList.get(0);
					PulseDischargeSend pulseDischargeSend = new PulseDischargeSend();
					BeanUtils.copyProperties(gprsConfigInfo, pulseDischargeSend);
					pulseDischargeSend.setId(null);
					pulseDischargeSend.setGprsId(null);
					if (ReflectUtil.checkBeanNullWithoutSup(pulseDischargeSend, PulseDischargeSend.class)) {
						updateCell.setPulseSendDone(5);
						cellInfoSer.updateByPrimaryKeySelective(updateCell);
					} else {
						pulseDischargeSend.setGprsId(cell.getGprsId());
						pulseDischargeSend.setSendDone((byte) 0);
						pulseDischargeSend.setPulseCell(cell.getCellIndex());
						waitPulseDischargeSendList.add(pulseDischargeSend);
						if (waitPulseDischargeSendList.size() == maxNum) {
							break;
						}
					}
				} else {
					updateCell.setPulseSendDone(5);
					cellInfoSer.updateByPrimaryKeySelective(updateCell);
				}
			}
			pulseDischargeSendSer.sendPulseDischargeSend(waitPulseDischargeSendList, maxNum);
		}
		InitCache.pulseDischargeProgressFlag = false;
		logger.debug("更新状态完成！！！！！！!耗时：" + (new Date().getTime() - sd.getTime()) / 1000 + "s");
	}
}
