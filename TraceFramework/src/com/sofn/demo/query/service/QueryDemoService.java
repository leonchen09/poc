package com.sofn.demo.query.service;

import com.sofn.demo.entity.Demo;
import com.sofn.framework.base.entity.PaginationSearch;

public interface QueryDemoService {
	PaginationSearch<Demo> readList(PaginationSearch<Demo> parameter);
}
