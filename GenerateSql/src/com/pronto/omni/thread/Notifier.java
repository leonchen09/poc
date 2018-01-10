package com.pronto.omni.thread;

import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.util.logging.Level;
import java.util.logging.Logger;
import java.util.logging.SimpleFormatter;
import java.util.logging.StreamHandler;

public class Notifier {
	
	public static void main(String[] argv) throws Exception{
		Waiter w = new Waiter();
		Lock lock = new Lock();
		w.lock = lock;
		w.start();
		Thread.sleep(1000);
//		synchronized(lock){
//			lock.notify();
//		}
		Object newLock = writeAndReadObj(lock);
		synchronized(newLock){
			newLock.notify();
		}
	}
	
	private static Object writeAndReadObj(Object obj) throws Exception{
		ObjectOutputStream out = new ObjectOutputStream
		(new FileOutputStream("e:\\objectFile.obj"));
		out.writeObject(obj);
		out.close();
		
		ObjectInputStream in = new ObjectInputStream
		(new FileInputStream("e:\\objectFile.obj"));
		Object result = in.readObject();
		return result;
	}
	
	private static Object writeAndReadObjFromDB(Object obj) throws Exception{
		String url = "jdbc:sqlserver://localhost:1433;DatabaseName=app";
		String userName = "pdx";
		String password = "pdx";

		Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");

		Connection conn = DriverManager.getConnection(url, userName, password);
		PreparedStatement ps = conn
				.prepareStatement("select * from testencode where tn = ?");
		
		ps.setObject(1, 1);
		ResultSet rs = ps.executeQuery();
		while(rs.next())
		{
			String name = rs.getString("name");
			String desc = rs.getString("des");
			String remark = rs.getString("remark");
			System.out.println("Row:\r\n"+name+"\t"+desc+"\t"+remark+"\r\n");
		}
		conn.close();
		return null;
	}
}
