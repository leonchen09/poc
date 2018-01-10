package com.station.adapter.biz;

import java.util.concurrent.LinkedBlockingQueue;

public class WorkQuene {
	//请求的数据队列。
	private static final LinkedBlockingQueue<DataPack> dataQuene = new LinkedBlockingQueue<DataPack>();
	
	public static boolean add(DataPack dataPack) {
		boolean result = dataQuene.add(dataPack);
		return result;
	}
	
	public static DataPack take() throws InterruptedException{
		return dataQuene.take();
	}
	
	public static DataPack peek(){
		return dataQuene.peek();
	}
	
	public static boolean isEmpty() {
		return dataQuene.isEmpty();
	}
	
	//数据包，记录接受的数据和对应的channel id。
	public static class DataPack{
		private String id;
		private byte[] data;
		
		public DataPack() {
			
		}
		
		public DataPack(String id, byte[] data) {
			this.id = id;
			this.data = data;
		}
		
		public String getId() {
			return id;
		}
		public void setId(String id) {
			this.id = id;
		}
		public byte[] getData() {
			return data;
		}
		public void setData(byte[] data) {
			this.data = data;
		}
		
	}
}
