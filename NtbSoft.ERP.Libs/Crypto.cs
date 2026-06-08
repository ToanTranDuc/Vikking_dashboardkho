using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace NtbSoft.ERP.Libs
{
    public class Crypto
    {
        // Constants used in our AES256 (Rijndael) Encryption / Decryption
        static string initVector = "@1B2c3D4e5F6g7H8";         // Must be 16 bytes
        //static string passPhrase = "!natbi$2014@se";           // Any string
        //static string saltValue = "s@1tValue";                 // Any string
        const string hashAlgorithm = "MD5";             // Can also be "MD5", "SHA1" is stronger
        const int passwordIterations = 2;               // Can be any number, usually 1 or 2       
        const int keySize = 256;                        // Allowed values: 192, 128 or 256


        private static string randomKeyText = string.Empty;

        /// <summary>
        /// Convert random key byte array into a ASCII string
        /// </summary>       
        /// <param name="bytes">Random key byte array to be converted.</param>    
        /// <returns>Random key formatted as a string</returns>       
        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        /// <summary>
        /// Convert random key ASCII string into a byte array 
        /// </summary>       
        /// <param name="randomKeyText">Random key string to be converted.</param>    
        /// <returns>Random key formatted as a byte array</returns>       
        static byte[] GetBytes(string randomKeyText)
        {
            byte[] bytes = new byte[randomKeyText.Length * sizeof(char)];
            System.Buffer.BlockCopy(randomKeyText.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        /// Get the random key string
        /// </summary>            
        /// <returns>Random key formatted as a string</returns>       
        public static string GetRandomKeyText()
        {
            return randomKeyText;
        }

        /// <summary>
        /// Encrypts text using Rijndael symmetric key algorithm and returns base64-encoded result.
        /// </summary>       
        /// <param name="plainText">Plain text data to be encrypted.</param>    
        /// <returns>Encrypted value formatted as a base64-encoded string.</returns>
        public static string Encrypt(string plainText)
        {
            // Create a new Rijndael object.
            Rijndael RijndaelAlg = Rijndael.Create();

            MemoryStream _mStream = new MemoryStream(Convert.FromBase64String(plainText));
            try
            {
                // Create a CryptoStream using the FileStream 
                // and the passed key and initialization vector (IV).
                CryptoStream cStream = new CryptoStream(_mStream,
                    RijndaelAlg.CreateEncryptor(RijndaelAlg.Key, RijndaelAlg.IV),
                    CryptoStreamMode.Write);

                return Convert.ToBase64String(_mStream.ToArray());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _mStream.Flush();
                _mStream.Close();
                _mStream.Dispose();
            }
        }

        /// <summary>
        /// Decrypts text using Rijndael symmetric key algorithm.
        /// </summary>
        /// <param name="cipherText">Base64-formatted text value.</param>
        /// <param name="keyBytes">The public key in byte array format</param>       
        /// <param name="initVector">Vector required to encrypt 1st block of text data, exactly 16 bytes long</param>      
        /// <returns>Decrypted UTF8-encoded string value</returns>       
        public static string Decrypt(string cipherText)
        {
            // Convert strings defining encryption key characteristics into byte
            // arrays. Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8
            // encoding.
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);

            // Convert our ciphertext into a byte array.
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            // Create uninitialized Rijndael encryption object.
            RijndaelManaged symmetricKey = new RijndaelManaged();

            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC;

            // Generate decryptor from existing key bytes and initialization vector. 
            // Key size will be defined based on the number of the key bytes
            byte[] keyBytes = GetBytes((keySize/8).ToString());
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(
                                                             keyBytes,
                                                             initVectorBytes);

            // Define memory stream which will be used to hold encrypted data
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            // Define cryptographic stream (always use Read mode for encryption)
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                          decryptor,
                                                          CryptoStreamMode.Read);

            // We don't know what the size of decrypted data will be, so allocate buffer 
            // long enough to hold ciphertext plaintext is never longer than ciphertext
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            // Start decrypting.
            int decryptedByteCount = cryptoStream.Read(
                                                    plainTextBytes,
                                                    0,
                                                    plainTextBytes.Length);

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert decrypted data into a string. 
            // Let us assume that the original plaintext string was UTF8-encoded.
            string plainText = Encoding.UTF8.GetString(
                                                    plainTextBytes,
                                                    0,
                                                    decryptedByteCount);

            // Return decrypted string  
            return plainText;
        }
    }
}
