using System;
using UnityEditor;
using UnityEngine;

namespace PJKT.SDK2
{
    [DisallowMultipleComponent]
    [AddComponentMenu("PJKT/Booth Descriptor")]
    public class BoothDescriptor : MonoBehaviour
    {
        public string boothName { get { return gameObject.name;}}
        public bool showBounds = true;
        public bool IsolateBoothAudio = false;
        public string currentCommunity;
        public string GroupID = "grp_";
        public string[] representitives = new string[3];
        public static Vector3 _maxBounds = new Vector3(5, 5, 5);
        public string SDKVersion = "Unknown";
        private static float _margin = 0.1f;
        
        
        #if !COMPILER_UDONSHARP && UNITY_EDITOR
            private void OnDrawGizmosSelected() 
            {
                //----------Draw a box around the booth----------//
                    //Get all renderers
                Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

                //Override the max dims with the margin
                Vector3 MaxDims = _maxBounds + (Vector3.one * _margin);

                //I have lost all faith in Bounds.Encapsulate
                Vector3 center, dims;
                if (renderers.Length > 0)
                {
                    Vector3 min = renderers[0].bounds.min;
                    Vector3 max = renderers[0].bounds.max;

                    //Get bounds of all renderers
                    foreach (Renderer renderer in renderers) {
                        Bounds bounds = renderer.bounds;
                        min = Vector3.Min(min, bounds.min);
                        max = Vector3.Max(max, bounds.max);
                    }

                    center = (min + max) / 2f;
                    dims = max - min;
                }
                else
                {
                    center = transform.position;
                    dims = Vector3.zero;
                }

                float longestSize = Mathf.Max(dims.x, dims.y, dims.z);
                float maxLongestSide = Mathf.Max(MaxDims.x, MaxDims.y, MaxDims.z);
                bool isTooBig = longestSize > maxLongestSide;

                if (showBounds || isTooBig)
                {
                    Gizmos.color = Color.gray;
                    Gizmos.DrawWireCube(center, _maxBounds);

                    Gizmos.color = isTooBig ? Color.red : Color.green;
                    Gizmos.DrawWireCube(center, dims);
                }
            }
        
        private void OnDrawGizmos() 
        {
            Matrix4x4 previousGizmosMatrix = Gizmos.matrix;
            Matrix4x4 previousHandlesMatrix = Handles.matrix;

            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Handles.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            
            Gizmos.color = new Color(0.2f, 0.2f, 0.8f, 0.5f);
            
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(_maxBounds.x, 0.01f, _maxBounds.z));
            Handles.Label(Vector3.zero + new Vector3(_maxBounds.x / 2 + .2f, 0, 0), "Floor Plane", EditorStyles.boldLabel);
            
            Gizmos.color = new Color(0.4f, 0.2f, 0.8f, 0.5f);
            Vector3 indicatorPos = new Vector3(0, 0, _maxBounds.z / 2);

            Gizmos.DrawMesh(Resources.GetBuiltinResource<Mesh>("Cylinder.fbx"), indicatorPos, Quaternion.identity, new  Vector3(.3f, .01f, .3f));
            Handles.Label(indicatorPos + new Vector3(0, .1f, 0), "Front", EditorStyles.boldLabel);

            Gizmos.matrix = previousGizmosMatrix;
            Handles.matrix = previousHandlesMatrix;
        }
#endif
    }
}