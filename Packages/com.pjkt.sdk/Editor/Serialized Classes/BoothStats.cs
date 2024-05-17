using System.Collections.Generic;
using System.Text;

namespace PJKT.SDK2
{
    public class BoothStats
    {
        public readonly StatsType Type;
        public readonly BoothPerformanceRanking PerformanceRank;
        public readonly string DetailsString;
        public readonly string RequirementsString;
        public readonly List<object> ComponentList;

        public BoothStats(StatsType statsType, BoothPerformanceRanking boothPerformanceRanking, string details, string requirements, List<object> components)
        {
            Type = statsType;
            PerformanceRank = boothPerformanceRanking;
            DetailsString = details;
            RequirementsString = requirements;
            ComponentList = components;
        }
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Type: {Type}\n");
            sb.Append($"Performance Rank: {PerformanceRank}\n");
            sb.Append($"Details: {DetailsString}\n");
            sb.Append($"Requirements: {RequirementsString}\n");
            sb.Append($"Components: {ComponentList.Count}");


            return sb.ToString();
        }
    }
}