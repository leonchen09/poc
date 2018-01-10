package com.station.common.spring.mybatis;

import java.util.Properties;

import com.alibaba.druid.util.DruidPasswordCallback;
import com.station.common.utils.ThreeDES;

public class DBPwdUtil extends DruidPasswordCallback {
	
	public void setProperties(Properties properties) {
        super.setProperties(properties);
        String pwd = properties.getProperty("password");
        if (pwd != null && pwd.length() > 1) {
            try {
                //这里的password是将jdbc.properties配置得到的密码进行解密之后的值
                //所以这里的代码是将密码进行解密
                String password = ThreeDES.decrypt(pwd); 
                setPassword(password.toCharArray());
            } catch (Exception e) {
                setPassword(pwd.toCharArray());
            }
        }
    }


}
