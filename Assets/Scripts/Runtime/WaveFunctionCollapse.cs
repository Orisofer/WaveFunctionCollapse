using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WFC
{
    public class WaveFunctionCollapse : MonoBehaviour
    {
        [SerializeField] private Tile[] Tiles;
        
        private Dictionary<GridCell, List<Tile>> m_AvailableTilesForCell = new Dictionary<GridCell, List<Tile>>();
        private List<GridCell> m_Cells = new List<GridCell>();

        public bool Finished { get; }

        public void InitCell(GameObject cellGameObject, Vector2Int position)
        {
            GridCell newGridCell = cellGameObject.GetComponent<GridCell>();
            m_Cells.Add(newGridCell);
            
            newGridCell.InitCell(position, Tiles.ToList());
        }
        
        public bool Generate()
        {
            GridCell firstCell = GetRandomCellFromList(m_Cells);

            firstCell.Collapse();
            
            return true;
        }

        private GridCell GetRandomCellFromList(List<GridCell> cells)
        {
            int randomIndex = Random.Range(0, cells.Count - 1);
            GridCell gridCell = cells[randomIndex];
            
            return gridCell;
        }
    }
}

