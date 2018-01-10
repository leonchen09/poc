package com.station.moudles.helper;

import com.station.moudles.entity.PackDataInfo;
import org.apache.commons.lang3.StringUtils;

/**
 * Created by changbin.li on 9/28/17.
 */
public class DischargeEvent extends AbstractEvent {

    public DischargeEvent(boolean isOnReport) {
        super("放电", isOnReport);
    }

    @Override
    protected boolean eventCondition(PackDataInfo packDataInfo) {
        return StringUtils.equals(type, packDataInfo.getState());
    }
}
