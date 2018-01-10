package com.station.moudles.helper;

import com.station.moudles.entity.GprsConfigInfo;
import com.station.moudles.entity.PackDataInfo;

/**
 * Created by changbin.li on 9/28/17.
 */
public class LossChargeEvent extends AbstractEvent {

//    private GprsConfigInfo gprsConfigInfo;

    /*public LossChargeEvent(GprsConfigInfo gprsConfigInfo) {
        super("掉电事件", false);
        this.gprsConfigInfo = gprsConfigInfo;
    }*/
    
    public LossChargeEvent(boolean isOneReport) {
    	super("掉电事件", isOneReport);
	}

    @Override
    protected boolean eventCondition(PackDataInfo packDataInfo) {
        return packDataInfo.getGenCur().compareTo(getGprsConfigInfo().getDownCur()) <= 0
                && packDataInfo.getGenVol().compareTo(getGprsConfigInfo().getDownVol()) <= 0;
    }

	/*public void setGprsConfigInfo(GprsConfigInfo gprsConfigInfo) {
		this.gprsConfigInfo = gprsConfigInfo;
	}*/
}
