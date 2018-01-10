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
	public void updateParameterAll(AppConfigVo appConfig,String parameterCategory) throws IllegalArgumentException, IllegalAccessException {
		List<Field> fieldList = ReflectUtil.getAllFields(AppConfigVo.class);
		for (Field f : fieldList) {
			f.setAccessible(true);
			if (!StringUtils.isNull(f.get(appConfig))) {
				//查询出是否有设备类型和parameterCode相互对应的，如果有就修改，没就新增，有就修改
				if (parameterCategory != null) {
					Parameter parameter = new Parameter();
					String parameterCode = (String)f.getName();
					parameter.setParameterCode(parameterCode);
					parameter.setParameterCategory(parameterCategory);
					List<Parameter>	parameterList = parameterMapper.selectListSelective(parameter);
					if(parameterList.size() == 0) {
						parameter.setParameterValue((String) f.get(appConfig));
						insert(parameter);
					}
				}
				Parameter p = new Parameter();
				if(parameterCategory != null) {
					p.setParameterCategory(parameterCategory);
					p.setParameterCode((String)f.getName());
					p.setParameterValue((String) f.get(appConfig));
					parameterMapper.updateByparameterCategory(p);
				}else {
					p.setParameterCode((String)f.getName());
					p.setParameterValue((String) f.get(appConfig));
					parameterMapper.updateByPrimaryKeySelective(p);
				}
			}
		}
	}

}