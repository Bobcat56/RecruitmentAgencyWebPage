using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;

namespace Presentation.Utilities
{
    public class Encryption
    {
        public AsymmetricParameters GenerateAysmmetricKeys()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            AsymmetricParameters myParams = new AsymmetricParameters()
            {
                PublicKey = rsa.ToXmlString(false),
                PrivateKey = rsa.ToXmlString(true)
            };

            return myParams;
        }

        public byte[] AsymmetricEncrypt(byte[] clearBytes, string publicKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(); //Generate another pair of keys
            rsa.FromXmlString(publicKey); //Import the keys created

            byte[] cipher = rsa.Encrypt(clearBytes, true);
            return cipher;
        }

        public byte[] AsymmetricDecrypt(byte[] cipher, string privateKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(); //Generate another pair of keys
            rsa.FromXmlString(privateKey); //Import the keys created

            byte[] originalData = rsa.Decrypt(cipher, true);
            return originalData;
        }

        public MemoryStream HybridEncrypt(byte[] data, string publicKey)
        {
            Aes myAlg = Aes.Create();

            myAlg.GenerateKey();
            myAlg.GenerateIV();

            MemoryStream msIn = new MemoryStream(data); //Point A
            msIn.Position = 0;//ascertain myself that the CryptoStream (later on) will start from position 0

            MemoryStream msOut = new MemoryStream(); // Point B
            using (CryptoStream myCryptoStream = new CryptoStream(msIn, myAlg.CreateEncryptor(), CryptoStreamMode.Read))
            {
                myCryptoStream.CopyTo(msOut);
            }
            msOut.Position = 0;

            //msOut contains my encrypted data
            byte[] encryptedKey = AsymmetricEncrypt(myAlg.Key, publicKey);
            byte[] encryptedIv = AsymmetricEncrypt(myAlg.IV, publicKey);

            MemoryStream output = new MemoryStream();
            output.Write(encryptedKey, 0, encryptedKey.Length); //if here it wrote 128bytes, atm position =128
            output.Write(encryptedIv, 0, encryptedIv.Length);  //it continues writing from position 128

            msOut.CopyTo(output);
            msOut.Flush();

            return output;
        }
        
        public MemoryStream HybridDecrypt(byte[] data, string privateKey)
        {
            MemoryStream msOut = new MemoryStream(data);
            msOut.Position = 0;
         
            byte[] encryptedKey = new byte[128];
            byte[] encryptedIv = new byte[128];

            msOut.Read(encryptedKey, 0, encryptedKey.Length);
            msOut.Read(encryptedIv, 0, encryptedIv.Length);

            byte[] decryptedKey = AsymmetricDecrypt(encryptedKey, privateKey);
            byte[] decryptedIv = AsymmetricDecrypt(encryptedIv, privateKey);

            Aes myAlg = Aes.Create();
            myAlg.Key = decryptedKey;
            myAlg.IV = decryptedIv;

            MemoryStream msIn = new MemoryStream();
            using (CryptoStream myCryptoStream = new CryptoStream(msOut, myAlg.CreateDecryptor(), CryptoStreamMode.Read))
            {
                myCryptoStream.CopyTo(msIn);
            }
            msIn.Position = 0;

            return msIn;
        }

        public byte[] DigitalSign(byte[] data, string privateKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);

            SHA512 myAlg = SHA512.Create();
            byte[] digest = myAlg.ComputeHash(data);

            byte[] signature = rsa.SignHash(digest, new HashAlgorithmName("SHA512"), RSASignaturePadding.Pkcs1);
            return signature;
        }

        public bool DigitalVerification(byte[] data, byte[] signature, string publicKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);

            SHA512 myAlg = SHA512.Create();
            byte[] digest = myAlg.ComputeHash(data);

            bool verification = rsa.VerifyHash(digest, signature, new HashAlgorithmName("SHA512"), RSASignaturePadding.Pkcs1);
            return verification;
        }
    }
}
