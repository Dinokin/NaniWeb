namespace NaniWeb.Data
{
    public class MangadexSeries
    {
        public int Id { get; set; }
        public int SeriesId { get; set; }
        public int MangadexId { get; set; }

        public Series Series { get; set; }
    }
}