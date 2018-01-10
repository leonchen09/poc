package com.station.moudles.service.impl;

import com.station.moudles.entity.Company;
import com.station.moudles.entity.User;
import com.station.moudles.mapper.CompanyMapper;
import com.station.moudles.service.CompanyService;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

@Service
public class CompanyServiceImpl extends BaseServiceImpl<Company, Integer> implements CompanyService {
	@Autowired
	CompanyMapper companyMapper;
	@Autowired
	CompanyService companySer;

	@Override
	public int selectListCountSelective(Company company) {
		return companyMapper.selectListCountSelective(company);
	}

	/**
	 * 通过用户查询出对的三级公司
	 */
	@Override
	public List<Map<String,String>> selectCompany1ByUser(String loginId) {
		
		List<Map<String,String>> companyList = new ArrayList<Map<String,String>>();
		Map<String,String> map = new HashMap<>();
		// 装载一级公司
		List<Company> companys1 = new ArrayList<Company>();
		// 装载二级公司
		List<Company> companys2 = new ArrayList<Company>();
		// 装载三级公司
		List<Company> companys3 = new ArrayList<Company>();

		List<Company> company1 = companyMapper.selectCompany1ByUser(loginId);
		// 如果是一级公司
		if (company1.size() != 0) {
			companys1.addAll(company1);// 一级公司装载
			map.put("company1", companys1.get(0).getCompanyName());
			map.put("company2", null);
			map.put("company3", null);
			companyList.add(map);
			return companyList;
		} else {
			// 如果是二级公司
			List<Company> company2 = companyMapper.selectCompany2ByUser(loginId);
			Company company = new Company();
			if (company2.size() != 0) {
				companys2.addAll(company2);// 装载二级公司
				for (Company com2 : company2) {
					company.setCompanyId(com2.getParentCompanyId());
					company.setCompanyLevel(1);
					List<Company> compan1 = companySer.selectListSelective(company);
					if (compan1.size() != 0) {
						companys1.addAll(compan1);
					}
				}
				map.put("company1", companys1.get(0).getCompanyName());
				map.put("company2", companys2.get(0).getCompanyName());
				map.put("company3", null);
				companyList.add(map);
				return companyList;

			} else {
				// 如果是三级公司
				List<Company> company3 = companyMapper.selectCompany3ByUser(loginId);
				if (company3.size() != 0) {
					companys3.addAll(company3);// 装载三级公司
					for (Company com3 : company3) {
						company.setCompanyId(com3.getParentCompanyId());
						company.setCompanyLevel(2);
						List<Company> compan2 = companySer.selectListSelective(company);
						if (compan2.size() != 0) {
							companys2.addAll(compan2);// 装载二级公司
						}
					}
					if (companys2.size() != 0) {
						for (Company com2 : companys2) {
							company.setCompanyId(com2.getParentCompanyId());
							company.setCompanyLevel(1);
							List<Company> compan1 = companySer.selectListSelective(company);
							if (compan1.size() != 0) {
								companys1.addAll(compan1);// 装载一级公司
							}
						}
					}
					map.put("company1", companys1.get(0).getCompanyName());
					map.put("company2", companys2.get(0).getCompanyName());
					map.put("company3", companys3.get(0).getCompanyName());
					companyList.add(map);					
					return companyList;
				}
			}
		}
		return companyList;

	}
}