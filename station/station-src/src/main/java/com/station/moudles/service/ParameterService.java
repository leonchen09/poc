package com.station.moudles.service;

import com.station.moudles.entity.Parameter;
import com.station.moudles.vo.AppConfigVo;

public interface ParameterService extends BaseService<Parameter, String> {

	void updateParameterAll(AppConfigVo appConfig) throws IllegalArgumentException, IllegalAccessException;
}