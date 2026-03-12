using System;

[Serializable]
public class PjktPrefabsList
{
    public PjktPrefabsInfo[] Prefabs;
}

[Serializable]
public class PjktPrefabsInfo
{
    public string PrefabName;
    public string PrefabDescription;
    public string ResourceURL;
    public string DefaultAssetPath;
}