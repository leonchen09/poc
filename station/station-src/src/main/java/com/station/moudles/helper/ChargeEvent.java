package com.station.moudles.helper;

import com.station.moudles.entity.PackDataInfo;
import org.apache.commons.lang3.StringUtils;

/**
 * Created by changbin.li on 9/28/17.
 */
public class ChargeEvent extends AbstractEvent {

    public ChargeEvent(boolean isOneReport) {
        super("充电", isOneReport);
    }

    @Override
    protected boolean eventCondition(PackDataInfo packDataInfo) {
        return StringUtils.equals(type, packDataInfo.getState());
    }
}
