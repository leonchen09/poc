package com.stepfunction.prontodoc.sx.dao.o10g;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.IOException;
import java.io.Reader;
import java.io.StringWriter;
import java.sql.Connection;
import java.sql.SQLException;

import oracle.sql.CLOB;

public class ClobTest {

	public static CLOB convertClob(String val) throws SQLException, IOException{
		Connection con = (new oracle.jdbc.driver.OracleDriver()).defaultConnection();
		CLOB result = CLOB.createTemporary(con, true, CLOB.DURATION_CALL);
		StringWriter sw = new StringWriter();
		BufferedWriter bw = new BufferedWriter(result.getCharacterOutputStream());
		bw.write(val);
		bw.flush();
		return result;
	}
}
