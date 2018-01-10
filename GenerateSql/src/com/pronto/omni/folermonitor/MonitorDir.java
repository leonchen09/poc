package com.pronto.omni.folermonitor;

import java.io.File;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Map;

public class MonitorDir  extends RunThread
{
    /**
     * 
     */
	public Map<String, Long> prevFiles = new HashMap<String, Long>();
    public Map<String, Long> currentFiles = new HashMap<String, Long>();
    
    /**
     * @param second  
     * @param filePath
     */
    public MonitorDir(int second, String filePath) 
    {
        super(second, filePath);
        File dirFile = new File(filePath);
        if(dirFile != null && dirFile.isDirectory())
        {
        	getStartFileInfo(filePath);
        	System.out.println("Start to watch " + dirFile.getAbsolutePath());
        }else{
        	System.err.println("Directory \"" + dirFile + "\" can not found, maybe it's a file or not existed.");
        	System.exit(0);
        }
    }
    
    /**
     * @param dirPath
     */
    public void getStartFileInfo(String dirPath)
    {
        File dirFile = new File(dirPath);
        if(dirFile == null || !dirFile.isDirectory())
        	return;
    
        File[] fileList = dirFile.listFiles();
        for(File tmpFile : fileList)
        {
            if(tmpFile.isFile())
            {
            	prevFiles.put(tmpFile.getAbsolutePath(), tmpFile.lastModified());
            }
            else
            {
                String tmpPath = tmpFile.getAbsolutePath();
                getStartFileInfo(tmpPath);
            }
        }
    }

    public void watchAddUpdate(String dirPath){
        File dirFile = new File(dirPath);
        File[] fileList = dirFile.listFiles();

        for(File tmpFile : fileList)
        {
        	if(tmpFile.isDirectory()){
        		watchAddUpdate(tmpFile.getAbsolutePath());
        	}else{
	        	
	        	currentFiles.put(tmpFile.getAbsolutePath(), tmpFile.lastModified());
	        	
	        	String filePath = tmpFile.getAbsolutePath();
	            Long currentModify = tmpFile.lastModified();
	            if(!prevFiles.containsKey(filePath))
	            {
	            	fileAdded(filePath);
	            	prevFiles.put(filePath, currentModify);
	            }
	            else if(prevFiles.containsKey(filePath))
	            {
	                Long prevModify = prevFiles.get(filePath);
	                if(prevModify.compareTo(currentModify)!=0)
	                {
	                	fileChanged(filePath);
	                	prevFiles.put(filePath, currentModify);
	                }
	            }
	        }
        }
    }

    public void watch(String dirPath)
    {
    	long currentTime = System.currentTimeMillis();
    	watchAddUpdate(dirPath);
    	
        Iterator<String> prevIt = prevFiles.keySet().iterator();
        while(prevIt.hasNext())
        {
            String prevFilePath = prevIt.next();
            if(!currentFiles.containsKey(prevFilePath))
            {
            	fileDeleted(prevFilePath);
            	prevIt.remove();
            }
        }
        currentFiles.clear();
        System.out.println("One loop finished, total time:" + (System.currentTimeMillis() - currentTime)+"ms, current filecount:" + prevFiles.size());
    }

    public void fileAdded(String file)
    {
        System.out.println(file+" is added");
    }
    public void fileChanged(String file)
    {
        System.out.println(file+" is changed");
    }
    public void fileDeleted(String file)
    {
        System.out.println(file+" is deleted");
    }
    
    
    public static void main(String args[])
    {
        MonitorDir md = new MonitorDir(5,args[0]);
        md.onStart();    
    }
    
}
