package com.station.moudles.service.impl;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.station.moudles.entity.Company;
import com.station.moudles.service.CachService;
import com.station.moudles.service.CompanyService;

@Service
public class CachServiceImpl implements CachService {
	@Autowired
	CompanyService companySer;
	
	@Override
	public Company getCompanyById(Integer companyId){
		return companySer.selectByPrimaryKey(companyId);
	}
}
