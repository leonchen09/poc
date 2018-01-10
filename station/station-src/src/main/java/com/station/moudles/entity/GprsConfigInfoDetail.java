package com.station.moudles.entity;

import java.util.List;

public class GprsConfigInfoDetail extends GprsConfigInfo {

	private List<SubDevice> subDeviceList;


	/**
	 * @return the subDeviceList
	 */
	public List<SubDevice> getSubDeviceList() {
		return subDeviceList;
	}

	/**
	 * @param subDeviceList the subDeviceList to set
	 */
	public void setSubDeviceList(List<SubDevice> subDeviceList) {
		this.subDeviceList = subDeviceList;
	}

}
