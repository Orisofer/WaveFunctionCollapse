using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace WFC
{
    public class WaveFunctionCollapse : MonoBehaviour
    {
        [SerializeField] private GameObject m_GridCellPrefab;
        [SerializeField] private Tile[] m_Tiles;

        private List<GridCell> m_GridCells;
        private Transform m_GridHolder;
        private Heap<GridCell> m_ModifiedCells;
        private ITileSelectionStrategy m_TileSelectionStrategy;
        
        private int m_GridWidth;
        private int m_GridHeight;
        private int m_NumCollapsed;
        private bool m_GridReady;

        // API Call for grid initialization
        public void Initialize(int width, int height, ITileSelectionStrategy choosingStrategy)
        {
            SetGridDimensions(width, height);
            InitCellGrid();
            InitHeap();
            SetTileChoosingStrategy(choosingStrategy);
        }

        private void SetGridDimensions(int width, int height)
        {
            m_GridWidth = width;
            m_GridHeight = height;
            m_NumCollapsed = 0;
            
            m_GridCells = new List<GridCell>(m_GridWidth * m_GridHeight);

            m_GridReady = true;
        }

        private void InitCellGrid()
        {
            if (!m_GridReady)
            {
                Debug.Log("WFC: Trying to generate a grid with missing/invalid parameters");
                return;
            }

            CreateGridHolder();
                
            for (int x = 0; x < m_GridWidth; x++)
            {
                for (int y = 0; y < m_GridHeight; y++)
                {
                    GameObject newCellGameObject = Instantiate(m_GridCellPrefab, m_GridHolder);
                    
                    Vector3 position = new Vector3(x, y, 0);
                    newCellGameObject.transform.position = position;
                    newCellGameObject.name = $"GridCell:({x},{y})";
                    
                    InitCell(newCellGameObject, new Vector2Int((int)position.x, (int)position.y));
                }
            }
        }

        private void InitHeap()
        {
            // init the heap data structure
            m_ModifiedCells = new Heap<GridCell>(m_GridWidth * m_GridHeight);
            
            // get a random cell to start with
            int randomStartIndex = Random.Range(0, m_GridWidth * m_GridHeight);
            
            // push it to the heap for the first iteration
            m_ModifiedCells.Push(m_GridCells[randomStartIndex]);
        }
        
        private void SetTileChoosingStrategy(ITileSelectionStrategy choosingStrategy)
        {
            m_TileSelectionStrategy = choosingStrategy;
        }
        
        // API Call for auto generate
        public async UniTask GenerateAuto(int delay = 0, CancellationTokenSource cts = null, Action finishedCallback = null)
        {
            try
            {
                while (m_NumCollapsed < m_GridCells.Count)
                {
                    // pick the next cell with the lowest entropy
                    IterateWave();

                    if (delay != 0)
                    {
                        await UniTask.Delay(delay, cancellationToken: cts?.Token ?? CancellationToken.None);
                    }
                }

                if (CheckFinished())
                {
                    finishedCallback?.Invoke();
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("task cancelled");
            }
            finally
            {
                Debug.Log("task ended");
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

                IterateWave();
            }
        }

        private void IterateWave()
        {
            // Stopwatch stopwatch = new Stopwatch();
            // stopwatch.Start();
            GridCell currentCell = GetLowestEntropyCell();
            CollapseCell(currentCell);
            Propagate(currentCell);
            // stopwatch.Stop();
            // Debug.Log("Heap Iteration Time: " + stopwatch.ElapsedTicks);
        }

        private void CollapseCell(GridCell currentCell)
        {
            // collapse cell
            currentCell.Collapse(m_TileSelectionStrategy);
            m_NumCollapsed++;
        }

        private void Propagate(GridCell from)
        {
            Stack<GridCell> stack = new Stack<GridCell>();
            
            stack.Push(from);
            
            while (stack.Count > 0)
            {
                GridCell current = stack.Pop();
                
                // propagate to neighbors
                Dictionary<CellDirection, GridCell> neighbors = GetCellNeighbors(current);
                
                // create an array that holds all the tiles we need to check against the current (if its collapsed it's only 1)
                Tile[] tilesToCompare;

                if (current.Collapsed)
                {
                    tilesToCompare = new Tile[1];
                    tilesToCompare[0] = current.CurrentTile;
                }
                else
                {
                    tilesToCompare = current.AvailableTiles.ToArray();
                }

                foreach (KeyValuePair<CellDirection, GridCell> neighbor in neighbors)
                {
                    if (neighbor.Value == null) continue;

                    int modifiedResult = AdjustAvailableTilesOnNeighbor(neighbor, tilesToCompare);

                    // if we restricted one or more tiled at neighbor the propagation goes on
                    if (modifiedResult >= 1 && !stack.Contains(neighbor.Value))
                    {
                        m_ModifiedCells.Push(neighbor.Value);
                        stack.Push(neighbor.Value);
                    }
                }
            }
        }
        
        private int AdjustAvailableTilesOnNeighbor(KeyValuePair<CellDirection, GridCell> neighbor, Tile[] tilesToCompare)
        {
            // get all neighbors available tile set
            List<Tile> newAvailableTiles = neighbor.Value.AvailableTiles;
            
            // get all the valid types from the available tiles of the neighbors
            List<EdgeType> availableTypesAtDirection = new List<EdgeType>();

            for (int i = 0; i < tilesToCompare.Length; i++)
            {
                EdgeType type = tilesToCompare[i].GetTypeInDirection(neighbor.Key);

                if (!availableTypesAtDirection.Contains(type))
                {
                    availableTypesAtDirection.Add(type);
                }
            }
            
            // get the opposite direction to check against the tiles to compare
            CellDirection oppositeDirection = TileUtility.GetOppositeDirection(neighbor.Key);

            int removedItems = 0;
            
            // iterate over the available tiles and remove every tile that's not compatible with the list of tiles
            for (int i = newAvailableTiles.Count - 1; i >= 0; --i)
            {
                EdgeType matchingType = newAvailableTiles[i].GetTypeInDirection(oppositeDirection);
                
                if (!availableTypesAtDirection.Contains(matchingType))
                {
                    newAvailableTiles.Remove(newAvailableTiles[i]);
                    removedItems++;
                }
            }
            
            // now we should have a fresh new available tiles on the neighbor
            return removedItems;
        }

        private GridCell GetLowestEntropyCell()
        {
            GridCell newCell = m_ModifiedCells.Pop();
            
            // this return null and invoke the recursion base case to stop
            if (newCell == null)
            {
                return null;
            }

            return newCell;
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
                    return m_GridCells.FirstOrDefault(c => c.Position.y == cell.Position.y + 1 && c.Position.x == cell.Position.x);
                
                case CellDirection.Down:
                    
                    if (cell.Position.y <= 0) break;
                    return m_GridCells.FirstOrDefault(c => c.Position.y == cell.Position.y - 1 && c.Position.x == cell.Position.x);
                
                case CellDirection.Left:
                    
                    if (cell.Position.x <= 0) break;
                    return m_GridCells.FirstOrDefault(c => c.Position.x == cell.Position.x - 1 && c.Position.y == cell.Position.y);
                
                case CellDirection.Right:
                    
                    if (cell.Position.x >= m_GridWidth) break;
                    return m_GridCells.FirstOrDefault(c => c.Position.x == cell.Position.x + 1 && c.Position.y == cell.Position.y);
            }
            
            return null;
        }
        
        private void InitCell(GameObject cellGameObject, Vector2Int position)
        {
            GridCell newGridCell = cellGameObject.GetComponent<GridCell>();
            m_GridCells.Add(newGridCell);
            
            newGridCell.InitCell(position, m_Tiles.ToList());
        }

        private bool CheckFinished()
        {
            if (m_NumCollapsed == m_GridCells.Count)
            {
                Debug.Log("WFC: Success!");
                return true;
            }
            return false;
        }

        public void ClearData()
        {
            m_NumCollapsed = 0;

            for (int i = 0; i < m_GridCells.Count; i++)
            {
                m_GridCells[i].InitCell(m_GridCells[i].Position, m_Tiles.ToList());
            }

            InitHeap();
        }
        
        private Transform CreateGridHolder()
        {
            if (m_GridHolder == null)
            {
                // create grid holder game object
                GameObject gridHolder = new GameObject("GridHolder");
                gridHolder.transform.SetParent(transform);
                m_GridHolder = gridHolder.transform;
            }

            return m_GridHolder;
        }
    }
}