using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFC;

public class TileSelectionWeightedRandomStrategy : TileSelectionStrategy
{
    public override Tile GetTile(Tile[] tiles)
    {
        int totalWeight = 0;

        // sum all the weights
        for (int i = 0; i < tiles.Length; i++)
        {
            totalWeight += tiles[i].Weight;
        }
        
        // get a random value in range of weights
        int randomValue = Random.Range(0, totalWeight);
        
        // increment tiles to reach random value
        int cumulativeWeight = 0;

        for (int i = 0; i < tiles.Length; i++)
        {
            cumulativeWeight += tiles[i].Weight;
            
            if (cumulativeWeight >= randomValue)
            {
                return tiles[i];
            }
        }
        
        Debug.Log("Random Weighted Selection Failed - first selection was retured");
        return tiles[0];
    }
}
