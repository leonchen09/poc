package com.pronto.omni.reg;

import java.util.prefs.Preferences;

public class ConfigureInfo {

	private static final String ROOT_NODE = "omni apps";

	private static Preferences omniAppsNode;
	
	private boolean currentUser = false;
	
	public ConfigureInfo(){
		init();
	}

	public ConfigureInfo(boolean currentUser){
		this.currentUser = currentUser;
		init();
	}
	
	private void init(){
		if(currentUser){
			//it's location is [HKEY_LOCAL_MACHINE\SOFTWARE\JavaSoft\Prefs\omni apps]
			omniAppsNode = Preferences.userRoot().node(ROOT_NODE);
		}else{
			//it's location is [HKEY_CURRENT_USER\SOFTWARE\JavaSoft\Prefs\omni apps]
			omniAppsNode = Preferences.systemRoot().node(ROOT_NODE);
		}
	}
	
	public static void main(String[] argv){
		ConfigureInfo config = new ConfigureInfo();
		config.setValue("bbb", "aa/d/F/DDD//\\/lkj~fff");
		System.out.println(config.readString("bbb"));
	}
	
	//for test only.
	private void setValue(String keyName, String value){
		Preferences prefs = omniAppsNode;
		prefs.putByteArray(keyName, value.getBytes());
	}
	
	/**
	 * read string value from perference, there is a bug to read string from window
	 * registry, so we convert the string to byte array and save byte array into registry,
	 * then when we read, read byte array and convert it back to string.
	 * @param keyName
	 * @return
	 */
	public String readString(String keyName){
//		return omniAppsNode.get(keyName, null);
		byte[] result = omniAppsNode.getByteArray(keyName, null);
		return new String(result);
	}

	/**
	 * the same with readString(String keyName) except it read key from sub node.
	 * @param subNodeName
	 * @param keyName
	 * @return
	 */
	public String readString(String subNodeName, String keyName){
		Preferences prefs = omniAppsNode.node(subNodeName);
		byte[] result = prefs.getByteArray(keyName, null);
		return new String(result);
//		return prefs.get(keyName, null);
	}

}
