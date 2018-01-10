package com.station.common.cache;

import java.util.List;
import java.util.concurrent.TimeUnit;

import org.springframework.stereotype.Component;

import com.station.moudles.entity.PackDataInfo;

@Component
public class PackDataCache extends AbstractCache<String, List<PackDataInfo>>{
	
    public PackDataCache() {
        super(12, TimeUnit.HOURS);
    }

    @Override
    protected List<PackDataInfo> loadContent(String key) {
        return null;
    }
    
}
