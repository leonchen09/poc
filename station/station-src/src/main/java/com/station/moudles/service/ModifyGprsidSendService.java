package com.station.moudles.service;

import com.station.moudles.entity.ModifyGprsidSend;

public interface ModifyGprsidSendService extends BaseService<ModifyGprsidSend, Integer> {

    void changeDeviceId(ModifyGprsidSend modifyGprsidSend, int row);
}