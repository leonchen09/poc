package com.reachcloud.demo;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;
import java.net.UnknownHostException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

import org.apache.ibatis.javassist.bytecode.analysis.Executor;

public class TcpClient {

	public static void main(String args[]) {
		ExecutorService executer = Executors.newFixedThreadPool(100);
		for(int i = 0; i < 100; i ++)
			executer.submit(new TcpClientThread());

	}

}
class TcpClientThread implements Runnable{

	@Override
	public void run() {
		Socket socket;
		try {
			socket = new Socket("127.0.0.1",8000);
//			BufferedReader sin=new BufferedReader(new InputStreamReader(System.in));
			PrintWriter os=new PrintWriter(socket.getOutputStream());
			BufferedReader is=new BufferedReader(new InputStreamReader(socket.getInputStream()));
//			String readline;
//			readline=sin.readLine(); //从系统标准输入读入一字符串
//			while(!readline.equals("bye")){
//				os.println(readline);
			while(true){
				os.println("client send data");
				os.flush();
//				System.out.println("Client:"+readline);
//				System.out.println("server:"+ is.readLine());
//				readline=sin.readLine(); //从系统标准输入读入一字符串
			} //继续循环
//			os.close(); //关闭Socket输出流
//			is.close(); //关闭Socket输入流
//			socket.close(); //关闭Socket
		} catch (UnknownHostException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
	}
	
}

