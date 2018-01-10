package com.station.moudles.mapper;

import com.station.moudles.entity.SugReplaceCellIndex;

import java.util.List;

public interface SugReplaceCellIndexMapper extends BaseMapper<SugReplaceCellIndex, Integer> {

    List<SugReplaceCellIndex> getLatestByGprsIds(Iterable<String> gprsIds);
}