package com.station.common.utils;

import com.google.common.collect.Lists;

import java.util.Collections;
import java.util.List;

import static org.apache.commons.lang3.StringUtils.isBlank;
import static org.apache.commons.lang3.StringUtils.substring;

public class ConvertUtils {

    public final static char SEPARATER = '-';

    public static Double toDouble(String value, Double defaultValue) {
        if (isBlank(value)) {
            return defaultValue;
        }
        value = org.apache.commons.lang3.StringUtils.trimToEmpty(value);
        try {
            return Double.parseDouble(value);
        } catch (Exception e) {
            return defaultValue;
        }
    }

    /**
     * <pre>
     *     e.g.
     *     intput: -1--2-3--4
     *     output: -1 -2 3 -4
     * </pre>
     *
     * @param value
     * @return
     */
    public static List<Double> separateStringIntoDoubleList(String value) {
        if (isBlank(value)) {
            return Collections.emptyList();
        }
        List<Double> values = Lists.newArrayList();
        char[] chars = value.toCharArray();
        int length = chars.length;
        int start = 0;
        char previous = ' ';
        for (int i = 0; i < length; i++) {
            char val = chars[i];
            if (i > 0 && val == SEPARATER && previous != SEPARATER) {
                values.add(Double.parseDouble(substring(value, start, i)));
                start = i + 1;
            }
            previous = val;
        }
        values.add(Double.parseDouble(substring(value, start)));
        return values;
    }
}
