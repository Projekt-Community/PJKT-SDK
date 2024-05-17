using System.Reflection;
using UdonSharp;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class toggletest : UdonSharpBehaviour
{
    public Toggle toggle;
    public void TestStuff()
    {
        Debug.Log("fug");
    }
}

#if UNITY_EDITOR && !COMPILER_UDONSHARP
[CustomEditor(typeof(toggletest))]
public class toggleEditor : Editor
{
    private toggletest testObject;
    private void OnEnable()
    {
        testObject = (toggletest)target;

    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("try"))
        {
            MethodInfo method = typeof(UdonSharpBehaviour).GetMethod("SendCustomEvent");
        
            UnityAction<string> action = System.Delegate.CreateDelegate(typeof(UnityAction<string>), testObject, method) as UnityAction<string>;
        
        
            UnityEventTools.AddStringPersistentListener(testObject.toggle.onValueChanged, action, "On");
        }
    }
}
#endif
