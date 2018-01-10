package com.station.common.utils;

import java.security.Key;
import java.util.Date;
import java.util.HashMap;

import javax.crypto.spec.SecretKeySpec;
import javax.xml.bind.DatatypeConverter;

import com.station.common.utils.jwt.Algorithm;
import com.station.common.utils.jwt.JWTSigner;

import io.jsonwebtoken.JwtBuilder;
import io.jsonwebtoken.Jwts;
import io.jsonwebtoken.SignatureAlgorithm;

public class JwtUtil {

	final private static String issuer = "station";
	final private static String secret = "pronto";
	final private static SignatureAlgorithm alg = SignatureAlgorithm.HS256;//加密算法
	final private static long expSec = 60*30;//token超时时间，单位秒，默认30分钟，60＊30
	
	/**
	 * 获取token
	 * @param aud 获取token的用户
	 * @return
	 */
	public static String createToken(String aud){
		final long iat = System.currentTimeMillis() / 1000L; // issued at claim 
		final long exp = iat + expSec; // expires claim. In this case the token expires in 60 seconds
		Date expDate = new Date(exp);
		
		byte[] apiKeySecretBytes = DatatypeConverter.parseBase64Binary(secret);
		Key signingKey = new SecretKeySpec(apiKeySecretBytes, alg.getJcaName());
		final HashMap<String, Object> claims = new HashMap<String, Object>();
		claims.put("iss", issuer);
		claims.put("aud", aud);
		claims.put("exp", exp);
		claims.put("iat", iat);

		JwtBuilder builder = Jwts.builder().setHeader(claims);
		builder.setExpiration(expDate);
		
		return builder.compact();
	}
	
	public static void main(String[] argv) {
		System.out.println("sin:"+ new JwtUtil().createToken("tieta"));
	}
	
}
