package com.station.common.utils;

import org.springframework.beans.BeanUtils;

import com.station.moudles.entity.StationInfo;
import com.station.moudles.vo.CommonSearchVo;
import com.station.moudles.vo.search.SearchStationInfoPagingVo;

public class CommonConvertUtils {
	public static Integer convertStrToStationOperatorType(String operatorTypeStr) {
		operatorTypeStr = operatorTypeStr.trim();
		Integer result;
		switch (operatorTypeStr) {
		case "移动":
			result = 1;
			break;
		case "联通":
			result = 2;
			break;
		case "电信":
			result = 3;
			break;
		default:
			result = 1;
			break;
		}
		return result;
	}

	public static String convertToStationOperatorTypeStr(Integer operatorType) {
		if (operatorType == null) {
			return "";
		}
		String result;
		switch (operatorType) {
		case 1:
			result = "移动";
			break;
		case 2:
			result = "联通";
			break;
		case 3:
			result = "电信";
			break;
		default:
			result = operatorType.toString();
			break;
		}
		return result;
	}

	public static Integer convertDeviceOperateType(String operatorTypeStr) {
		operatorTypeStr = operatorTypeStr.trim();
		Integer result;
		switch (operatorTypeStr) {
		case "首次安装":
			result = 1;
			break;
		case "更换主从机":
			result = 2;
			break;
		case "更换单体":
			result = 3;
			break;
		default:
			result = 1;
			break;
		}
		return result;
	}

	public static void convertCommonToStationPage(CommonSearchVo commonSearchVo,
			SearchStationInfoPagingVo searchStationInfoPagingVo) {
		BeanUtils.copyProperties(commonSearchVo, searchStationInfoPagingVo);
		Integer companyLevel = commonSearchVo.getCompanyLevel();
		if (companyLevel != null) {
			switch (commonSearchVo.getCompanyLevel()) {
			case 1:
				searchStationInfoPagingVo.setCompanyId1(commonSearchVo.getCompanyId());
				break;
			case 2:
				searchStationInfoPagingVo.setCompanyId2(commonSearchVo.getCompanyId());
				break;
			case 3:
				searchStationInfoPagingVo.setCompanyId3(commonSearchVo.getCompanyId());
				break;
			default:
				searchStationInfoPagingVo.setCompanyId3(commonSearchVo.getCompanyId());
				break;
			}
		}
	}

	public static void convertCommonToStation(CommonSearchVo commonSearchVo, StationInfo stationInfo) {
		BeanUtils.copyProperties(commonSearchVo, stationInfo);
		Integer companyLevel = commonSearchVo.getCompanyLevel();
		if (companyLevel != null) {
			switch (commonSearchVo.getCompanyLevel()) {
			case 1:
				stationInfo.setCompanyId1(commonSearchVo.getCompanyId());
				break;
			case 2:
				stationInfo.setCompanyId2(commonSearchVo.getCompanyId());
				break;
			case 3:
				stationInfo.setCompanyId3(commonSearchVo.getCompanyId());
				break;
			default:
				stationInfo.setCompanyId3(commonSearchVo.getCompanyId());
				break;
			}
		} else {
			stationInfo.setCompanyId3(commonSearchVo.getCompanyId());
		}
	}
}
