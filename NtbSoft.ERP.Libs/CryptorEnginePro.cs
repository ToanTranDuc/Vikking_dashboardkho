using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace NtbSoft.ERP.Libs
{
    public class CryptorEnginePro
    {
        //private static string key = "Pas5pr@se";  // can be any string
        private static int[] arrKey = { 11, 22, 31, 21, 23, 82, 39, 48, 57, 37, 77, 67, 63, 91, 12, 25, 56, 82, 18, 89, 55, 15, 51, 53, 69, 28, 92, 94, 96, 98, 97, 83, 89, 86, 84, 76, 73, 72, 36, 75 };
        private static string saltValue = "s@1tValue@Line@Name";  // can be any string
        private static string hashAlgorithm = "SHA1";   // can be "MD5"
        private static int passwordIterations = 8;      // can be any number
        private static string initVector = "@CSS@CSS@CSS@CSS"; // must be 16 bytes
        private static int keySize = 256;

        public static bool HasCompare(string plainText, string cipherText)
        {
            if (Decrypt(cipherText).Equals(plainText))
                return true;

            return false;
        }
        public static string GetRendomValue()
        {
            Random random = new Random();
            string r = "";
            int i;
            for (i = 1; i < 11; i++)
            {
                r += arrKey[random.Next(0, 39)].ToString();
            }
            return r;
        }
        public static bool HasCompareEncrypt(string cipherText, string cipherText1)
        {
            if (Decrypt(cipherText).Equals(Decrypt(cipherText1)))
                return true;

            return false;
        }

        public static string Encrypt(string plainText)
        {
            System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
            //Get your key from config file to open the lock!
            //string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));
            string key = "A93reRTUJHsCuQSHR+L3GxqOJyDmQpCgps102ciuabc=";
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            PasswordDeriveBytes password = new PasswordDeriveBytes(
                                                            key,
                                                            saltValueBytes,
                                                            hashAlgorithm,
                                                            passwordIterations);

            byte[] keyBytes = password.GetBytes(keySize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();

            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(
                                                             keyBytes,
                                                             initVectorBytes);

            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                         encryptor,
                                                         CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();

            byte[] cipherTextBytes = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();
            string cipherText = Convert.ToBase64String(cipherTextBytes);

            return cipherText;
        }

        public static string Decrypt(string cipherText)
        {
            if (!IsBase64String(cipherText))
                throw new Exception("Invalid character in a Base-64 string");

            try
            {
                System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
                //Get your key from config file to open the lock!
                //string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));
                string key = "A93reRTUJHsCuQSHR+L3GxqOJyDmQpCgps102ciuabc=";
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);

                // byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

                // Convert our ciphertext into a byte array.
                byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

                PasswordDeriveBytes password = new PasswordDeriveBytes(key, saltValueBytes, hashAlgorithm, passwordIterations);

                byte[] keyBytes = password.GetBytes(keySize / 8);

                RijndaelManaged symmetricKey = new RijndaelManaged();
                symmetricKey.Mode = CipherMode.CBC;
                symmetricKey.Padding = PaddingMode.None;

                ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
                MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

                byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                memoryStream.Close();
                cryptoStream.Close();
                string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

                // Return decrypted string.
                return plainText;
            }
            catch (Exception e)
            {
                throw new InvalidDataException("[" + e.ToString() + " : " + e.Message + "]  Data corrupt");
            }
        }

        public static bool IsBase64String(string base64String)
        {
            base64String = base64String.Trim();
            return (base64String.Length % 4 == 0) && 
                System.Text.RegularExpressions.Regex.IsMatch(base64String, @"^[a-zA-Z0-9\+/]*={0,3}$",
                System.Text.RegularExpressions.RegexOptions.None);

        }
    }
}
