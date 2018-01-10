package com.pronto.jdbc;

public class StringTest {

	public static void main(String[] argv){
		//testStr();
		testSb();
	}
	
	private static void testStr(){
		String s = "abcdefghij0123456789abcdefghij0123456789abcdefghij0123456789abcdefghij0123456789abcdefghij0123456789";
		System.out.println("JVM MAX MEMORY:" + Runtime.getRuntime().maxMemory()/1024/1024+"M");
		System.out.println("JVM USING MEMORY:" + Runtime.getRuntime().totalMemory()/1024/1024+"M");
		Runtime.getRuntime().traceMethodCalls(true);
		while(true){
			try{
				s += s;
			}catch(Exception ex){
				System.err.println(ex.getMessage());
			}
			catch(Error o){
				String unit = null;
				int sizeb = s.length();
				int time = 0;
				float size = sizeb;
				while(size > 1024){
					size = size/1024;
					time++;
				}
				switch(time){
				case 0: unit = "byte";break;
				case 1: unit = "k"; break;
				case 2: unit = "m"; break;
				case 3: unit = "g"; break;
				}
				System.out.println("String has used memory:" + size + unit + ", length:"+sizeb);
				System.out.println("Now, JVM USING MEMORY:" + Runtime.getRuntime().totalMemory()/1024/1024+"M");
				System.err.println("MemoryError: " + o);
				break;
			}
		}
	}
	
	private static void testSb(){
		String s = "abcdefghij0123456789abcdefghij0123456789abcdefghij0123456789abcdefghij0123456789abcdefghij0123456789";
		System.out.println("JVM MAX MEMORY:" + Runtime.getRuntime().maxMemory()/1024/1024+"M");
		System.out.println("JVM USING MEMORY:" + Runtime.getRuntime().totalMemory()/1024/1024+"M");
		Runtime.getRuntime().traceMethodCalls(true);
		StringBuffer sb = new StringBuffer();
		int i = 0;
		while( i < 3097152){
			i ++;
			sb.append(s);
		}
		try{
			s = sb.toString();
		}catch(Exception ex){
			System.err.println(ex.getMessage());
		}
		catch(Error o){
			System.err.println("MemoryError: " + o);
		}finally{
			String unit = null;
			int sizeb = s.length();
			int time = 0;
			float size = sizeb;
			while(size > 1024){
				size = size/1024;
				time++;
			}
			switch(time){
			case 0: unit = "byte";break;
			case 1: unit = "k"; break;
			case 2: unit = "m"; break;
			case 3: unit = "g"; break;
			}
			System.out.println("String has used memory:" + size + unit + ", length:"+sizeb);
			System.out.println("Now, JVM USING MEMORY:" + Runtime.getRuntime().totalMemory()/1024/1024+"M");
		}
		
	}
}
