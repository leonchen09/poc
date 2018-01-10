package com.station.moudles.service;

import java.util.List;

import com.station.moudles.entity.PulseDischargeSend;

public interface PulseDischargeSendService extends BaseService<PulseDischargeSend, Integer> {

	void sendPulseDischargeSend(List<PulseDischargeSend> waitPulseDischargeSendList, int maxNum);

	void cleanPulseDischargeSend();
}