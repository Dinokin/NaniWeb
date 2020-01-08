using System.Collections.Generic;

namespace NaniWeb.Data
{
    public class Series
    {
        public enum SeriesStatus
        {
            Ongoing,
            Hiatus,
            Dropped,
            Completed
        }

        public enum SeriesType
        {
            Manga,
            Webtoon
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Artist { get; set; }
        public string Synopsis { get; set; }
        public SeriesType Type { get; set; }
        public SeriesStatus Status { get; set; }
        public string UrlSlug { get; set; }

        public List<Chapter> Chapters { get; set; }
    }
}