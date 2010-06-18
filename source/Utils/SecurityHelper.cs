namespace AtomSite.Utils
{
  using System;
  using System.Globalization;
  using System.IO;
  using System.Security.Cryptography;
  using System.Text;
  using System.Security.Principal;
  using System.Linq;
  using System.Security;

  public static class SecurityHelper
  {    
    //NOTE: For use in the PasswordEncrypt and PasswordDecrypt members only.
    //Since we're accepting a user created clear text password - it's  unlikey that the password
    //will be more than 128 bits long, or 16 bytes (although it's possibe). The default AES block size
    //is 128 bits and so even though the mode of encryption is CBC - we'll amlost never encrypt more than a single block.
    //Even if we were encrypting multiple blocks, the IV does not have to be a secret - so it's hard
    //coded here for now (although in the future it should be moved to an application specific configuration setting
    //along with the full length symmetric key).
    private static readonly string hexIv = "A9DCF37AED8574A1441FD82DB743765A";

    /// <summary>
    /// Hash utility - pass the hash algorithm name as a string i.e. SHA256, SHA1, MD5 etc.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="algorithm"></param>      
    /// <returns>Hashed String</returns>
    public static string HashIt(string input, string algorithm)
    {
      return HashIt(input, algorithm, true);
    }

    /// <summary>
    /// Hash utility - pass the hash algorithm name as a string i.e. SHA256, SHA1, MD5 etc.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="algorithm"></param>
    /// <param name="upperCase"></param>
    /// <returns>Hashed String</returns>
    public static string HashIt(string input, string algorithm, bool upperCase)
    {

      if (input == null) //changed by JV
        throw new ArgumentNullException("input");

      if (string.IsNullOrEmpty(algorithm))
        throw new ArgumentNullException("algorithm");

      byte[] result = ((HashAlgorithm)CryptoConfig.CreateFromName(algorithm)).ComputeHash(Encoding.UTF8.GetBytes(input));

      if (upperCase)
        return BitConverter.ToString(result).Replace("-", "").ToUpper();

      return BitConverter.ToString(result).Replace("-", "").ToLower();
    }

    //public static string ToMd5Hash(string val)
    //{
    //    if (val == null) throw new ArgumentNullException("val");

    //    byte[] data = Encoding.ASCII.GetBytes(val);
    //    data = MD5.Create().ComputeHash(data);
    //    string ret = "";
    //    for (int i = 0; i < data.Length; i++)
    //        ret += data[i].ToString("x2").ToLower();
    //    return ret;
    //}

    /// <summary>
    /// Helper method to encrypt an account password. Password encryption must be 'reversable' in
    /// order to support WSSE authentication. Atom-enabled clients using WSSE will be performing the WSSE digest 
    /// on the client (not on the server) and so we must be able to recover the user's password in order
    /// to verfify the WSSE digest.
    /// </summary>
    /// <param name="clearTextPassword"></param>
    /// <param name="hexSymmetricKey"></param>
    /// <returns></returns>
    public static string PasswordEncrypt(string clearTextPassword, string hexSymmetricKey)
    {
      if (clearTextPassword == null) //In theory it could be an empty string
        throw new ArgumentNullException("clearTextPassword");

      byte[] cipherbytes = SymmetricEncrypt(Encoding.UTF8.GetBytes(clearTextPassword), HexToByteArray(hexSymmetricKey), HexToByteArray(hexIv));
      return HexFromByteArray(cipherbytes);

    }

    /// <summary>
    /// Helper method to decrypt an account password. Password encryption must be 'reversable' in
    /// order to support WSSE authentication. Atom-enabled clients using WSSE will be performing the WSSE digest 
    /// on the client (not on the server) and so we must be able to recover the user's password in order
    /// to verfify the WSSE diget.
    /// </summary>
    /// <param name="encryptedPasswod"></param>
    /// <param name="hexSymmetricKey"></param>
    /// <returns></returns>
    public static string PasswordDecrypt(string encryptedPasswod, string hexSymmetricKey)
    {
      if (encryptedPasswod == null) //In theory it could be an empty string
        throw new ArgumentNullException("encryptedPasswod");

      byte[] clearBytes = SymmetricDecrypt(HexToByteArray(encryptedPasswod), HexToByteArray(hexSymmetricKey), HexToByteArray(hexIv));

      return Encoding.UTF8.GetString(clearBytes);
    }


    /// <summary>
    /// Helper method to encrypt a byte array using AES. (Albahari C# 3.0 In a Nutshell)
    /// </summary>
    /// <param name="clearBytes"></param>
    /// <param name="key"></param>
    /// <param name="iv"></param>
    /// <returns></returns>
    public static byte[] SymmetricEncrypt(byte[] clearBytes, byte[] key, byte[] iv)
    {
      if (clearBytes == null)
        throw new ArgumentNullException("clearBytes");

      if (key == null)
        throw new ArgumentNullException("key");

      if (iv == null)
        throw new ArgumentNullException("iv");

      //Aes 
      using (SymmetricAlgorithm algorithm = Aes.Create())
      using (ICryptoTransform decryptor = algorithm.CreateEncryptor(key, iv))
        return SymmetricTransform(clearBytes, key, iv, decryptor);
    }



    /// <summary>
    /// Helper method to decrypt a byte array using using AES. (Albahari C# 3.0 In a Nutshell)
    /// </summary>
    /// <param name="cipherBytes"></param>
    /// <param name="key"></param>
    /// <param name="iv"></param>
    /// <returns></returns>
    public static byte[] SymmetricDecrypt(byte[] cipherBytes, byte[] key, byte[] iv)
    {
      if (cipherBytes == null)
        throw new ArgumentNullException("cipherBytes");

      if (key == null)
        throw new ArgumentNullException("key");

      if (iv == null)
        throw new ArgumentNullException("iv");

      //Aes 
      using (SymmetricAlgorithm algorithm = Aes.Create())
      using (ICryptoTransform decryptor = algorithm.CreateDecryptor(key, iv))
        return SymmetricTransform(cipherBytes, key, iv, decryptor);
    }

    /// <summary>
    /// Helper method to perform crypto transorm to a memory stream.  (Albahari C# 3.0 In a Nutshell)
    /// </summary>
    /// <param name="data"></param>
    /// <param name="key"></param>
    /// <param name="iv"></param>
    /// <param name="transformer"></param>
    /// <returns></returns>
    private static byte[] SymmetricTransform(byte[] data, byte[] key, byte[] iv, ICryptoTransform transformer)
    {
      MemoryStream ms = new MemoryStream();
      using (Stream c = new CryptoStream(ms, transformer, CryptoStreamMode.Write))
        c.Write(data, 0, data.Length);
      return ms.ToArray();
    }


    public static string GenerateSymmetricKey()
    {
      //Creates the default implementation, which is RijndaelManaged.  
      SymmetricAlgorithm algorithm = SymmetricAlgorithm.Create();
      algorithm.GenerateKey();
      return HexFromByteArray(algorithm.Key);
    }

    public static string GenerateSymmetricIv()
    {
      //Creates the default implementation, which is RijndaelManaged.  
      SymmetricAlgorithm algorithm = SymmetricAlgorithm.Create();
      algorithm.GenerateIV();
      return HexFromByteArray(algorithm.IV);
    }

    /// <summary>
    /// Helper method to compare a byte array
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool CompareByteArray(byte[] a, byte[] b)
    {
      if (a.Length != b.Length)
        return false;
      for (int i = 0; i < a.Length; i++)
      {
        if (a[i] != b[i])
          return false;
      }
      return true;
    }

    /// <summary>
    /// Helper method to cereate a hex string from byte array.
    /// </summary>
    /// <param name="data">The byte data.</param>
    /// <returns>A hex string (base16 encoded) </returns>
    public static string HexFromByteArray(byte[] data)
    {
      return BitConverter.ToString(data).Replace("-", "").ToUpper();
    }

    /// <summary>
    /// Helper method to create a byte array from a hex encoded string
    /// </summary>
    /// <param name="data">A hex string (base16 encoded).</param>
    /// <returns>A byte array</returns>
    public static byte[] HexToByteArray(string data)
    {
      byte[] result = new byte[data.Length / 2];
      for (int i = 0; i < result.Length; i++)
      {
        result[i] = byte.Parse(data.Substring(i * 2, 2), NumberStyles.HexNumber);
      }

      return result;
    }
  }
}
