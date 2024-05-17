using System;
using System.Collections.Generic;

namespace PJKT.SDK2
{
    [Serializable]
    public class Community
    {
        public int id;
        public string name;
        public List<string> roles;
    }

    [Serializable]
    public class CommunityMembership
    {
        public Community community;
    }

    [Serializable]
    public class PjktActiveUser
    {
        public User user;
        public List<CommunityMembership> communityMemberships;
        public Staff staff;
        
        public int GetCommunityId(string communityName)
        {
            foreach (var communityMembership in communityMemberships)
            {
                if (communityMembership.community.name == communityName)
                {
                    return communityMembership.community.id;
                }
            }
            return -1;
        }
    }

    [Serializable]
    public class Staff
    {
        public List<object> departments;
    }

    [Serializable]
    public class User
    {
        public int id;
        public string username;
        public object email;
        public List<object> socials;
    }
}