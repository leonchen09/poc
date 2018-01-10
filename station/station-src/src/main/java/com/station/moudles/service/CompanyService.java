package com.station.moudles.service;

import java.util.List;
import java.util.Map;

import com.station.moudles.entity.Company;
import com.station.moudles.entity.User;

public interface CompanyService extends BaseService<Company, Integer> {
	int selectListCountSelective(Company temp);
	
	// add 通过用户返回用户对应所以的三级公司	 
	public List<Map<String,String>> selectCompany1ByUser(String loginId); 
}