package com.stepfunction.prontodoc.sx.sample;

import java.io.FileNotFoundException;
import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;

import javax.servlet.ServletContextEvent;
import javax.servlet.ServletContextListener;

import com.stepfunction.prontodoc.sx.PdxService;
import com.stepfunction.prontodoc.sx.enums.PDX_IPX;
import com.stepfunction.prontodoc.sx.logging.SxLogger;
import com.stepfunction.prontodoc.sx.param.PdxResultSet;
import com.stepfunction.prontodoc.sx.param.PdxpSearchCategory;
import com.stepfunction.prontodoc.sx.param.PdxpStart;
import com.stepfunction.prontodoc.sx.wkl.api.ServiceWKL;

public class PdxListener implements ServletContextListener {
	
	public static final String CONNSTR = "jdbc:sqlserver://localhost;DatabaseName=DBx;user=pdx;password=pdx";
	
	@Override
	public void contextDestroyed(ServletContextEvent arg0) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void contextInitialized(ServletContextEvent arg0) {
		try {
			init();
			testCase_01();
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
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
			while (ServiceWKL.getPdxState() != PDX_IPX.ipx_IsRunning) {
				Thread.sleep(200);
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
