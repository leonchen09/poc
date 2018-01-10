package com.prontodoc.util;

import java.util.prefs.*;

public class RegRead {


	    public static void main(String[] args) {   
	    	RegRead reg = new RegRead();   
	        reg.writeValue();   
	  
	  }   

	    public void writeValue() {   
	        String[] keys = { "version", "initial", "creator" };   
	        String[] values = { "1.3", "ini.mp3", "www.ifjava.com" };   
	
	        Preferences pre = Preferences.systemRoot().node("");   
	       for (int i = 0; i < keys.length; i++) {   
	            pre.put(keys[i], values[i]);   
	        }   
	   }   
       
	
}
