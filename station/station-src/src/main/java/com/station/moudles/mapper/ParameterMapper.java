package com.station.moudles.mapper;

import com.station.moudles.entity.Parameter;

public interface ParameterMapper extends BaseMapper<Parameter, String> {
	
	void updateByparameterCategory(Parameter parameter);
}