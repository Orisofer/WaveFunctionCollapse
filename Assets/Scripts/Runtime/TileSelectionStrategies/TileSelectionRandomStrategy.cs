using UnityEngine;
using WFC;

public class TileSelectionRandomStrategy : TileSelectionStrategy
{
    public override Tile GetTile(Tile[] tiles)
    {
        int index = Random.Range(0, tiles.Length - 1);
        return tiles[index];
    }
}
