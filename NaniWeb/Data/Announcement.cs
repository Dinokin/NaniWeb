using System;

namespace NaniWeb.Data
{
    public class Announcement
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string UrlSlug { get; set; }
        public DateTime PostDate { get; set; }
    }
}