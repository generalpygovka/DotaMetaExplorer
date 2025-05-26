namespace DotaMetaExplorer.Models;

public class Subscribe
{
    public int Id { get; set; }
    public int FavouriteHeroId { get; set; }
    public int ChatId { get; set; }
    public int FavouriteTeamId { get; set; }
    public bool IsSubscribeForPatch { get; set; }
}
