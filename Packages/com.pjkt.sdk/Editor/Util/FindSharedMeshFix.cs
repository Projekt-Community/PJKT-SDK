using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PJKT.SDK2
{
    /// <summary>
    /// DONT PANIC
    /// This is a quick and dirty fix for the issues of meshes being instantiated in the scene.
    /// </summary>

    public static class FindSharedMeshFix
    {
        [MenuItem("PJKT/Fix Mesh Instances")]
        public static void FixMeshInstances()
        {
            //find all booths
            BoothDescriptor[] boothDescriptors = Object.FindObjectsOfType<BoothDescriptor>();
            if (boothDescriptors == null || boothDescriptors.Length == 0)
            {
                Debug.LogWarning("No BoothDescriptors found in scene");
                return;
            }
            
            //get all the renderers for that booth
            foreach (BoothDescriptor boothDescriptor in boothDescriptors)
            {
                MeshFilter[] filters = boothDescriptor.gameObject.GetComponentsInChildren<MeshFilter>();
                if (filters.Length == 0)
                {
                    Debug.LogWarning("No renderers found for " + boothDescriptor.gameObject.name);
                    continue;
                }
                
                List<GameObject> meshesNotFound = new List<GameObject>();
                
                //find all the shared meshes and look for the mesh in the assets folder with the same name minus the word instance
                foreach (var filter in filters)
                {
                    string name = filter.sharedMesh.name;
                    
                    //remove the word instance
                    name = name.Replace(" Instance", "");
                    
                    //find the mesh in the assets folder
                    string[] guids = AssetDatabase.FindAssets(name);
                    if (guids.Length == 0)
                    {
                        meshesNotFound.Add(filter.gameObject);
                    }
                    
                    //if the mesh is found, assign it to the filter
                    foreach (var guid in guids)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);
                        Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                        if (mesh == null) continue;
                        filter.sharedMesh = mesh;
                    }
                }
                
                //warn about meshes we couldnt fix
                if (meshesNotFound.Count > 0)
                {
                    Debug.LogWarning("Could not find the following meshes for " + boothDescriptor.gameObject.name);
                    foreach (var go in meshesNotFound)
                    {
                        Debug.LogWarning(go.name, go);
                    }
                }
                else
                {
                    Debug.Log("All meshes found for " + boothDescriptor.gameObject.name);
                }
            }
        }
    }
}