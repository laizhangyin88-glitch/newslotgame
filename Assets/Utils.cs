using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BlizzUtils
{
    public static class Utils
    {
        static System.Random rand;
        public static void SetRandomSeed()
        {
            rand = new System.Random((int)System.DateTime.Now.Ticks);
        }
        public static int GetRandom(int from, int to)
        {
            return rand.Next(from, to);
        }

        public static byte[] ToByteArray(string str)
        {
            byte[] send = System.Text.Encoding.UTF8.GetBytes(str);
            byte[] old = send;
            send = new byte[old.Length + 5];
            System.BitConverter.GetBytes(old.Length + 1).CopyTo(send, 0);
            send[4] = 0;
            old.CopyTo(send, 5);
            return send;
        }

        public static string LocalIP()
        {
            string AddressIP = string.Empty;
            string IP = "";
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());   //Dns.GetHostName()获取本机名Dns.GetHostAddresses()根据本机名获取ip地址组
            foreach (IPAddress ip in ips)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    IP = ip.ToString();  //ipv4
                }
            }
            return IP;
        }

        /// <summary>
        /// 指定Post地址使用Get 方式获取全部字符串
        /// </summary>
        /// <param name="url">请求后台地址</param>
        /// <param name="dic">key:参数名,value:值</param>
        /// <returns></returns>
        public static string Post(string url, Dictionary<string, string> dic)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            #region 添加Post 参数
            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (var item in dic)
            {
                if (i > 0)
                    builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
            byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }


        /// <summary>
        /// 按长度分割字符串，汉字按一个字符算
        /// </summary>
        /// <param name="SourceString"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static List<string> SplitLength(string SourceString, int Length)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < SourceString.Trim().Length; i += Length)
            {
                if ((SourceString.Trim().Length - i) >= Length)
                    list.Add(SourceString.Trim().Substring(i, Length));
                else
                    list.Add(SourceString.Trim().Substring(i, SourceString.Trim().Length - i));
            }
            return list;
        }

    }

}
