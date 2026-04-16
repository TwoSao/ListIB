namespace ListIB.Models
{
    public class Riik
    {
        public string Nimi { get; set; } = string.Empty;
        public string Pealinn { get; set; } = string.Empty;
        public long Rahvaarv { get; set; }
        public Microsoft.Maui.Controls.ImageSource? Lipp { get; set; }
        public string Lisainfo { get; set; } = string.Empty;
        public int Rating { get; set; } = 1;
        public double DevelopmentLevel { get; set; } = 0;
        public DateTime VisitDate { get; set; } = DateTime.Now;
        public bool IsFavorite { get; set; }
    }
}
