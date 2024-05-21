
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace PJKT.SDK.Prefabs
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class OpenGroupInformationContainer : UdonSharpBehaviour
    {
        [Header("Populate this with the group you want to open")]
        [Header("it will be replaced by our own button prefab.")]
        [Header("Feel free to remove the mesh renderer.")]
        public int groupIdToOpen;
    }
}
