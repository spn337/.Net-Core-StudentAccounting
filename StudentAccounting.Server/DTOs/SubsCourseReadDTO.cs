using System;

namespace StudentAccounting.Server.DTOs
{
    public class SubsCourseReadDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string StudyDate { get; set; }
        public int DaysToStudyCount { get; set; }
    }
}
