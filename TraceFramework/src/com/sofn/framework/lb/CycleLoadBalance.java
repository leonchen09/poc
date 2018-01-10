package com.sofn.framework.lb;

import java.util.concurrent.atomic.AtomicInteger;

import org.springframework.stereotype.Component;

@Component
public class CycleLoadBalance implements LoadBalance {

	// 当前使用的标志位，用static，标识每个class loader中只有一份。
	private static AtomicInteger flag = new AtomicInteger(0);

	@Override
	public int getNext(int totalCount) {
		if (totalCount < 1)
			totalCount = 1;
		int cur = Math.abs(flag.getAndIncrement() % totalCount);

		return cur;
	}

}
