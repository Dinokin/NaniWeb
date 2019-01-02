using System;

namespace NaniWeb.Data
{
    public class Page
    {
        public Guid Id { get; set; }
        public int PageNumber { get; set; }
        public int ChapterId { get; set; }

        public Chapter Chapter { get; set; }
    }
}