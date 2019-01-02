namespace NaniWeb.Data
{
    public class MangadexChapter
    {
        public int Id { get; set; }
        public int ChapterId { get; set; }
        public int MangadexId { get; set; }

        public Chapter Chapter { get; set; }
    }
}