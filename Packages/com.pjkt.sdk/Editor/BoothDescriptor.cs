/////////////////////////////////////////////////////////
///                                                   ///
///    Written by Chanoler                            ///
///    If you are a VRChat employee please hire me    ///
///    chanolercreations@gmail.com                    ///
///                                                   ///
/////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;

namespace PJKT.SDK
{
    [DisallowMultipleComponent]
    public class BoothDescriptor : MonoBehaviour
    {
        public string boothName { get { return gameObject.name;}}
        public bool showBounds = true;

        #if !COMPILER_UDONSHARP && UNITY_EDITOR
            private void OnValidate() {
                
            }

            private void OnDrawGizmosSelected() {
                //----------Draw a box around the booth----------//
                    //Get all renderers
                Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

                //Override the max dims with the margin
                Vector3 MaxDimsWithoutMargin = PJKT.SDK.Window.BoothValidator.MaxDims;
                Vector3 MaxDims = MaxDimsWithoutMargin + (Vector3.one * PJKT.SDK.Window.BoothValidator.MaxDimsMargin);

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
                    Gizmos.DrawWireCube(center, PJKT.SDK.Window.BoothValidator.MaxDims);

                    Gizmos.color = isTooBig ? Color.red : Color.green;
                    Gizmos.DrawWireCube(center, dims);
                }

            }
        #endif
    }

    // Custom Inspector for PJKT Booth Descriptor
    [CustomEditor(typeof(BoothDescriptor))]
    public class PJKTBoothDescriptorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}