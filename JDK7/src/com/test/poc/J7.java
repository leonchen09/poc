package com.test.poc;

import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.Objects;

import com.test.poc.entity.Obj;

public class J7 {

	public static void main(String[] argv) throws IOException{
		Obj obj = null;
		System.out.println("Objects.nonNull(obj):"+ Objects.nonNull(obj));
		System.out.println("Objects.isNull(obj):"+ Objects.isNull(obj));
		
		//System.out.println(":" + Objects.requireNonNull(obj));
		Path paths = Paths.get("F:\\release\\PureObjectData.txt");
		System.out.println(paths.toAbsolutePath().toString());
		System.out.println(paths.toFile().getName());
		FileInputStream st = new FileInputStream(paths.toFile());
		System.out.println("content:" + st.read());
		st.close();
	}
	
}
