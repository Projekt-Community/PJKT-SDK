using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Net.Http;

namespace PJKT.SDK2.NET
{
    public class PJKTNet
    {
        public const string defaultHost = "https://api.projektcommunity.com";

        public static async Task<HttpResponseMessage> SendMessage(PJKTServerMessage message, string url = defaultHost)
        {
            string messageJson = JsonUtility.ToJson(message, true);
            StringContent data = new StringContent(messageJson, Encoding.UTF8, "application/json");
            Uri endpoint = new Uri(url + message.endpoint);
            HttpResponseMessage response;
            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler() { CookieContainer = cookies };
            using (HttpClient client = new HttpClient(handler))
            {
                using (HttpRequestMessage request = new HttpRequestMessage(message.method, endpoint))
                {
                    request.Content = data;
                    if (Authentication.PjktCookie != "")
                    {
                        cookies.Add(endpoint, new Cookie("session", Authentication.PjktCookie));
                    }
                    
                    //Debug.Log($"sending {messageJson} to {endpoint}");
                    response = await client.SendAsync(request);
                }
            }

            string body = await response.Content.ReadAsStringAsync();
            //Debug.Log(response.ToString() + '\n' + body);
            
            return response;
        }

        //Request json from the server
        public static async Task<string> RequestMessage(string endpoint)
        {
            Uri uri = new Uri(defaultHost + endpoint);
            HttpResponseMessage response;
            
            CookieContainer cookies = new CookieContainer();
            if (Authentication.PjktCookie != "")
            {
                cookies.Add(uri, new Cookie("session", Authentication.PjktCookie));
            }
            //else Debug.Log("no session token found to add to the get request cookie");
            
            HttpClientHandler handler = new HttpClientHandler() { CookieContainer = cookies };
            using (HttpClient client = new HttpClient(handler))
            {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri))
                {
                    //request.Headers.Add("Origin", "https://www.projektcommunity.com");
                    //Debug.Log($"sending Get request {request.ToString()} to {endpoint}");
                    response = await client.SendAsync(request);
                }
            }
            
            string body = await response.Content.ReadAsStringAsync();
            //Debug.Log(response.ToString() + '\n' + body);
            return body;
        }

    }
}