using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace WFC
{
    public class WaveFunctionCollapse : MonoBehaviour
    {
        [SerializeField] private GridCell m_GridCellPrefab;
        [SerializeField] private Tile[] m_Tiles;

        private Dictionary<Vector2Int, GridCell> m_GridCells;
        private Transform m_GridHolder;
        private Heap<GridCell> m_ModifiedCells;
        private ITileSelectionStrategy m_TileSelectionStrategy;
        
        private int m_GridWidth;
        private int m_GridHeight;
        private int m_NumCollapsed;

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
            
            m_GridCells = new Dictionary<Vector2Int, GridCell>(m_GridWidth * m_GridHeight);
        }

        private void InitCellGrid()
        {
            m_GridHolder = CreateGridHolder();
                
            for (int x = 0; x < m_GridWidth; x++)
            {
                for (int y = 0; y < m_GridHeight; y++)
                {
                    GridCell newCell = Instantiate(m_GridCellPrefab, m_GridHolder);
                    
                    Vector3 position = new Vector3(x, y, 0);
                    newCell.transform.position = position;
                    newCell.name = $"GridCell:({x},{y})";

                    Vector2Int cellPosition = new Vector2Int((int)position.x, (int)position.y);
                    
                    newCell.InitCell(cellPosition, m_Tiles.ToList());
                    
                    m_GridCells.Add(cellPosition, newCell);
                }
            }
        }

        private void InitHeap()
        {
            // init the heap data structure
            m_ModifiedCells = new Heap<GridCell>(m_GridWidth * m_GridHeight);
            
            // get a random cell to start with
            int randomStartIndexX = Random.Range(0, m_GridWidth);
            int randomStartIndexY = Random.Range(0, m_GridHeight);
            
            Vector2Int randomCellPosition = new Vector2Int(randomStartIndexX, randomStartIndexY);
            
            // push it to the heap for the first iteration
            m_ModifiedCells.Push(m_GridCells[randomCellPosition]);
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
            Vector2Int cellPosition = new Vector2Int(cell.Position.x, cell.Position.y);
            
            switch (direction)
            {
                case CellDirection.Up:
                    
                    Vector2Int dirUp = new Vector2Int(cellPosition.x, cellPosition.y + 1);
                    if (!m_GridCells.TryGetValue(dirUp, out GridCell neighborUp)) break;
                    return neighborUp;
                
                case CellDirection.Down:
                    
                    Vector2Int dirDown = new Vector2Int(cellPosition.x, cellPosition.y - 1);
                    if (!m_GridCells.TryGetValue(dirDown, out GridCell neighborDown)) break;
                    return neighborDown;
                
                case CellDirection.Left:
                    
                    Vector2Int dirLeft = new Vector2Int(cellPosition.x - 1, cellPosition.y);
                    if (!m_GridCells.TryGetValue(dirLeft, out GridCell neighborLeft)) break;
                    return neighborLeft;
                
                case CellDirection.Right:
                    
                    Vector2Int dirRight = new Vector2Int(cellPosition.x + 1, cellPosition.y);
                    if (!m_GridCells.TryGetValue(dirRight, out GridCell neighborRight)) break;
                    return neighborRight;
            }
            
            return null;
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

            foreach (KeyValuePair<Vector2Int, GridCell> cell in m_GridCells)
            {
                cell.Value.InitCell(cell.Value.Position, m_Tiles.ToList());
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