using System;
using System.Collections.Generic;
using PJKT.SDK2.NET;
using UnityEditor;
using UnityEngine;

/// <summary>
/// This handels getting all the events
/// </summary>
namespace PJKT.SDK2
{
    internal static class PjktEventManager
    {
        public static Project SelectedProjekt { get; set; }
        public static List<Project> Projekts = new List<Project>();
        
        public static async void GetEvents()
        {
            Projekts.Clear();

            string response = await PJKTNet.RequestMessage("/projects");
            if (string.IsNullOrEmpty(response)) return;
            
            //Debug.Log($"events response: \n{response}");

            ProjectsData data = JsonUtility.FromJson<ProjectsData>(response);
            Projekts = new List<Project>(data.projects);
            
            //grab the latest event and auto select it
            if (Projekts.Count <= 0) return;

            DateTime latest = DateTime.MinValue;
            Project currentProject = null;
            foreach (var evt in Projekts)
            {
                DateTime deadline = DateTime.Parse(evt.booth_deadline_date);   
                if (deadline <= latest) continue;
                
                latest = deadline;
                currentProject = evt;
            }

            //if (latest < DateTime.Now) return;
            if (currentProject == null) return;
            SelectedProjekt = currentProject;
            Vector3 bounds = new  Vector3(currentProject.booth_requirements.MaxDims[0], currentProject.booth_requirements.MaxDims[1], currentProject.booth_requirements.MaxDims[2]);
            BoothDescriptor._maxBounds = (bounds);
            BoothValidator.Requirements = currentProject.booth_requirements;
            PjktSdkWindow window = EditorWindow.GetWindow<PjktSdkWindow>();
            if (window != null) window.SetEvent(currentProject);
        }
    }
}