package com.station.moudles.mapper;

import com.station.moudles.entity.PackDataExpandLatest;

import java.util.List;

public interface PackDataExpandLatestMapper extends BaseMapper<PackDataExpandLatest, String> {

    List<PackDataExpandLatest> getByGprsIds(Iterable<String> gprsIds);
}