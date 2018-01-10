package com.reachcloud.framework.base.dao;


public interface PubSeqMapper {
	//设置并返回Oracle数据库自增id值
	public int selectSeq(String seqName);
	
}
