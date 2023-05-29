//#define PJKT_DEBUG

#if PJKT_DEBUG
    //Do not warn about async methods without await if we are in debug mode
    #pragma warning disable CS1998
#endif

using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using System.Net.Http;
using PJKT.SDK.NET.Messages;
using PJKT.SDK.Window;

namespace PJKT.SDK.NET
{
    public class PJKTNet
    {
        public const string defaultHost = "https://api.projektcommunity.com";
        private const int port = 11000;
        
        //Send json to the server
        public static async Task<HttpResponseMessage> SendMessage(PJKTServerMessage message, string url = defaultHost)
        {
            string messageJson = JsonUtility.ToJson(message, true);
            StringContent data = new StringContent(messageJson, Encoding.UTF8, "application/json");
            Uri endpoint = new Uri(url + message.endpoint);
            HttpResponseMessage response = new HttpResponseMessage();
            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler() { CookieContainer = cookies };
            using (HttpClient client = new HttpClient(handler))
            {
                HttpRequestMessage request = new HttpRequestMessage(message.method, endpoint);
                request.Content = data;
                cookies.Add(endpoint, new Cookie("session", AuthData.PjktCookie));
                response = await client.SendAsync(request);
            }
            return response;
        }

        //Request json from the server
        public static async Task<string> RequestMessage(string url = defaultHost)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";

            string responseData = "";

            using (WebResponse response = await request.GetResponseAsync())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        responseData = reader.ReadToEnd();
                    }
                }
            }
                
            return responseData;
        }

    }
}