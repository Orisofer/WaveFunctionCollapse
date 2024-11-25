using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WFC
{
    public class WaveFunctionCollapse : MonoBehaviour
    {
        [SerializeField] private GameObject m_GridCellprefab;
        [SerializeField] private Tile[] m_Tiles;

        private List<GridCell> m_Cells;
        private int m_GridWidth;
        private int m_GridHeight;
        private int m_NumCollapsed;
        private bool m_GridReady;
        private bool m_Finished;
        
        //TODO: 2) lakes doesn't generate properly, lots of misses. good guess is to check from what source the algorithm-
        //TODO: -moves to the next cell to collapse (currently its from all the cells, maybe should be a queue or stack

        // API Call for grid initialization
        public void GenerateGrid(int width, int height, out Vector3 gridCenterPoint)
        {
            SetGridDimentions(width, height);

            if (!m_GridReady)
            {
                Debug.Log("WFC: Trying to generate a grid with missing/invalid parameters");
                gridCenterPoint = Vector3.zero;
                return;
            }
            
            Vector3 alignCenter = new Vector3(((float)m_GridWidth / 2f) - 0.5f, ((float)m_GridHeight / 2f) - 0.5f, -10f);
            gridCenterPoint = alignCenter;
            
            // create grid holder game object
            GameObject gridHolder = new GameObject("GridHolder");
            gridHolder.transform.SetParent(transform);
                
            for (int x = 0; x < m_GridWidth; x++)
            {
                for (int y = 0; y < m_GridHeight; y++)
                {
                    GameObject newCellGameObject = Instantiate(m_GridCellprefab, gridHolder.transform);
                    
                    Vector3 position = new Vector3(x, y, 0);
                    newCellGameObject.transform.position = position;
                    newCellGameObject.name = $"GridCell:({x},{y})";
                    
                    InitCell(newCellGameObject, new Vector2Int((int)position.x, (int)position.y));
                }
            }
        }
        
        private void SetGridDimentions(int width, int height)
        {
            m_Finished = false;
            
            m_GridWidth = width;
            m_GridHeight = height;
            m_NumCollapsed = 0;
            
            m_Cells = new List<GridCell>(m_GridWidth * m_GridHeight);

            m_GridReady = true;
        }
        
        
        // API Call for auto generate
        public async UniTask GenerateAuto(int delay = 0, Action finishedCallback = null)
        {
            while (m_NumCollapsed < m_Cells.Count)
            {
                // pick the next cell with the lowest entropy
                GridCell currentCell = GetLowestEntropyCell();
                NextCollapse(currentCell);
                await UniTask.Delay(delay);
            }

            if (CheckFinished())
            {
                finishedCallback?.Invoke();
            }
        }

        // API Call for generate step by step
        public void GenerateStep(int numSteps = 1, Action finishedCallback = null)
        {
            for (int i = 0; i < numSteps; i++)
            {
                if (CheckFinished())
                {
                    finishedCallback?.Invoke();
                    break;
                }
                
                GridCell currentCell = GetLowestEntropyCell();
                NextCollapse(currentCell);
            }
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
        
        private void InitCell(GameObject cellGameObject, Vector2Int position)
        {
            GridCell newGridCell = cellGameObject.GetComponent<GridCell>();
            m_Cells.Add(newGridCell);
            
            newGridCell.InitCell(position, m_Tiles);
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

        private Dictionary<CellDirection, GridCell> GetCellNeighbors(GridCell cell)
        {
            Dictionary<CellDirection, GridCell> neighbors = new Dictionary<CellDirection, GridCell>()
            {
                {CellDirection.Up, GetCellInDirection(cell, CellDirection.Up)},
                {CellDirection.Down, GetCellInDirection(cell, CellDirection.Down)},
                {CellDirection.Left, GetCellInDirection(cell, CellDirection.Left)},
                {CellDirection.Right, GetCellInDirection(cell, CellDirection.Right)},
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

        private bool CheckFinished()
        {
            if (m_NumCollapsed == m_Cells.Count)
            {
                Debug.Log("WFC: Success!");
                m_Finished = true;
                return true;
            }
            
            Debug.Log("WFC: Failed!");
            return false;
        }
    }
}