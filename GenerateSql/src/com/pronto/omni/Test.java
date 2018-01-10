package com.pronto.omni;

import java.util.ArrayList;
import java.util.List;
import java.util.Random;

public class Test {

	List<Table> tables = new ArrayList<Table>();
	List<Column> columns = new ArrayList<Column>();
	List<Tag> tags = new ArrayList<Tag>();

	/**
	 * @param args
	 */
	public static void main(String[] args) {
		Test test = new Test();
		test.mockScope();
		test.mockDefine();
		GenerateSQL generateSQL = new GenerateSQL();
		try {
			ScopedData.getInstance().preProcess();

			Random rand = new Random();
			for (int i = 0; i < 10; i++) {
				String sql = generateSQL.generateSQL(test.mockSelectTag(rand.nextInt(20)+1));
				System.out.println("Generated SQL:\r\n" + sql);
			}
		} catch (Exception e) {
			e.printStackTrace();
		}

	}

	private void mockScope() {
		Table mainTable = new Table();
		// mainTable.setIdxOfMaps((byte)1);
		// mainTable.setJoinClause(" customer cust");
		mainTable.setAlias("cust");
		mainTable.setTableName("customer");
		tables.add(mainTable);

		Table appTable = new Table();
		// appTable.setIdxOfMaps((byte)2);
		// appTable.setJoinClause(" INNER JOIN application app ON app.cust_id = cust.id");
		appTable.setAlias("app");
		appTable.setTableName("application");
		appTable.setDriverTable(mainTable);
		appTable.setJoinColumns(new String[] { "id", "cust_id" });
		appTable.setJoinType(0);
		tables.add(appTable);

		Table addrTable = new Table();
		// addrTable.setIdxOfMaps((byte)4);
		// addrTable.setJoinClause(" LEFT JOIN address addr on addr.cust_id = cust.id");
		addrTable.setAlias("addr");
		addrTable.setTableName("address");
		addrTable.setDriverTable(mainTable);
		addrTable.setJoinColumns(new String[] { "id", "cust_id" });
		addrTable.setJoinType(1);
		tables.add(addrTable);

		Table cityTable = new Table();
		// cityTable.setIdxOfMaps((byte)8);
		// cityTable.setJoinClause(" INNER JOIN city city on city.id = addr.city_id");
		cityTable.setAlias("city");
		cityTable.setTableName("city");
		cityTable.setDriverTable(addrTable);
		cityTable.setJoinColumns(new String[] { "city_id", "id" });
		cityTable.setJoinType(0);
		tables.add(cityTable);

		Column custCode = new Column();
		// custCode.setIdxOfMaps((byte)1);
		// custCode.setFullName("cust.code");
		custCode.setColumnName("code");
		custCode.setOwnerTable(mainTable);
		columns.add(custCode);

		Column custName = new Column();
		// custName.setIdxOfMaps((byte)2);
		// custName.setFullName("cust.name");
		custName.setColumnName("name");
		custName.setOwnerTable(mainTable);
		columns.add(custName);

		Column custSex = new Column();
		// custSex.setIdxOfMaps((byte)4);
		// custSex.setFullName("cust.sex");
		custSex.setColumnName("sex");
		custSex.setOwnerTable(mainTable);
		columns.add(custSex);

		Column custTel = new Column();
		// custTel.setIdxOfMaps((byte)8);
		// custTel.setFullName("cust.tel");
		custTel.setColumnName("tel");
		custTel.setOwnerTable(mainTable);
		columns.add(custTel);

		Column appCode = new Column();
		// appCode.setIdxOfMaps((byte)16);
		// appCode.setFullName("app.code");
		appCode.setColumnName("code");
		appCode.setOwnerTable(appTable);
		appCode.setAliasName("applicationCode");
		columns.add(appCode);

		Column appType = new Column();
		// appType.setIdxOfMaps((byte)32);
		// appType.setFullName("app.type");
		appType.setColumnName("type");
		appType.setOwnerTable(appTable);
		columns.add(appType);

		Column appDesc = new Column();
		// appDesc.setIdxOfMaps((byte)64);
		// appDesc.setFullName("app.[desc]");
		appDesc.setColumnName("[desc]");// NOTICE:the literal "desc" is the
										// keyword of sql, so it should be
										// bracketd wiht "[]".
		appDesc.setOwnerTable(appTable);
		columns.add(appDesc);

		Column appDescription = new Column();
		// appDescription.setIdxOfMaps((byte)-128);
		// appDescription.setFullName("app.description");
		appDescription.setColumnName("description");
		appDescription.setOwnerTable(appTable);
		columns.add(appDescription);

		Column appRemark = new Column();
		// appRemark.setIdxOfMaps((byte)1);
		// appRemark.setFullName("app.remark");
		appRemark.setColumnName("remark");
		appRemark.setOwnerTable(appTable);
		columns.add(appRemark);

		Column cityName = new Column();
		// cityName.setIdxOfMaps((byte)2);
		// cityName.setFullName("city.name");
		cityName.setColumnName("name");
		cityName.setOwnerTable(cityTable);
		columns.add(cityName);

		ScopedData.getInstance().setScopedTables(tables);
		ScopedData.getInstance().setScopedColumns(columns);
		ScopedData.getInstance().setDomainWhere(" cust.id=1");
	}

	private void mockDefine() {
		Tag custCode = new Tag();
		custCode.setColumn(columns.get(0));
		// custCode.setIdxOfColumnMaps(new byte[]{1,0});
		// custCode.setIdxOfTableMaps(new byte[]{1,0});
		tags.add(custCode);

		Tag custName = new Tag();
		custName.setColumn(columns.get(1));
		// custName.setIdxOfColumnMaps(new byte[]{2,0});
		// custName.setIdxOfTableMaps(new byte[]{1,0});
		tags.add(custName);

		Tag custTel = new Tag();
		custTel.setColumn(columns.get(3));
		// custTel.setIdxOfColumnMaps(new byte[]{8,0});
		// custTel.setIdxOfTableMaps(new byte[]{1,0});
		tags.add(custTel);

		Tag appCode = new Tag();
		appCode.setColumn(columns.get(4));
		// appCode.setIdxOfColumnMaps(new byte[]{16,0});
		// appCode.setIdxOfTableMaps(new byte[]{3,0});
		tags.add(appCode);

		Tag appDesc = new Tag();
		appDesc.setColumn(columns.get(6));
		// appDesc.setIdxOfColumnMaps(new byte[]{64,0});
		// appDesc.setIdxOfTableMaps(new byte[]{3,0});
		tags.add(appDesc);

		Tag appDescription = new Tag();
		appDescription.setColumn(columns.get(7));
		// appDescription.setIdxOfColumnMaps(new byte[]{-128,0});
		// appDescription.setIdxOfTableMaps(new byte[]{3,0});
		tags.add(appDescription);

		Tag appRemark = new Tag();
		appRemark.setColumn(columns.get(8));
		tags.add(appRemark);

		Tag cityName = new Tag();
		cityName.setColumn(columns.get(9));
		// cityName.setIdxOfColumnMaps(new byte[]{0,2});
		// cityName.setIdxOfTableMaps(new byte[]{15,0});
		tags.add(cityName);

		ScopedData.getInstance().setTags(tags);
	}

	private List<Tag> mockSelectTag(int selectedCount) {
		List<Tag> result = new ArrayList<Tag>();
		
		System.out.println("------------------begin select tag random, count:" + selectedCount +"----------------");
		Random rand = new Random();
		for (int i = 0; i < selectedCount; i++) {
			result.add(tags.get(rand.nextInt(tags.size() - 1)));
//			System.out.println("Tag:" + result.get(i));
		}

		return result;
	}
}
