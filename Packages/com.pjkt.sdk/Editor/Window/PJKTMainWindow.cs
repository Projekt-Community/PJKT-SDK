using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Net.Http;
using PJKT.SDK.NET.Messages;
using PJKT.SDK.NET;

namespace PJKT.SDK.Window
{
    public class PJKTMainWindow : EditorWindow {

        //----------UI Data----------//
        //PJKT logo textures
        private static Texture2D pjktLogoText;
        private static Texture2D pjktLogoBackgroundL;
        private static Texture2D pjktLogoBackgroundR;
        private static Texture2D pjktLogoBackgroundColor;

        //Stores active screens, avoids having to create new instances every time
        private List<PJKTWindow> openWindows = new List<PJKTWindow>();
        private PJKTWindow currentWindow;
        private int tabNumber = 0;
        private Vector2 scrollPos = Vector2.zero;
        string[] tabNames = new string[]
        {
            "Booth Uploader",
            "Account Settings"
        };

        //----------Window----------//
        [MenuItem("PJKT SDK/SDK Window")]
        private static void ShowWindow() {
            //Boilerplate code to show the window
            PJKTMainWindow window = GetWindow<PJKTMainWindow>();
            window.titleContent = new GUIContent("PJKT SDK");
            window.minSize = new Vector2(345, 614);
            window.Show();
        }

        private void OnDestroy() 
        {
            //Close all screens
            foreach (PJKTWindow screen in openWindows) {
                screen.OnClose();
            }
        }

        private void OnEnable()
        {
            //initialize the logo textures
            pjktLogoText            = (Texture2D)AssetDatabase.LoadAssetAtPath("Packages/com.pjkt.sdk/Editor/Graphics/logotext512.png"   , typeof(Texture2D));
            pjktLogoBackgroundL     = (Texture2D)AssetDatabase.LoadAssetAtPath("Packages/com.pjkt.sdk/Editor/Graphics/logobgL512.png"    , typeof(Texture2D));
            pjktLogoBackgroundR     = (Texture2D)AssetDatabase.LoadAssetAtPath("Packages/com.pjkt.sdk/Editor/Graphics/logobgR512.png"    , typeof(Texture2D));
            pjktLogoBackgroundColor = (Texture2D)AssetDatabase.LoadAssetAtPath("Packages/com.pjkt.sdk/Editor/Graphics/logobgColor512.png", typeof(Texture2D));
            
            if (!AuthData.isAuthorized) AuthData.ClearPjktSession();
        }

        private void OnGUI() {

            //Display the PJKT logo
            GUI.DrawTexture(new Rect(0, 0, position.width, 128), pjktLogoBackgroundColor, ScaleMode.StretchToFill, false, 0); //Fill BG
            GUI.DrawTexture(new Rect(0, 0, 128, 128), pjktLogoBackgroundL, ScaleMode.ScaleToFit, false, 0); //Draw left side
            GUI.DrawTexture(new Rect(position.width - 128, 0, 128, 128), pjktLogoBackgroundR, ScaleMode.ScaleToFit, false, 0); //Draw right side
            GUI.DrawTexture(new Rect(position.width / 2 - 128, 0, 256, 128), pjktLogoText, ScaleMode.ScaleToFit, true, 0); //Draw text
            GUILayout.Space(128);

            

            //Force current screen to the login screen if we are not logged in or the current screen is null
            if (currentWindow == null || !AuthData.isAuthorized && !(currentWindow is PJKTLoginScreen)) SwitchTo<PJKTLoginScreen>();

            //If current screen is a login screen, don't display anything else
            if (currentWindow is PJKTLoginScreen) {
                scrollPos = GUILayout.BeginScrollView(scrollPos, false, false);
                currentWindow.OnGUI();
                GUILayout.Space(16);
                GUILayout.EndScrollView();
            }
            else
            {
                DrawGreeting();
                currentWindow.OnGUI();
            }
        }

        private void DrawGreeting()
        {
            GUIStyle communityNameStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, alignment = TextAnchor.UpperRight, normal = { textColor = Color.yellow } };
            
            //Greeting
            GUILayout.Space(16);
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("Welcome to the projekt");
                GUILayout.Label(AuthData.displayName, communityNameStyle);
                GUILayout.FlexibleSpace();
            }
            GUILayout.Space(16);

            //Screen swappers
            //Show a tab for each of the Windows specified in the tabs array
            tabNumber = GUILayout.SelectionGrid(tabNumber, tabNames, tabNames.Length);
            switch (tabNumber)
            {
                case 0:
                    if (currentWindow.GetType() != typeof(PJKTCustomBoothWindow)) SwitchTo<PJKTCustomBoothWindow>();
                    break;
                case 1:
                    if (currentWindow.GetType() != typeof(PJKTAccountWindow)) SwitchTo<PJKTAccountWindow>();
                    break;
            }
        }

        private void DoTabGui<T>(T window) where T : PJKTWindow
        {
            window.OnGUI();
        }
        

        //Used for switching between screens, and creating new ones if they haven't been created yet
        //Please note the 2 versions, SwitchTo<T> and SwitchTo(Type), both do the same thing but
        //the compiler doesn't like it when you try to use SwitchTo<T> with an instance of a class
        //and it doesn't like it when you try to use SwitchTo(Type) with a class type
        //It's fucking stupid, I know
        //THIS VERSION WORKS FOR INSTANCES, NOT CLASSES
        internal PJKTWindow SwitchTo(Type type) {
           
            PJKTWindow newScreen = openWindows.Find(s => s.GetType() == type);
            
            if (newScreen == null) {
                newScreen = (PJKTWindow)Activator.CreateInstance(type, this);
                newScreen.mainWindow = this;
                openWindows.Add(newScreen);
            }

            if (currentWindow != null) currentWindow.OnClose();
            currentWindow = newScreen;
            newScreen.OnOpen();

            return newScreen;
        }
        //THIS VERSION WORKS FOR CLASSES, NOT INSTANCES
        internal PJKTWindow SwitchTo<T>() where T : PJKTWindow {
            return SwitchTo(typeof(T));
        }

        private void OnSelectionChange() {
            //Repaint();
        }

        private void OnFocus()
        {
            if (currentWindow != null) currentWindow.OnFocus();
        }
    }
}