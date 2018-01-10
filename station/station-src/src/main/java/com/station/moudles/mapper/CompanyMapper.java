package com.station.moudles.mapper;

import java.util.List;

import org.apache.ibatis.annotations.Param;
import com.station.moudles.entity.Company;
import com.station.moudles.entity.User;

public interface CompanyMapper extends BaseMapper<Company, Integer> {
	int selectListCountSelective(Company company);
	
	// add 通过用户返回用户对应所以的一级公司	 
	public List<Company> selectCompany1ByUser(@Param("loginId") String loginId); 
	// 二级公司
	public List<Company> selectCompany2ByUser(@Param("loginId") String loginId); 
	// 三级公司
	public List<Company> selectCompany3ByUser(@Param("loginId") String loginId); 
}