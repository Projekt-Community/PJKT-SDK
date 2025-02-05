using System;
using System.Collections.Generic;

namespace PJKT.SDK2
{
    [Serializable]
    public class Project
    {
        public BoothRequirements booth_requirements;
        public int id;
        public string name;
        public string start_date;
        public string end_date;
        public bool accepting_booth;
        public bool accepting_events;
        public string booth_deadline_date;
        public string events_deadline_date;
        public string createdAt;
        public string updatedAt;
        public string deletedAt;
        public int logo_id;
        public int group_photo_id;
        public int poster_id;
        public int venue_id;
        public PjktRemoteImage Logo;
    }

    [Serializable]
    public class ProjectsData
    {
        public List<Project> projects;
    }

    [Serializable]
    public class PjktRemoteImage
    {
        public int id;
        public string name;
        public string path;
        public string createdAt;
        public string updatedAt;
        public string deletedAt;
    }
}