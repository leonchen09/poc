package com.station.common.utils;

import java.lang.reflect.Field;

/**
 * Created by Jack on 9/7/2017.
 */
public class BeanValueUtils {

    public static  <T> Object getValue(String name, T t) {
        try {
            Field field = t.getClass().getDeclaredField(name);
            field.setAccessible(true);
            return field.get(t);
        } catch (NoSuchFieldException | IllegalAccessException e) {
            throw new IllegalStateException("Read " + name + " failed.", e);
        }
    }

    public static  <T> void bindProperty(String name, Object value, T t) {
        try {
            Field field = t.getClass().getDeclaredField(name);
            field.setAccessible(true);
            field.set(t, value);
        } catch (IllegalAccessException | NoSuchFieldException e) {
            throw new IllegalStateException("Bind " + name + " field failed.", e);
        }
    }
}
