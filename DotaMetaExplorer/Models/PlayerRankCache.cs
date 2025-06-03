namespace DotaMetaExplorer.Models
{
    public class PlayerRankCache
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string PersonaName { get; set; } = string.Empty;
        public int LeaderboardRank { get; set; }
    }
}
