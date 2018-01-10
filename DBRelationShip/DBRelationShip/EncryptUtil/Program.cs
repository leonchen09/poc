using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace EncryptUtil
{
    class Program
    {
        static void Main() 
        { 
            Console.WriteLine("Encrypt String...");
            txtKey = "prontodocconprontodoccon"; //this is the key to encrypt, it's length must be 24 to compatible with JAVA version.
            btnKeyGen(); 
            Console.WriteLine("Encrypt Key :{0}", txtKey);
            Console.WriteLine();
            string txtEncrypted = EncryptString("server=localhost;uid=pdx; pwd=pdx; database=dbx"); 
            Console.WriteLine("Encrypt String : {0}", txtEncrypted); 
            string txtOriginal = DecryptString(txtEncrypted); 
            Console.WriteLine("Decrypt String : {0}", txtOriginal); 
            Console.ReadLine(); 
        }    
        private static SymmetricAlgorithm mCSP; 
        private static string txtKey; 
        private static void btnKeyGen() 
        { 
            mCSP = SetEnc(); 
            byte[] byt2 = Encoding.UTF8.GetBytes(txtKey); 
            mCSP.Key = byt2; 
        }       
        private static string EncryptString(string Value) 
        { 
            ICryptoTransform ct; 
            MemoryStream ms; 
            CryptoStream cs; 
            byte[] byt;

            mCSP.Mode = System.Security.Cryptography.CipherMode.ECB;
            mCSP.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            ct = mCSP.CreateEncryptor(mCSP.Key, mCSP.IV); 
            byt = Encoding.UTF8.GetBytes(Value); 
            ms = new MemoryStream(); 
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write); 
            cs.Write(byt, 0, byt.Length); 
            cs.FlushFinalBlock(); 
            cs.Close(); 
            return Convert.ToBase64String(ms.ToArray()); 
        }    
        private static string DecryptString(string Value) 
        { 
            ICryptoTransform ct; 
            MemoryStream ms; 
            CryptoStream cs; 
            byte[] byt;
            mCSP.Mode = System.Security.Cryptography.CipherMode.ECB;
            mCSP.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            ct = mCSP.CreateDecryptor(mCSP.Key, mCSP.IV);
            byt = Convert.FromBase64String(Value); 
            ms = new MemoryStream(); 
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write); 
            cs.Write(byt, 0, byt.Length); 
            cs.FlushFinalBlock(); 
            cs.Close(); 
            return Encoding.UTF8.GetString(ms.ToArray()); 
        }    
        private static SymmetricAlgorithm SetEnc() 
        {
            return new TripleDESCryptoServiceProvider(); 
        }
    }
}
