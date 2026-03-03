using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJKT.SDK2
{
    public class HelpLinks
    {
        public const string DocumentationLink = "https://docs.projektcommunity.com/";
        public const string DiscordLink = "https://discord.com/channels/1391918011824345109/1391953951292788786";
        public const string OfficialTutorialsLink = "https://www.youtube.com/@PJKT"; //temp
        public const string CommunityTutorialsLink = "https://www.youtube.com/@PJKT"; //temp
        
        [UnityEditor.MenuItem("PJKT SDK/Help/Documentation", priority = 20)]
        public static void OpenDocumentation() => Application.OpenURL(DocumentationLink);
        [UnityEditor.MenuItem("PJKT SDK/Help/Discord Helpdesk", priority = 21)]
        public static void OpenDiscord() => Application.OpenURL(DiscordLink);
        [UnityEditor.MenuItem("PJKT SDK/Help/Official Tutorials", priority = 22)]
        public static void OpenOfficialTutorials() => Application.OpenURL(OfficialTutorialsLink);
        [UnityEditor.MenuItem("PJKT SDK/Help/Community Tutorials", priority = 23)]
        public static void OpenCommunityTutorials() => Application.OpenURL(CommunityTutorialsLink);
    }
}