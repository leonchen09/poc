package com.station.moudles.service.impl;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.station.moudles.entity.Company;
import com.station.moudles.entity.StationInfo;
import com.station.moudles.service.CachService;
import com.station.moudles.service.CommonService;

@Service
public class CommonServiceImpl implements CommonService {
	@Autowired
	CachService cachSer;
	private static final Logger logger = LoggerFactory.getLogger(CommonServiceImpl.class);

	@Override
	public boolean completeCompany(StationInfo stationInfo) {
		if (!stationInfo.getCompanyId3().equals(-1)) {
			Company c3 = cachSer.getCompanyById(stationInfo.getCompanyId3());
			if (c3 == null) {
				logger.error("公司不存在！");
				return false;
			} else if (!c3.getCompanyLevel().equals(3)) {
				logger.error("公司不是3级公司！");
				return false;
			}
			stationInfo.setCompanyName3(c3.getCompanyName());
			if (!c3.getParentCompanyId().equals(-1)) {
				Company c2 = cachSer.getCompanyById(c3.getParentCompanyId());
				stationInfo.setCompanyId1(c2.getParentCompanyId());
				stationInfo.setCompanyId2(c2.getCompanyId());
			}
		} else {
			stationInfo.setCompanyId1(-1);
			stationInfo.setCompanyId2(-1);
			stationInfo.setCompanyId3(-1);
			stationInfo.setCompanyName3("");
		}
		return true;
	}
}
