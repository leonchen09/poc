package com.station.common.cache;

import com.station.moudles.entity.PackDataInfo;
import com.station.moudles.service.ModelCalculationService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.Collections;
import java.util.Date;
import java.util.List;
import java.util.concurrent.TimeUnit;

/**
 * Created by Jack on 9/16/2017.
 */
@Component
public class DischargeCache extends AbstractCache<String, List<PackDataInfo>> {

    private final ModelCalculationService modelCalculationService;

    @Autowired
    public DischargeCache(ModelCalculationService modelCalculationService) {
        super(3, TimeUnit.DAYS);
        this.modelCalculationService = modelCalculationService;
    }

    @Override
    protected List<PackDataInfo> loadContent(String key) {
        PackDataInfo packDataInfo = modelCalculationService.getLatestDischargeRecord(key, new Date());
        if (packDataInfo == null) {
            return Collections.emptyList();
        }
        List<PackDataInfo> items = modelCalculationService.getContinuousDischargeRecords(packDataInfo, key);
        return items;
    }
    //刷新缓存。
    public void resetContent(String key, List<PackDataInfo> items) {
    	super.put(key, items);
    }
}
