using System.Collections.Generic;
using PJKT.SDK2.NET;
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
            
            Debug.Log($"events response: \n{response}");

            ProjectsData data = JsonUtility.FromJson<ProjectsData>(response);
            Projekts = new List<Project>(data.projects);

            //foreach (Project pjktProjekt in AllProjekts.projects) pjktProjekt.GetHeaderImage();

        }
    }
}