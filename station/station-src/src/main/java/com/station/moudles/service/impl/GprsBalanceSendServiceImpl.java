package com.station.moudles.service.impl;

import java.sql.Date;
import java.util.Calendar;
import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.station.moudles.entity.GprsBalanceSend;
import com.station.moudles.entity.PulseDischargeInfo;
import com.station.moudles.mapper.GprsBalanceSendMapper;
import com.station.moudles.mapper.PulseDischargeInfoMapper;
import com.station.moudles.service.GprsBalanceSendService;

@Service
public class GprsBalanceSendServiceImpl extends BaseServiceImpl<GprsBalanceSend, Integer>
		implements GprsBalanceSendService {
	@Autowired
	GprsBalanceSendMapper gprsBalanceSendMapper;
	@Autowired
	PulseDischargeInfoMapper pulseDischargeInfoMapper;

	@Override
	public void save(GprsBalanceSend gprsBalanceSend) {
		gprsBalanceSend.setSendDone((byte) -1);
		if (gprsBalanceSend.getId() != null) {

			updateByPrimaryKeySelective(gprsBalanceSend);
		} else {
			GprsBalanceSend queryGprsBalanceSend = new GprsBalanceSend();
			queryGprsBalanceSend.setGprsId(gprsBalanceSend.getGprsId());
			queryGprsBalanceSend.setSendDone((byte) -1);

			List<GprsBalanceSend> gprsBalanceSendList = selectListSelective(queryGprsBalanceSend);
			if (gprsBalanceSendList.size() > 0) {
				gprsBalanceSend.setId(gprsBalanceSendList.get(0).getId());
				updateByPrimaryKeySelective(gprsBalanceSend);
			} else {
				insertSelective(gprsBalanceSend);
			}
		}
	}

	@Override
	public void send(GprsBalanceSend gprsBalanceSend) {
		Calendar calendar = Calendar.getInstance();
		gprsBalanceSend.setSendDone((byte) 0);
		gprsBalanceSend.setId(null);
		gprsBalanceSend.setSendTime(calendar.getTime());
		insertSelective(gprsBalanceSend);
		//主机有时会失败，所以插入两条数据时间间隔1分钟
		calendar.add(Calendar.MINUTE, 1);
		gprsBalanceSend.setSendTime(calendar.getTime());
		insertSelective(gprsBalanceSend);
	}
	
	@Override
	public GprsBalanceSend selectByGprs(String gprsId) {
		return gprsBalanceSendMapper.selectByGprs(gprsId);
	}

}