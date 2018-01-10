package com.stepfunction.prontodoc.sx.sample;

import java.io.FileNotFoundException;
import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;

import com.stepfunction.prontodoc.sx.PdxService;
import com.stepfunction.prontodoc.sx.enums.PDX_IPX;
import com.stepfunction.prontodoc.sx.logging.SxLogger;
import com.stepfunction.prontodoc.sx.param.PdxResultSet;
import com.stepfunction.prontodoc.sx.param.PdxpSearchCategory;
import com.stepfunction.prontodoc.sx.param.PdxpStart;

public class PdxStart {

	public static final String CONNSTR = "jdbc:sqlserver://localhost;DatabaseName=DBx;user=pdx;password=pdx";
	/**
	 * @param args
	 * @throws Exception 
	 */
	public static void main(String[] args) throws Exception {
		init();
		testCase_01();

	}
	private static void init() throws Exception {
		String testCaseName = Thread.currentThread().getStackTrace()[1].getMethodName();
		try {
			PdxService pdxService = new PdxService();
			PdxpStart startParam = new PdxpStart();
			startParam.setConnectToAppDB(CONNSTR);
			startParam.setConnectionPoolSize(3);
			startParam.setRenderDocumentTasks(2);
			startParam.setTemplateRepository(true);
			startParam.setDiscardRenderRequests(false);

			if (pdxService.pdx_Start(startParam)) {
				System.out.println("Connect To DB Successful");
			} else {
				System.out.println("Connect To DB Failed");
			}
		} catch (Exception ex) {
			ex.printStackTrace();
		}
	}

	/* Test GroupCountOnly */
	public static void testCase_01() throws FileNotFoundException, IOException {
		String testCaseName = Thread.currentThread().getStackTrace()[1].getMethodName();

		try {
			System.out.println(String.format("Testing: %s", testCaseName));
			PdxService pdxService = new PdxService();

			long searchResult = PDX_IPX.ipx_GroupCountOnly;
			String collation = "Category";

			PdxpSearchCategory param = new PdxpSearchCategory(searchResult, collation);
			PdxResultSet pdxResultSet = pdxService.pdx_SearchCategory(param);
			getRecordset(pdxResultSet.getRecordset());
			SxLogger.getLogger().debug(testCaseName + "Succeed.");
		} catch (Exception ex) {
			ex.printStackTrace();
		}
	}
	
	private static void getRecordset(ArrayList<HashMap<String, Object>> array) {
		for (int i = 0; i < array.size(); i++) {
			System.out.println(array.get(i));
		}
	}
}
