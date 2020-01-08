using System;
using System.Collections.Generic;

namespace NaniWeb.Data
{
    public class Chapter
    {
        public int Id { get; set; }
        public int Volume { get; set; }
        public decimal ChapterNumber { get; set; }
        public string Name { get; set; }
        public int SeriesId { get; set; }
        public DateTime ReleaseDate { get; set; }

        public Series Series { get; set; }
        public List<Page> Pages { get; set; }
    }
}