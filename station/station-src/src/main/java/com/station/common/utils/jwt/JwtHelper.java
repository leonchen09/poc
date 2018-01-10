package com.station.common.utils.jwt;

import java.io.IOException;
import java.security.InvalidKeyException;
import java.security.NoSuchAlgorithmException;
import java.security.SignatureException;
import java.util.Date;
import java.util.HashMap;
import java.util.Map;

/**
 * Token工具类
 * @author minimu
 *
 */
public class JwtHelper {
	final private static String issuer = "station";
	final private static String secret = "^%^&*HBBKKGE$DDFGLMMMHGGR$$%^*U@@#$$%&Y**";
	final private static Algorithm alg = Algorithm.HS256;//加密算法
	final private static long expSec = 60*30;//token超时时间，单位秒，默认30分钟，60＊30
	
	//final private static JWTSigner signer = new JWTSigner(secret);
	//private static JWTVerifier verifier = new JWTVerifier(secret);
	
	/**
	 * 获取token
	 * @param aud 获取token的用户
	 * @return
	 */
	public static String createToken(String aud){
		final long iat = System.currentTimeMillis() / 1000L; // issued at claim 
		final long exp = iat + expSec; // expires claim. In this case the token expires in 60 seconds

		
		final HashMap<String, Object> claims = new HashMap<String, Object>();
		claims.put("iss", issuer);
		claims.put("aud", aud);
		claims.put("exp", exp);
		claims.put("iat", iat);

		return new JWTSigner(secret).sign(claims,new JWTSigner.Options().setAlgorithm(alg));
	}
	
	/**
	 * 验证token
	 * @param token 
	 * @return 返回的Map一定会包含code字段。
	 * 如果验证成功code值为0，为1表示token过期，为9表示其他错误。
	 * 如果验证成功，Map中还包含用户的身份和验证信息，如issb表示tokend的发放机构，aud表示请求的用户，iat表示token获取时间，exp表示过期时间
	 */
	public static Map verifyToken(String token){		
		Map<String, Object> claims = null;
		try {
		    claims= new JWTVerifier(secret).verify(token);
		    claims.put("code", 0);
		} catch (JWTVerifyException | InvalidKeyException | NoSuchAlgorithmException | IllegalStateException | SignatureException | IOException e) {
			claims = new HashMap<String, Object>();
			claims.put("code", (e instanceof JWTExpiredException)?1:9);
		}
		return claims;
	}
	
	/**
	 * 获取时间
	 * @param claims token验证生成的Map数据
	 * @param dateType 1生成时间，2过期时间，
	 * @return 如果Map数据正确返回日期，否则返回null
	 */
	public static Date getDate(Map<String, Object> claims,int dateType){
		if(!claims.get("code").equals(0))return null;
		Date d = new Date();
		d.setTime((int)(dateType==1?claims.get("iat"):claims.get("exp"))*1000L);
		return d;
	}
}
