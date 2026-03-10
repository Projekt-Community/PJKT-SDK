using UnityEngine;

namespace PJKT.SDK2
{
    [RequireComponent(typeof(Renderer))]
    [DisallowMultipleComponent]
    [AddComponentMenu("PJKT/Material Swapper")]
    public class PjktMaterialSwapper : MonoBehaviour
    {
        public Renderer rend;
        [SerializeField] private Material[] windowsMaterials;
        [SerializeField] private Material[] androidMaterials;
        [SerializeField] private Material[] iosMaterials;
        
        //0 = windows, 1 = android, 2 = IOS
        public void SwapMaterialsToTarget(int target)
        {
            //apply the target materials
            switch (target)
            {
                case 0:
                    rend.sharedMaterials = windowsMaterials;
                    break;
                case 1:
                    rend.sharedMaterials = androidMaterials;
                    break;
                case 2:
                    rend.sharedMaterials = iosMaterials;
                    break;
            }
        }

        private void Reset()
        { 
            //called when component is added. used to get the renderer and materials
            rend = GetComponent<Renderer>();
            return; //some fucked shit where this is resetting the mats untill the next assembly reload
            
            Material[] rendererMats = rend.sharedMaterials;
            
            //set all arrays to this for now
            windowsMaterials = rendererMats;
            androidMaterials = rendererMats;
            iosMaterials = rendererMats;
        }
    }
}