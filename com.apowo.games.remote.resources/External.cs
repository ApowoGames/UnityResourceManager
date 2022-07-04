using System.Security.Cryptography;
using System.Text;

namespace ApowoGames.Resources.External
{
    public class StringEnum
    {
        protected StringEnum(string value) { Value = value; }
        public string Value { get; }
        public override string ToString() => Value;
    }
    
    public class MD5Helper
    {
        public static string EncryptString(string str)
        {
            MD5 md5 = MD5.Create();
            // 将字符串转换成字节数组
            byte[] byteOld = Encoding.UTF8.GetBytes(str);
            // 调用加密方法
            byte[] byteNew =  md5.ComputeHash(byteOld);
            // 将加密结果转换为字符串
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteNew)
            {
                // 将字节转换成16进制表示的字符串，
                sb.Append(b.ToString("x2"));
            }
            // 返回加密的字符串
            return sb.ToString();
        }
    }
}