using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace NoMoreSeewo
{
    public class eewData
    {
        public string? ID, OriginTime, HypoCenter;
        public float? x, y, Magunitude, MaxIntensity;
    }


    public class MathThings
    {
        float EARTH_RADIUS = 6378.137f;
        float rad(float? d)
        {
            return (float)(d * Math.PI / 180.0);
        }

        public float GetDistance(float? x1, float? y1, float? x2, float? y2)
        {
            float a = rad(x1) - rad(x2);
            float b = rad(y1) - rad(y2);
            float s = (float)(2 * Math.Asin(Math.Sqrt(
                Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(rad(x1)) * Math.Cos(rad(x2)) * Math.Pow(Math.Sin(b / 2), 2))));

            s = s * EARTH_RADIUS;
            s = (s * 10000) / 10;

            return s;
        }
    }

    public class InternetActions
    {
        public int id = 0;

        /// <summary>
        /// 获取页面html
        /// </summary>
        /// <param name="url">请求的地址</param>
        /// <param name="encoding">编码方式</param>
        /// <returns></returns>
        public string HttpGetPageHtml(string url, string encoding)
        {
            string pageHtml = string.Empty;

            using (WebClient MyWebClient = new())
            {
                Encoding encode = Encoding.GetEncoding(encoding);
                MyWebClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.84 Safari/537.36");
                MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
                Byte[] pageData = MyWebClient.DownloadData(url); //从指定网站下载数据
                pageHtml = encode.GetString(pageData);
            }
            return pageHtml;
        }
        /// <summary>
        /// 从html中通过正则找到ip信息(只支持ipv4地址)
        /// </summary>
        /// <param name="pageHtml"></param>
        /// <returns></returns>
        public string GetIPFromHtml(String pageHtml)
        {
            //验证ipv4地址
            string reg = @"(?:(?:(25[0-5])|(2[0-4]\d)|((1\d{2})|([1-9]?\d)))\.){3}(?:(25[0-5])|(2[0-4]\d)|((1\d{2})|([1-9]?\d)))";
            string ip = "";
            Match m = Regex.Match(pageHtml, reg);
            if (m.Success)
            {
                ip = m.Value;
            }
            return ip;
        }

        public eewData GetEEW()
        {
            WebClient webClient = new();
            try
            {
                Byte[] raw = webClient.DownloadData("https://api.projectbs.cn/icl/get_data.json");


                string jsonRaw = Encoding.UTF8.GetString(raw);

                JObject json = JObject.Parse(jsonRaw);
                eewData data = new()
                {
                    ID = json["ICL"]["eventId"].ToString(),
                    OriginTime = json["ICL"]["startAt2"].ToString(),
                    HypoCenter = json["ICL"]["epicenter"].ToString(),
                    x = (float?)json["ICL"]["latitude"],
                    y = (float?)json["ICL"]["longitude"],
                    Magunitude = (float)json["ICL"]["magnitude"],
                    MaxIntensity = (float)json["ICL"]["maxInt"]
                };

                return data;
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.ToString());
                return GetEEW_FailBack();
            }
        }

        public eewData GetEEW_FailBack()
        {
            WebClient webClient = new();
            try
            {
                Byte[] raw = webClient.DownloadData("https://api.wolfx.jp/cenc_eqlist.json?");


                string jsonRaw = Encoding.UTF8.GetString(raw);

                JObject json = JObject.Parse(jsonRaw);
                eewData data = new()
                {
                    ID = "cenc",
                    OriginTime = json["No0"]["time"].ToString(),
                    HypoCenter = json["No0"]["location"].ToString(),
                    x = (float?)json["No0"]["latitude"],
                    y = (float?)json["No0"]["longitude"],
                    Magunitude = (float)json["No0"]["magnitude"],
                    MaxIntensity = 0.0f
                };

                return data;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return GetEEW_FailBack();
            }
        }
    }
}
