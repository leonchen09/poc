package com.station.moudles.service.impl;

import java.lang.reflect.Field;
import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.station.common.utils.ReflectUtil;
import com.station.common.utils.StringUtils;
import com.station.moudles.entity.Parameter;
import com.station.moudles.mapper.ParameterMapper;
import com.station.moudles.service.ParameterService;
import com.station.moudles.vo.AppConfigVo;

@Service
public class ParameterServiceImpl extends BaseServiceImpl<Parameter, String> implements ParameterService {
	@Autowired
	ParameterMapper parameterMapper;

	@Override
	public void updateParameterAll(AppConfigVo appConfig) throws IllegalArgumentException, IllegalAccessException {
		List<Field> fieldList = ReflectUtil.getAllFields(AppConfigVo.class);
		for (Field f : fieldList) {
			f.setAccessible(true);
			if (!StringUtils.isNull(f.get(appConfig))) {
				Parameter p = new Parameter();
				p.setParameterCode(f.getName());
				p.setParameterValue((String) f.get(appConfig));
				updateByPrimaryKeySelective(p);
			}
		}
	}

}