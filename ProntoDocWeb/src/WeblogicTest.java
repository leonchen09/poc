import java.util.Hashtable;

import weblogic.common.T3ServicesDef;
import weblogic.common.T3StartupDef;


public class WeblogicTest implements T3StartupDef {
		
		public String startup(String name, Hashtable args) throws Exception {
			System.out.println("Startup class running.");
			return "Startup completed successfully";
		}

		@Override
		public void setServices(T3ServicesDef arg0) {
			// TODO Auto-generated method stub
			
		} 
}
