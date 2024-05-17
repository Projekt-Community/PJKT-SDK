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
        public object deletedAt;
        public object logo_id;
        public object group_photo_id;
        public object poster_id;
        public object venue_id;
    }

    [Serializable]
    public class ProjectsData
    {
        public List<Project> projects;
    }
}