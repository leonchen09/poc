package com.station.moudles.mapper;

import com.station.moudles.entity.GprsBalanceSend;

public interface GprsBalanceSendMapper extends BaseMapper<GprsBalanceSend, Integer> {

	GprsBalanceSend selectByGprs(String gprsId);
}