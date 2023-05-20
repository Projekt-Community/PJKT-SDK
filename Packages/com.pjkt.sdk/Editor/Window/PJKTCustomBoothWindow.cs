using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace PJKT.SDK.Window
{
    internal class PJKTCustomBoothWindow : PJKTWindow
    {
        private Vector2 boothScrollView;
        private BoothDescriptor[] boothsInScene;
        private BoothDescriptor selectedBooth;

        private bool boothfoldout = false;
        
        //treeview stuff
        private TreeViewState boothTreeState;
        private BoothValidationTreeView boothTree;
        private MultiColumnHeader boothTreeHeader;
        private MultiColumnHeaderState boothTreeHeaderState;
        
        internal override void OnGUI()
        {
            string boothname = "None";
            bool validBoothSelected = true;
            
            //check if booth got yeeted
            if (selectedBooth == null)
            {
                validBoothSelected = GetAllBooths();
                if (validBoothSelected) GetBoothPerformanceMetrics();
            }
            else boothname = selectedBooth.gameObject.name;

            

            //buttons
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button(new GUIContent("Select booth")))
                {
                    boothfoldout = !boothfoldout;
                }

                string errorTooltip = string.Empty;
                if (BoothValidator.Report == null || BoothValidator.Report.Overallranking <= 0)
                {
                    errorTooltip = "Your booth must be Ok or better in every category";
                    GUI.enabled = false;
                }
                if (GUILayout.Button(new GUIContent("Upload Booth", errorTooltip)))
                {
#pragma warning disable CS4014
                    BoothUploader.UploadBoothAsync(selectedBooth);
#pragma warning restore CS4014
                }
                GUI.enabled = true;
            }
            
            //booth selector
            if (boothfoldout)
            {
                using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    DrawBoothSelectionButtons();
                }
            }
            
            GUILayout.Space(5f);
            
            //top label
            using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(new GUIContent("Selected Booth: " + boothname), EditorStyles.whiteLargeLabel);
                GUILayout.FlexibleSpace();
            }
            
            if (BoothValidator.Report != null && validBoothSelected) DrawBoothTreeView();
        }

        private void DrawBoothSelectionButtons()
        {
            using (var scrollview = new GUILayout.ScrollViewScope(boothScrollView, GUILayout.Height(Screen.width/2)))
            {
                boothScrollView = scrollview.scrollPosition;
                GUILayout.BeginHorizontal();
                int rows = 1;
                for (int i = 0; i < boothsInScene.Length; i++)
                {
                    if (boothsInScene[i] == null) continue;
                    //button
                    GUILayout.BeginVertical();
                    if (GUILayout.Button(new GUIContent(AssetPreview.GetAssetPreview(boothsInScene[i].gameObject)), GUILayout.Width((Screen.width / 4) - 10f), GUILayout.Height((Screen.width / 4) - 10f)))
                    {
                        selectedBooth = boothsInScene[i];
                        boothfoldout = !boothfoldout;
                        GetBoothPerformanceMetrics();
                    }
                    string boothName = boothsInScene[i].gameObject.name;

                    //label
                    GUILayout.Label(boothName, EditorStyles.boldLabel, GUILayout.Width((Screen.width / 4) - 10f));

                    GUILayout.EndVertical();
                    GUILayout.FlexibleSpace();
                    
                    //check if we need to go to a new line
                    if (i + 1 == (4 * rows))
                    {
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        rows++;
                    }
                }
                GUILayout.EndHorizontal();
            }
        }

        private void DrawBoothTreeView()
        {
            //need to figure out offset
            Rect rect = EditorGUILayout.BeginVertical();
            float offset = (Screen.height - rect.y)-276;
            
            if (boothfoldout) offset -= (Screen.width / 2) + 8;
                
            GUILayout.Space(offset);
            boothTreeHeader.ResizeToFit();
            boothTree.OnGUI(rect);
            EditorGUILayout.EndVertical();
        }
        
        internal override void OnOpen()
        {
            InitWindow();
        }

        internal void InitWindow()
        {
            if (GetAllBooths()) BoothValidator.GenerateReport(selectedBooth);
            else BoothValidator.Report = new BoothValidationReport();
            
            //plant trees

            var columns = new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Booth Requirements"),
                    headerTextAlignment = TextAlignment.Center,
                    width = 200,
                    canSort = false,
                    sortedAscending = true,
                    autoResize = true,
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Perf", "Performance Ranking for Category"),
                    headerTextAlignment = TextAlignment.Center,
                    canSort = false,
                    sortedAscending = true,
                    width = 50,
                    minWidth = 30,
                    maxWidth = 50,
                    autoResize = false,
                },
            }; 
            boothTreeHeaderState = new MultiColumnHeaderState(columns);
            boothTreeHeader = new MultiColumnHeader(boothTreeHeaderState)
            {
                height = 30,
            };
            boothTreeHeader.ResizeToFit();
            
            boothTreeState = new TreeViewState();
            boothTree = new BoothValidationTreeView(boothTreeState, boothTreeHeader);
            boothTree.LoadTextures();
            boothTree.Reload();

        }
        internal override void OnFocus()
        {
            //refresh tree views here
            GetAllBooths();
            if (selectedBooth != null) BoothValidator.GenerateReport(selectedBooth);
            else BoothValidator.Report = new BoothValidationReport();
            boothTree.Reload();
        }

        private bool GetAllBooths()
        {
            boothsInScene = new BoothDescriptor[0];
            boothsInScene = Resources.FindObjectsOfTypeAll(typeof(BoothDescriptor)) as BoothDescriptor[];
            if (selectedBooth == null) selectedBooth = boothsInScene.Length > 0 ? boothsInScene[0] : null;
            return selectedBooth != null;
        }

        private void GetBoothPerformanceMetrics()
        {
            if (selectedBooth == null) return;
            BoothValidator.GenerateReport(selectedBooth);
            boothTree.Reload();
        }
    }
}