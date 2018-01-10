package com.station.common.utils;

import org.junit.Test;

import com.google.common.collect.HashBiMap;

import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.TimeUnit;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotNull;

/**
 * Created by changbin.li on 8/30/17.
 */
public class ConvertUtilsTest {

    @Test
    public void separateStringIntoDoubleList() {
        String value = "-294.17--294.19--294.17--294.23";
        List<Double> values = ConvertUtils.separateStringIntoDoubleList(value);
        assertNotNull(values);
        assertEquals(values.size(), 4);
        assertEquals(values.get(0).toString(), "-294.17");
        assertEquals(values.get(1).toString(), "-294.19");
        assertEquals(values.get(2).toString(), "-294.17");
        assertEquals(values.get(3).toString(), "-294.23");

        value = "294.17-294.19--294.17-294.23";
        values = ConvertUtils.separateStringIntoDoubleList(value);
        assertNotNull(values);
        assertEquals(values.size(), 4);
        assertEquals(values.get(0).toString(), "294.17");
        assertEquals(values.get(1).toString(), "294.19");
        assertEquals(values.get(2).toString(), "-294.17");
        assertEquals(values.get(3).toString(), "294.23");
    }


    @Test
    public void test() throws InterruptedException {
        ExecutorService fixedThreadPool = Executors.newSingleThreadExecutor();
        fixedThreadPool.execute(() -> {
            try {
                Thread.sleep(TimeUnit.SECONDS.toMillis(5));
            } catch (InterruptedException e) {
            }
            System.out.println("1--》OK!" + System.currentTimeMillis());
        });
        fixedThreadPool.execute(() -> {
            System.out.println("2--》OK!" + System.currentTimeMillis());
        });
        fixedThreadPool.execute(() -> {
            System.out.println("3--》OK!" + System.currentTimeMillis());
        });
        fixedThreadPool.execute(() -> {
            System.out.println("4--》OK!" + System.currentTimeMillis());
        });

        fixedThreadPool.awaitTermination(10, TimeUnit.SECONDS);
    }
    
}
