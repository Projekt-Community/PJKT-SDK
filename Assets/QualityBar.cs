using System.Reflection;
using UdonSharp;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VRC.Udon.Editor.ProgramSources.UdonGraphProgram.UI.GraphView.UdonNodes;

public class QualityBar : VisualElement
{
    public new class UxmlFactory : UxmlFactory<QualityBar> { }
    private const string uxmlPath = "Assets/quality selector.uxml";

    private Label mobileQuality => this.Q<Label>("mobile");
    private Label lowQuality => this.Q<Label>("low");
    private Label mediumQuality => this.Q<Label>("med");
    private Label highQuality => this.Q<Label>("high");

    private byte qualityFlags = 0b0000_0000;
    
    public QualityBar()
    {
        VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
        asset.CloneTree(this);
        
        lowQuality.RegisterCallback<ClickEvent>(SelectQuality);
        mediumQuality.RegisterCallback<ClickEvent>(SelectQuality);
        highQuality.RegisterCallback<ClickEvent>(SelectQuality);
        mobileQuality.RegisterCallback<ClickEvent>(SelectQuality);
        
        //set alphas to 0
        lowQuality.style.backgroundColor = new Color(.3f, .3f, 1, 0);
        mediumQuality.style.backgroundColor = new Color(1f, 1f, .3f, 0);
        highQuality.style.backgroundColor = new Color(1f, .3f, .3f, 0);
        mobileQuality.style.backgroundColor = new Color(1f, .3f, 1, 0);
    }
    
    private void SelectQuality(ClickEvent evt)
    {
        Label label = (Label)evt.target;
        
        switch (label.name)
        {
            case "mobile":
                //add something here for the funny flags. in the mean time im gonna do my own
                qualityFlags = (byte)(qualityFlags ^ 0b0000_0001);
                break;
            case "low":
                qualityFlags = (byte)(qualityFlags ^ 0b0000_0010);
                break;
            case "med":
                qualityFlags = (byte)(qualityFlags ^ 0b0000_0100);
                break;
            case "high":
                qualityFlags = (byte)(qualityFlags ^ 0b0000_1000);
                break;
        }
        
        SetQuality();
    }

    public void SetQuality()
    {
        //set the alphas based on the quality flags
        mobileQuality.style.backgroundColor = new Color(1f, .3f, 1, (qualityFlags & 0b0000_0001) == 0b0000_0001 ? 1 : 0);
        lowQuality.style.backgroundColor = new Color(.3f, .3f, 1, (qualityFlags & 0b0000_0010) == 0b0000_0010 ? 1 : 0);
        mediumQuality.style.backgroundColor = new Color(1f, 1f, .3f, (qualityFlags & 0b0000_0100) == 0b0000_0100 ? 1 : 0);
        highQuality.style.backgroundColor = new Color(1f, .3f, .3f, (qualityFlags & 0b0000_1000) == 0b0000_1000 ? 1 : 0);
    }
}

public class QualitySelector : EditorWindow
{
    [MenuItem("LPD/Quality Selector")]
    public static void ShowWindow()
    {
        QualitySelector window = GetWindow<QualitySelector>();
        window.titleContent = new GUIContent("Quality Selector");
        window.minSize = new Vector2(200, 200);
    }

    private void OnEnable()
    {
        VisualElement root = rootVisualElement;
        root.Add(new QualityBar());
    }
}
