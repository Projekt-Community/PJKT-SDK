using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJKT.SDK2
{
    public enum BoothType
    {
        SdkBooth,
        WebToolBooth
    }
    
    [Serializable]
    public class BoothMetadata
    {
        public string EventName;
        public BoothType boothType;
        public DateTime BoothUploadDate;
        public string BoothUploaderUsername;
        public CommunityInfo communityInfo;
        public SdkBoothInfo sdkBoothInfo;
        public WebToolBoothInfo webToolBoothInfo;
        
        public bool IsValid()
        {
            bool valid = true;
            //probably need a better way to do this 
            valid &= !string.IsNullOrEmpty(BoothUploaderUsername);
            
            return valid;
        }
    }

    [Serializable]
    public class CommunityInfo
    {
        public int Id;
        public string CommunityName;
        public string CommunityDescription;
        public string LogoUrl;
        public string GroupID;
    }

    [Serializable]
    public class SdkBoothInfo
    {
        public string BoothPrefabName;
        public string[] BoothStats;
    }

    [Serializable]
    public class WebToolBoothInfo
    {
        public List<Pedestal> pedestals;
        public DateTime timestamp;
        public string version;
    }

    [Serializable]
    public class Pedestal
    {
        public string id;
        public string type;
        public string vrcId;
        public Vector3 position;
    }
}