using System;
using System.Net.Http;

// A collection of messages used by the PJKT SDK to communicate with the PJKT server.
// All of these messages are serializable and deserializable to JSON.
namespace PJKT.SDK2.NET
{
    // Parent which all messages inherit from.
    [Serializable]
    public abstract class PJKTServerMessage
    {
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
        public override string endpoint => "/auth/register";
        public override bool expectResponse => true;

        public PJKTAccountCreationMessage(string username, string inviteCode, string token)
        {
            this.username = username;
            this.idToken = token;
            this.inviteCode = inviteCode;
        }

        public string inviteCode;
        public string username;
        public string idToken;
    }
    
    /// <summary>
    /// Checks the invite code to see if its valid
    /// </summary>
    [Serializable]
    public class PJKTCheckInviteCodeMessage : PJKTServerMessage
    {
        public override HttpMethod method => HttpMethod.Post;
        public override string endpoint => "/auth/register/validate";
        public override bool expectResponse => true;

        public PJKTCheckInviteCodeMessage(string inviteCode)
        {
            this.inviteCode = inviteCode;
        }

        public string inviteCode;
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
        public override string endpoint => "/auth/login";
        public override bool expectResponse => true;
        
        public PJKTLoginMessage()
        {
            idToken = Authentication.token;
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
        
        public PJKTLogoutMessage()
        {
            idToken = Authentication.token;
        }
        
        public string idToken;
    }
    
    /// <summary>
    /// adds a user with an existing account to a community with an invite code 
    /// </summary>
    [Serializable]
    public class PJKTJoinCommunityMessage : PJKTServerMessage
    {
        public override HttpMethod method => HttpMethod.Post;
        public override string endpoint => "/users/user/invite";
        public override bool expectResponse => true;

        public PJKTJoinCommunityMessage(string inviteCode)
        {
            this.inviteCode = inviteCode;
            this.idToken = Authentication.token;
        }

        public string inviteCode;
        public string idToken;
    }
}