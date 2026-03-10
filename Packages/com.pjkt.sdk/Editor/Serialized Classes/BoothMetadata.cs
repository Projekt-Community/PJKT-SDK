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
        
    }
    
    //add the serialised stuff from the fang booth setup script
}