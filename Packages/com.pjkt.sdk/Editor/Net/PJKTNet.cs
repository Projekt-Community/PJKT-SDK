/////////////////////////////////////////////////////////
///                                                   ///
///    Written by Chanoler                            ///
///    If you are a VRChat employee please hire me    ///
///    chanolercreations@gmail.com                    ///
///                                                   ///
/////////////////////////////////////////////////////////

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
            #if PJKT_DEBUG
                string messageJson = JsonUtility.ToJson(message, true);
                Debug.Log(message.GetType() + ":\n" + messageJson);
                return "DEBUG";
            #else
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
#endif
        }

        //Request json from the server
        public static async Task<string> RequestMessage(string url = defaultHost)
        {
            #if PJKT_DEBUG
                return "DEBUG";
            #else
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
            #endif
        }

    }











    /*public class PJKTNetLegacy
    {
        //Communication is as follows:
        //0. -> Client sends the intention (1 byte) to the server
        //1. -> Client sends the secret key length and key to the server
        //2. <- Server responds with 'Y' or 'N'
        //3. -> Client sends the file size and SHA1 hash to the server
        //4. <- Server responds with 'Y' or 'N'
        //5. -> Client sends the file to the server
        //6. <- Server acknowledges the file by responding with the SHA1 hash of the received file
        
        //Bytes exchanged:
        //0. -> 1 byte
        //1. -> 4 bytes (int) + key.Length bytes
        //2. <- 1 byte
        //3. -> 4 bytes (int) + 20 bytes
        //4. <- 1 byte
        //5. -> file.Length bytes
        //6. <- 20 bytes

        static readonly long MaxSize = 10 * 1024 * 1024; //10 MB

        public static void UploadFile(string key, string filePath)
        {
            string host = "localhost";
            int port = 11000;

            byte Y = Encoding.ASCII.GetBytes("Y")[0];
            byte N = Encoding.ASCII.GetBytes("N")[0];

            //Intention "U" = Upload
            byte intention = Encoding.ASCII.GetBytes("U")[0];

            //Disposable objects
            TcpClient client = null;
            NetworkStream stream = null;

            try {
                client = new TcpClient();
                client.Connect(host, port);
                stream = client.GetStream();

                //----------Intention and Authentication----------//
                EditorUtility.DisplayProgressBar("Authenticating", "Authenticating with the server", 0);

                //Send the intention to the server
                stream.WriteByte(intention); // -> 1 byte

                //Advise the server of the key length
                byte[] serializedKey = Encoding.ASCII.GetBytes(key);
                stream.Write(BitConverter.GetBytes(serializedKey.Length), 0, 4); // -> 4 bytes (int)

                //Send the secret key to the server
                stream.Write(serializedKey, 0, serializedKey.Length); // -> key.Length bytes

                //Wait for either 5 seconds or a response from the server
                client.ReceiveTimeout = 5000;
                int responseByte = stream.ReadByte();
                if (responseByte == N) {
                    OhShit("The server rejected our secret key");
                    return;
                } else if (responseByte != Y) {
                    //This should never happen
                    OhShit("Invalid response from server, please try again or ask for help in the Discord server");
                    return;
                }

                //----------File Info----------//
                EditorUtility.DisplayProgressBar("Getting file info", "Getting file info", 0);

                //Check if the file exists
                FileInfo fileInfo = new FileInfo(filePath);
                if (!fileInfo.Exists) {
                    OhShit("Tried to upload \"" + filePath + "\", but it does not exist.");
                    return;
                }

                //Get the file size
                long fileSize = fileInfo.Length;
                //Self-reject files that are too large
                if (fileSize > MaxSize || fileSize > int.MaxValue) {
                    OhShit("The built file is too large to upload. Please reduce the size of your project and try again.");
                    return;
                }

                //Load the file into memory
                EditorUtility.DisplayProgressBar("Getting file info", "Reading file into memory", 0);
                byte[] fileBytes = File.ReadAllBytes(filePath);

                //Calculate the SHA1 hash of the file
                EditorUtility.DisplayProgressBar("Getting file info", "Calculating SHA1 hash", 0);
                SHA1 sha1 = SHA1.Create();
                sha1.ComputeHash(fileBytes);
                byte[] hash = sha1.Hash;
                Debug.Assert(hash.Length == 20);

                //Advise the server of the file size and hash
                stream.Write(BitConverter.GetBytes(fileBytes.Length), 0, 4); // -> 4 bytes (int)
                stream.Write(hash, 0, hash.Length); // -> 20 bytes

                //Wait for either 5 seconds or a response from the server
                EditorUtility.DisplayProgressBar("Getting file info", "Waiting for server response", 0);
                client.ReceiveTimeout = 5000;
                responseByte = stream.ReadByte();
                if (responseByte == N) {
                    OhShit("Server rejected the file");
                    return;
                } else if (responseByte != Y) {
                    //This should never happen
                    OhShit("Invalid response from server, please try again or ask for help in the Discord server");
                    return;
                }

                //----------File Upload----------//

                //Send the file to the server, monitoring the progress
                // Task progressTask = Task.Run(() => {
                //     int lastProgress = 0;
                //     while (true) {
                //         int progress = (int)(fileStream.Position * 100 / fileSize);
                //         if (progress != lastProgress) {
                //             EditorUtility.DisplayProgressBar("Uploading file", "Uploading file to the server", progress / 100f);
                //             lastProgress = progress;
                //         }
                //     }
                // });

                //Send the file
                EditorUtility.DisplayProgressBar("Uploading file", "Uploading file to the server", 0.5f);
                stream.Write(fileBytes, 0, fileBytes.Length);

                //----------File Acknowledgement----------//

                //Wait for the server to acknowledge the file
                EditorUtility.DisplayProgressBar("Uploading file", "Waiting for acknowledgement from server", 1f);
                client.ReceiveTimeout = 5000;
                byte[] serverHash = new byte[20];
                int bytesRead = stream.Read(serverHash, 0, 20);
                if (bytesRead != 20) {
                    OhShit("Server did not acknowledge the file");
                    return;
                }

                //Check if the server hash matches the client hash
                if (!serverHash.SequenceEqual(hash)) {
                    //This should never happen
                    OhShit( "The file received by the server does not match the file sent by the client, " +
                            "however the server did not reject the file. Your submission may have been corrupted. " +
                            "Please ping @Chanoler#9450 in the Discord server because this should never happen.");
                    return;
                }

                //----------Success----------//
                EditorUtility.DisplayDialog("Success", "Your submission has been uploaded successfully!", "OK");

            } catch (Exception e) {
                Debug.LogError(e);
            } finally {
                EditorUtility.ClearProgressBar();
                if (client != null) client.Dispose();
                if (stream != null) stream.Dispose();
            }
        }

        private static void OhShit(string message)
        {
            EditorUtility.ClearProgressBar();
            Debug.Log(message);
            EditorUtility.DisplayDialog("Error", message, "OK");
        }
    }*/
}