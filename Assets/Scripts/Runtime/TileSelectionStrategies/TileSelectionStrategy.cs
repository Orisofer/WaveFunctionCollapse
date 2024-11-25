using WFC;

public abstract class TileSelectionStrategy : ITileSelectionStrategy
{
    public abstract Tile GetTile(Tile[] tiles);
}
