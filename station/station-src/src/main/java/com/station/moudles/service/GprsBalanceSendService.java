package com.station.moudles.service;

import com.station.moudles.entity.GprsBalanceSend;

public interface GprsBalanceSendService extends BaseService<GprsBalanceSend, Integer> {

	void save(GprsBalanceSend gprsBalanceSend);

	void send(GprsBalanceSend gprsBalanceSend);

	GprsBalanceSend selectByGprs(String gprsId);
}