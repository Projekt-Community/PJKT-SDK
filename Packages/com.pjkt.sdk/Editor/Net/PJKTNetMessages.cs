using System;
using System.Collections.Generic;
using System.Net.Http;
using PJKT.SDK.Window;

// A collection of messages used by the PJKT SDK to communicate with the PJKT server.
// All of these messages are serializable and deserializable to JSON.
namespace PJKT.SDK.NET.Messages
{
    // Parent which all messages inherit from.
    [Serializable]
    public abstract class PJKTServerMessage
    {
        // String containing the firebase token, common to all messages.
        public abstract HttpMethod method { get; } //POST or GET
        public abstract string endpoint { get; } //Path to the server endpoint
        public abstract bool expectResponse { get; } //Whether or not the server is expected to respond
    }

    // Holds abstract value pairs.
    public class Tuple<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;
    }

    /* ~~~Account Creation~~~
     * User Creates account on Firebase
     * Firebase returns a User Object
     * Front end sends the following to the server: 
     */
    [Serializable]
    public class PJKTAccountCreationMessage : PJKTServerMessage
    {
        public override HttpMethod method => HttpMethod.Post;
        public override string endpoint => "/booth/signup";
        public override bool expectResponse => true;

        public PJKTAccountCreationMessage(string communityName, string token)
        {
            this.communityName = communityName;
            this.idToken = token;
        }

        // Name of Community
        public string communityName;
        public string idToken;


    }

    /* ~~~Login~~~
     * User Logs in with Firebase
     * Firebase returns User Object
     * Front end sends following to the server:
     * (Up to date requirements)
     */
    [Serializable]
    public class PJKTLoginMessage : PJKTServerMessage
    {
        public override HttpMethod method => HttpMethod.Post;
        public override string endpoint => "/booth/login";
        public override bool expectResponse => true;
        
        public PJKTLoginMessage()
        {
            idToken = AuthData.token;
        }

        // String containing the firebase authentication id
        public string idToken;
        // String containing the Firebase CSRF Token (A cookie that shows the firebase session)
        //public string CSRFToken;
    }

    [Serializable]
    public class PJKTLogoutMessage : PJKTServerMessage
    {
        public override HttpMethod method => HttpMethod.Post;
        public override string endpoint => "/auth/logout";

        public override bool expectResponse => true;
    }
    // Response from the server
    [Serializable]
    public class PJKTLoginResponseMessage
    {
        // Insert booth requirements here
        public Tuple<string, string>[] Requirements;
        // All User Information
        public Tuple<string, string>[] Profile;
    }

    /* ~~~Uploading~~~
     * User clicks upload file
     * Front end sends the following: 
     */
    //TODO
    [Serializable]
    public class PJKTUploadMessage : PJKTServerMessage
    {
        public override HttpMethod method => HttpMethod.Post;
        public override string endpoint => "/booth/submit";
        public override bool expectResponse => false;

        public PJKTUploadMessage(byte[] file, byte[] previewImage, string boothName = "", string boothDescription = "")
        {
            this.name = boothName != "" ? boothName : AuthData.communityName;
            this.description = boothDescription != "" ? boothDescription : "A booth to represent the " + AuthData.communityName + " community.";
            //Base64 encode the files
            this.file = Convert.ToBase64String(file ?? new byte[0]);
            this.previewImage = Convert.ToBase64String(previewImage ?? new byte[0]);
        }

        // String of session Cookie
        //public string SessionCookie;
        // The Unity Package File
        public string file; //Base64 encoded
        public byte[] fileBytes { get { return Convert.FromBase64String(file); } }
        // A picture file that shows an image of the booth
        public string previewImage; //Base64 encoded
        public byte[] previewImageBytes { get { return Convert.FromBase64String(previewImage); } }
        // Title of the booth
        public string name;
        // Description of the booth or submission
        public string description;
    }

    /* ~~~Updating Profile~~~
     * User Clicks Update Profile
     * Frontend sends the following: 
     */
    [Serializable]
    public class PJKTProfile : PJKTServerMessage
    {
        public override HttpMethod method => throw new NotImplementedException();
        public override string endpoint => throw new NotImplementedException();
        public override bool expectResponse => throw new NotImplementedException();

        public const string path = "profile";

        public PJKTProfile(string communityName = "Unknown" , string discordURL = "", byte[] profilePhoto = null)
        {
            this.CommunityName = communityName;
            this.DiscordURL = discordURL;
            //Base64 encode the files
            this.ProfilePhoto = Convert.ToBase64String(profilePhoto ?? new byte[0]);
        }

        // Name of the community
        public readonly string CommunityName;
        // File of the profile image,
        public readonly string ProfilePhoto;
        public byte[] ProfilePhotoBytes { get { return Convert.FromBase64String(ProfilePhoto); } }
        //Valid Discord URL
        public readonly string DiscordURL;
    }

    [Serializable]
    internal class PjktResponseObject
    {
        public bool error;
        public string message;
        public PjktResponseData data;
    }
    [Serializable]
    internal class PjktResponseData
    {
        public string communityName;
        public string errorMessage;
    }
}