package com.sofn.framework.datasource.cluster;

import java.util.List;

public interface AvailableRDS {
	
	public String getNextDatasourceKey();

	public void refreshDatasourceKeys(List<String> keys);
	
	public List<String> getDatasourceKeys();
}
