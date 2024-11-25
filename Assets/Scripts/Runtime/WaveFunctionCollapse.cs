using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WFC
{
    public class WaveFunctionCollapse : MonoBehaviour
    {
        [SerializeField] private Tile[] Tiles;
        
        private Dictionary<GridCell, List<Tile>> m_AvailableTilesForCell = new Dictionary<GridCell, List<Tile>>();
        private List<GridCell> m_Cells = new List<GridCell>();
        
        private int m_GridWidth;
        private int m_GridHeight;
        private int m_NumCollapsed;

        public bool Finished { get; private set; }
        
        //TODO: 2) lakes doesn't generate properly, lots of misses. good guess is to check from what source the algorithm-
        //TODO: -moves to the next cell to collapse (currently its from all the cells, maybe should be a queue or stack
        
        public void SetGridDimentions(int width, int height)
        {
            Finished = false;
            
            m_GridWidth = width;
            m_GridHeight = height;
            m_NumCollapsed = 0;
        }
        
        public async UniTask<bool> GenerateAuto()
        {
            // // pick cell to collapse
            // GridCell currentCell = GetRandomCellFromList(m_Cells);
            // NextCollapse(currentCell);

            while (m_NumCollapsed < m_Cells.Count)
            {
                // pick the next cell with the lowest entropy
                GridCell currentCell = GetLowestEntropyCell();
                NextCollapse(currentCell);
                await UniTask.Delay(5);
            }

            if (m_NumCollapsed == m_Cells.Count)
            {
                Debug.Log("WFC: Success!");
                Finished = true;
                return true;
            }
            
            Finished = true;
            return false;
        }

        public bool GenerateStep()
        {
            GridCell currentCell = GetLowestEntropyCell();
            NextCollapse(currentCell);
            
            if (m_NumCollapsed == m_Cells.Count)
            {
                Debug.Log("WFC: Success!");
                return true;
            }
            
            return false;
        }

        private void NextCollapse(GridCell currentCell)
        {
            // collapse cell
            currentCell.Collapse();
            m_NumCollapsed++;
            
            // propagate to neighbors
            Dictionary<CellDirection, GridCell> neighbors = GetCellNeighbors(currentCell);

            foreach (KeyValuePair<CellDirection, GridCell> neighbor in neighbors)
            {
                if (neighbor.Value == null) continue;

                AdjustAvailableTilesOnNeighbor(neighbor, currentCell);
            }
        }
        
        public void InitCell(GameObject cellGameObject, Vector2Int position)
        {
            GridCell newGridCell = cellGameObject.GetComponent<GridCell>();
            m_Cells.Add(newGridCell);
            
            newGridCell.InitCell(position, Tiles);
        }

        private GridCell GetLowestEntropyCell()
        {
            List<GridCell> newCells = new List<GridCell>();
            
            for (int i = 0; i < m_Cells.Count; i++)
            {
                if (m_Cells[i].Collapsed) continue;
                newCells.Add(m_Cells[i]);
            }

            // this return null and invoke the recursion base case to stop
            if (newCells.Count == 0)
            {
                return null;
            }

            GridCell newCell = newCells.OrderBy(cell => cell.AvailableTiles.Length).FirstOrDefault();
            
            // this return null and invoke the recursion base case to stop
            if (newCell == null)
            {
                return null;
            }

            return newCell;
        }

        private void AdjustAvailableTilesOnNeighbor(KeyValuePair<CellDirection, GridCell> neighbor, GridCell currentCell)
        {
            Tile currentTile = currentCell.CurrentTile;
            List<Tile> newAvailableTiles = FilterByDirection(neighbor, currentTile);
            neighbor.Value.SetAvailableTiles(newAvailableTiles);
        }

        private List<Tile> FilterByDirection(KeyValuePair<CellDirection, GridCell> neighbor, Tile currentTile)
        {
            List<Tile> newAvailableTiles = new List<Tile>();
            
            switch (neighbor.Key)
            {
                case CellDirection.Up:
                    newAvailableTiles = neighbor.Value.AvailableTiles.Where(t => t.GetEdgeDown == currentTile.GetEdgeUp)
                        .ToList();
                    break;
                case CellDirection.Down:
                    newAvailableTiles = neighbor.Value.AvailableTiles.Where(t => t.GetEdgeUp == currentTile.GetEdgeDown)
                        .ToList();
                    break;
                case CellDirection.Left:
                    newAvailableTiles = neighbor.Value.AvailableTiles.Where(t => t.GetEdgeRight == currentTile.GetEdgeLeft)
                        .ToList();
                    break;
                case CellDirection.Right:
                    newAvailableTiles = neighbor.Value.AvailableTiles.Where(t => t.GetEdgeLeft == currentTile.GetEdgeRight)
                        .ToList();
                    break;
            }

            return newAvailableTiles;
        }

        private Dictionary<CellDirection, GridCell> GetCellNeighbors(GridCell firstCell)
        {
            Dictionary<CellDirection, GridCell> neighbors = new Dictionary<CellDirection, GridCell>()
            {
                {CellDirection.Up, GetCellInDirection(firstCell, CellDirection.Up)},
                {CellDirection.Down, GetCellInDirection(firstCell, CellDirection.Down)},
                {CellDirection.Left, GetCellInDirection(firstCell, CellDirection.Left)},
                {CellDirection.Right, GetCellInDirection(firstCell, CellDirection.Right)},
            };

            return neighbors;
        }

        private GridCell GetCellInDirection(GridCell cell, CellDirection direction)
        {
            switch (direction)
            {
                case CellDirection.Up:
                    
                    if (cell.Position.y >= m_GridHeight) break;
                    return m_Cells.FirstOrDefault(c => c.Position.y == cell.Position.y + 1 && c.Position.x == cell.Position.x);
                
                case CellDirection.Down:
                    
                    if (cell.Position.y <= 0) break;
                    return m_Cells.FirstOrDefault(c => c.Position.y == cell.Position.y - 1 && c.Position.x == cell.Position.x);
                
                case CellDirection.Left:
                    
                    if (cell.Position.x <= 0) break;
                    return m_Cells.FirstOrDefault(c => c.Position.x == cell.Position.x - 1 && c.Position.y == cell.Position.y);
                
                case CellDirection.Right:
                    
                    if (cell.Position.x >= m_GridWidth) break;
                    return m_Cells.FirstOrDefault(c => c.Position.x == cell.Position.x + 1 && c.Position.y == cell.Position.y);
            }
            
            return null;
        }
    }
}