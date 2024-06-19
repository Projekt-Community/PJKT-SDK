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
                    if(!name.EndsWith("Instance")) return;
                    uint existingMeshHash = CalculateMeshHash(filter.sharedMesh);
                    
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
                        
                        uint replacementMeshHash = CalculateMeshHash(mesh);
                        
                        //compare
                        if (existingMeshHash != replacementMeshHash) continue;
                        
                        filter.sharedMesh = mesh;
                        break;
                    }
                    
                    //set dirty so we can undo
                    EditorUtility.SetDirty(filter);
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
        static uint CalculateMeshHash (Mesh mesh)
		{
			// Calculate an unique (I hope) hash value per mesh.
			// If anyone has a better idea on how to do this, please let me know.

			uint hash = 0;
			uint count = 0;
			Vector3 scale = new Vector3(2039.0f, 2053.0f, 2063.0f);

			Bounds bounds = mesh.bounds;
			Vector3 extent = bounds.max - bounds.min;
			Vector3 invExtent = new Vector3(1.0f / extent.x, 1.0f / extent.y, 1.0f / extent.z);

			// Vertex hash
			// Each vertex is scaled down to 0..1 inside the bounding volume. Then an arbitrary
			// limited scale is applied and the three components are added together.

			foreach (Vector3 v in mesh.vertices) 
			{
				Vector3 vertexBound = Vector3.Scale(v-bounds.min, invExtent);
				uint vertexHash = (uint)Vector3.Dot(vertexBound, scale);

				hash += vertexHash * count++;
			}

			// Consider the position of the bounds themselves (two identical meshes with different
			// origins must have different hashes)

			uint boundHash = (uint)(Vector3.Dot(bounds.max, new Vector3(1.0f,3.0f,5.0f)) + Vector3.Dot(bounds.min, new Vector3(3.0f,5.0f,1.0f)));
			hash += boundHash * count;

			count = 7;

			// Indexes hash
			// Must be calculated per submesh for considering different materials.
			// I haven't seen subMeshCount = 0, but who knows...

			if (mesh.subMeshCount == 0)
			{
				foreach (int t in mesh.triangles)
					hash += (uint)t * count;
			}
			else
			{
				for (int s=0, c=mesh.subMeshCount; s<c; s++)
				{
					int[] triangles = mesh.GetTriangles(s);

					foreach (int t in triangles)
						hash += (uint)t * count;

					count++;
				}
			}

			hash += count * (uint)mesh.subMeshCount;

			count = 11;

			// Normals hash

			Vector3 boundNormal = Vector3.one * 0.5f;

			foreach (Vector3 v in mesh.normals)
			{
				Vector3 vertexBound = Vector3.Scale(v+Vector3.one, boundNormal);
				uint vertexHash = (uint)Vector3.Dot(vertexBound, scale);

				hash += vertexHash * count++;
			}

			count = 13;

			// Tangent hash (not required for Blender)

			foreach (Vector3 v in mesh.tangents)
			{
				Vector3 vertexBound = Vector3.Scale(v+Vector3.one, boundNormal);
				uint vertexHash = (uint)Vector3.Dot(vertexBound, scale);

				hash += vertexHash * count++;
			}

			count = 17;

			// UV1 hash

			foreach (Vector2 v in mesh.uv)
			{
				uint uvHash = (uint)Vector2.Dot(v, scale);
				hash += uvHash * count++;
			}

			count = 19;

			// UV2 hash

			foreach (Vector2 v in mesh.uv2)
			{
				uint uvHash = (uint)Vector2.Dot(v, scale);
				hash += uvHash * count++;
			}

			// TODO: Hash for skinned meshes, bones, ... maybe?
			// mesh.boneWeights
			// mesh.bindposesSS

			return hash;
		}
    }
}