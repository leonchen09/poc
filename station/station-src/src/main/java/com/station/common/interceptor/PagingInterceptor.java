package com.station.common.interceptor;

import java.lang.reflect.Field;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.List;
import java.util.Properties;

import org.apache.ibatis.executor.parameter.DefaultParameterHandler;
import org.apache.ibatis.executor.parameter.ParameterHandler;
import org.apache.ibatis.executor.statement.RoutingStatementHandler;
import org.apache.ibatis.executor.statement.StatementHandler;
import org.apache.ibatis.mapping.BoundSql;
import org.apache.ibatis.mapping.MappedStatement;
import org.apache.ibatis.mapping.ParameterMapping;
import org.apache.ibatis.plugin.Interceptor;
import org.apache.ibatis.plugin.Intercepts;
import org.apache.ibatis.plugin.Invocation;
import org.apache.ibatis.plugin.Plugin;
import org.apache.ibatis.plugin.Signature;

import com.station.moudles.vo.search.PageEntity;

/**
 *
 * @author zdm mapper文件中的sqlid必须以Paging结尾，执行完sql后recordCount属性里面是总count数
 *         如果是复杂的级联查询，必须用如下方式查询： select tmp.*,b.* from (select * from tablea
 *         where ....) tmp left join tableb b on .....
 *         具体可以参考RoomController中的getRoomtypes方法
 *         为了数据集的规范，现在分页严格区分传入传出对象，只有出入的参数对象继承PageEntity属性，传出的结果对象不继承PageEntity
 * 
 */
@Intercepts({ @Signature(method = "prepare", type = StatementHandler.class, args = { Connection.class }) })
public class PagingInterceptor implements Interceptor {
	/**
	 * 拦截后要执行的方法
	 */
	@Override
	public Object intercept(Invocation invocation) throws Throwable {
		boolean isComplex = false;
		String mainSql = "";
		int beginIndex = 0, endIndex = 0;
		// 我们在PageInterceptor类上已经用@Signature标记了该Interceptor只拦截StatementHandler接口的prepare方法，又因为Mybatis只有在建立RoutingStatementHandler的时候
		// 是通过Interceptor的plugin方法进行包裹的，所以我们这里拦截到的目标对象肯定是RoutingStatementHandler对象。
		final RoutingStatementHandler handler = (RoutingStatementHandler) invocation.getTarget();
		// 通过反射获取到当前RoutingStatementHandler对象的delegate属性
		final StatementHandler delegate = (StatementHandler) ReflectUtil.getFieldValue(handler, "delegate");
		// 获取到当前StatementHandler的
		// boundSql，这里不管是调用handler.getBoundSql()还是直接调用delegate.getBoundSql()结果是一样的，因为之前已经说过了
		// RoutingStatementHandler实现的所有StatementHandler接口方法里面都是调用的delegate对应的方法。
		final BoundSql boundSql = delegate.getBoundSql();
		// 通过反射获取delegate父类BaseStatementHandler的mappedStatement属性
		MappedStatement mappedStatement = (MappedStatement) ReflectUtil.getFieldValue(delegate, "mappedStatement");
		// 只重写需要分页的sql语句。通过MappedStatement的ID匹配，默认重写以Paging结尾的MappedStatement的sql
		String pagingSqlId = ".*Paging$";
		if (mappedStatement.getId().matches(pagingSqlId)) {
			// 拦截到的prepare方法参数是一个Connection对象
			// Connection connection = (Connection) invocation.getArgs()[0];
			// 获取当前要执行的Sql语句，也就是我们直接在Mapper映射语句中写的Sql语句
			final String sql = boundSql.getSql();
			if (sql.indexOf("left join") != -1) {
				isComplex = true;
				beginIndex = sql.indexOf("(") + 1;
				System.out.println(sql.lastIndexOf(")"));
				endIndex = sql.lastIndexOf(")");
				mainSql = sql.substring(beginIndex, endIndex);
			}
			// 拿到当前绑定Sql的参数对象，就是我们在调用对应的Mapper映射语句时所传入的参数对象
			final PageEntity entity = (PageEntity) boundSql.getParameterObject();
			// 重写sql
			final String pageSql = this.getMysqlPageSql(entity, sql, isComplex, mainSql, beginIndex, endIndex);
			// 拦截到的prepare方法参数是一个Connection对象
			Connection connection = (Connection) invocation.getArgs()[0];
			// 给当前的page参数对象设置总记录数 影响性能
			this.setTotalRecord(entity, mappedStatement, connection, boundSql, isComplex, mainSql);
			// 利用反射设置当前BoundSql对应的sql属性为我们建立好的分页Sql语句
			ReflectUtil.setFieldValue(boundSql, "sql", pageSql);
		}
		// 将执行权交给下一个拦截器
		return invocation.proceed();
	}

	/**
	 * 给当前的参数对象page设置总记录数
	 * 
	 * @param page
	 *            Mapper映射语句对应的参数对象
	 * @param mappedStatement
	 *            Mapper映射语句
	 * @param connection
	 *            当前的数据库连接
	 */
	private void setTotalRecord(PageEntity entity, MappedStatement mappedStatement, Connection connection,
			BoundSql boundSql, boolean isComplex, String mainSql) {
		// 获取对应的BoundSql，这个BoundSql其实跟我们利用StatementHandler获取到的BoundSql是同一个对象。
		// delegate里面的boundSql也是通过mappedStatement.getBoundSql(paramObj)方法获取到的。
		// final BoundSql boundSql = mappedStatement.getBoundSql(page);
		// 获取到我们自己写在Mapper映射语句中对应的Sql语句
		final String sql = boundSql.getSql();
		// 通过查询Sql语句获取到对应的计算总记录数的sql语句
		final String countSql = this.getCountSql(sql, isComplex, mainSql);
		// 通过BoundSql获取对应的参数映射
		final List<ParameterMapping> parameterMappings = boundSql.getParameterMappings();
		// 利用Configuration、查询记录数的Sql语句countSql、参数映射关系parameterMappings和参数对象page建立查询记录数对应的BoundSql对象。
		final BoundSql countBoundSql = new BoundSql(mappedStatement.getConfiguration(), countSql, parameterMappings,
				boundSql.getParameterObject());
		// 通过mappedStatement、参数对象page和BoundSql对象countBoundSql建立一个用于设定参数的ParameterHandler对象
		final ParameterHandler parameterHandler = new DefaultParameterHandler(mappedStatement,
				boundSql.getParameterObject(), countBoundSql);
		// 通过connection建立一个countSql对应的PreparedStatement对象。
		PreparedStatement pstmt = null;
		ResultSet rs = null;
		try {
			pstmt = connection.prepareStatement(countSql);
			// 通过parameterHandler给PreparedStatement对象设置参数
			parameterHandler.setParameters(pstmt);
			// 之后就是执行获取总记录数的Sql语句和获取结果了。
			rs = pstmt.executeQuery();
			if (rs.next()) {
				final int totalRecord = rs.getInt(1);
				// 给当前的参数page对象设置总记录数
				entity.setRecordCount(totalRecord);
			}
			if (rs != null) {
				rs.close();
			}
			if (pstmt != null) {
				pstmt.close();
			}
		} catch (SQLException e) {
			e.printStackTrace();
		}
	}

	/**
	 * 根据原Sql语句获取对应的查询总记录数的Sql语句
	 * 
	 * @param sql
	 * @return
	 */
	private String getCountSql(String sql, boolean isComplex, String mainSql) {
		// final int index = sql.toUpperCase().indexOf("FROM");
		// return "select count(*) " + sql.substring(index);
		if (isComplex) {
			return "select count(*) from (" + mainSql + ") as total";
		} else {
			return "select count(*) from (" + sql + ") as total";
		}
	}

	/**
	 * 获取Mysql数据库的分页查询语句
	 * 
	 * @param page
	 *            分页对象
	 * @param sqlBuffer
	 *            包含原sql语句的StringBuffer对象
	 * @return Mysql数据库分页语句
	 */
	private String getMysqlPageSql(PageEntity entity, String sql, boolean isComplex, String mainSql, int beginIndex,
			int endIndex) {
		// 计算第一条记录的位置，Mysql中记录的位置是从0开始的。
		// int offset = (page.getPage().getPageIndex() - 1) *
		// page.getPageSize();
		try {
			StringBuffer sqlBuffer = new StringBuffer();
			if (isComplex) {
				sqlBuffer = new StringBuffer(mainSql);
				sqlBuffer.append(" limit ").append((entity.getPageNo() - 1) * entity.getPageSize()).append(",")
						.append(entity.getPageSize());
				sql = sql.substring(0, beginIndex) + sqlBuffer.toString() + sql.substring(endIndex, sql.length());
				return sql;
			} else {
				sqlBuffer = new StringBuffer(sql);
				sqlBuffer.append(" limit ").append((entity.getPageNo() - 1) * entity.getPageSize()).append(",")
						.append(entity.getPageSize());
				return sqlBuffer.toString();
			}
		} catch (Exception e) {
			System.out.println(sql);
			return sql;
		}
	}

	/**
	 * 拦截器对应的封装原始对象的方法
	 */
	@Override
	public Object plugin(Object target) {
		// 当目标类是StatementHandler类型时，才包装目标类，否者直接返回目标本身,减少目标被代理的
		// 次数
		if (target instanceof StatementHandler) {
			return Plugin.wrap(target, this);
		} else {
			return target;
		}
	}

	/**
	 * 设置注册拦截器时设定的属性
	 */
	@Override
	public void setProperties(Properties properties) {
	}

	/**
	 * 利用反射进行操作的一个工具类
	 * 
	 */
	private static class ReflectUtil {
		/**
		 * 利用反射获取指定对象的指定属性
		 * 
		 * @param obj
		 *            目标对象
		 * @param fieldName
		 *            目标属性
		 * @return 目标属性的值
		 */
		public static Object getFieldValue(Object obj, String fieldName) {
			Object result = null;
			final Field field = ReflectUtil.getField(obj, fieldName);
			if (field != null) {
				field.setAccessible(true);
				try {
					result = field.get(obj);
				} catch (IllegalArgumentException e) {
					e.printStackTrace();
				} catch (IllegalAccessException e) {
					e.printStackTrace();
				}
			}
			return result;
		}

		/**
		 * 利用反射获取指定对象里面的指定属性
		 * 
		 * @param obj
		 *            目标对象
		 * @param fieldName
		 *            目标属性
		 * @return 目标字段
		 */
		private static Field getField(Object obj, String fieldName) {
			Field field = null;
			for (Class<?> clazz = obj.getClass(); clazz != Object.class; clazz = clazz.getSuperclass()) {
				try {
					// 返回一个 Field 对象，该对象反映此 Class 对象所表示的类或接口的指定已声明字段。
					field = clazz.getDeclaredField(fieldName);
					break;
				} catch (NoSuchFieldException e) {
					// 这里不用做处理，子类没有该字段可能对应的父类有，都没有就返回null。
				}
			}
			return field;
		}

		/**
		 * 利用反射设置指定对象的指定属性为指定的值
		 * 
		 * @param obj
		 *            目标对象
		 * @param fieldName
		 *            目标属性
		 * @param fieldValue
		 *            目标值
		 */
		public static void setFieldValue(Object obj, String fieldName, String fieldValue) {
			final Field field = ReflectUtil.getField(obj, fieldName);
			if (field != null) {
				try {
					field.setAccessible(true);
					field.set(obj, fieldValue);
				} catch (IllegalArgumentException e) {
					e.printStackTrace();
				} catch (IllegalAccessException e) {
					e.printStackTrace();
				}
			}
		}
	}
}
