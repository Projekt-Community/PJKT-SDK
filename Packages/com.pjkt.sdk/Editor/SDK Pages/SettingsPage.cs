using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
using VRC.PackageManagement.Core;

namespace PJKT.SDK2
{
    public class SettingsPage : SDKPage
    {
        public override void OnTabEnable()
        {
            topArea.style.display = DisplayStyle.None;
            scrollView.style.display = DisplayStyle.None;
            style.flexGrow = 1;
            
            SettingsPannel panel = new SettingsPannel();
            panel.style.flexGrow = 1;
            VisualElement parent = topArea.parent;
            parent.Add(panel);
        }
        
        
    }
}