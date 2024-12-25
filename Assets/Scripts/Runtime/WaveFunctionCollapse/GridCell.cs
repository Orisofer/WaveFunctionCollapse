using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WFC
{
    public class GridCell : MonoBehaviour, IHeapItem<GridCell>
    {
        [SerializeField] private SpriteRenderer m_SpriteRenderer;

        private List<Tile> m_AvailableTiles;
        private Tile m_CurrentTile;
        private Sprite m_DefaultTileTexture;
        private Vector2Int m_Position;
        private Color m_DefaultTileColor;

        public List<Tile> AvailableTiles => m_AvailableTiles;
        public Tile CurrentTile => m_CurrentTile;
        public Vector2Int Position => m_Position;
        public bool Collapsed { get; private set; }
        public int HeapIndex { get; set; }

        private void Awake()
        {
            m_DefaultTileTexture = m_SpriteRenderer.sprite;
            m_DefaultTileColor = m_SpriteRenderer.color;
        }

        public void InitCell(Vector2Int position, List<Tile> availableTiles)
        {
            Collapsed = false;
            m_CurrentTile = null;
            m_Position = position;
            
            m_SpriteRenderer.sprite = m_DefaultTileTexture;
            m_SpriteRenderer.color = m_DefaultTileColor;
            
            m_AvailableTiles = availableTiles;
        }

        public void Collapse(ITileSelectionStrategy strategy)
        {
            Tile tile = GetTileFromAvailable(strategy);

            if (!tile)
            {
                m_SpriteRenderer.sprite = m_DefaultTileTexture;
                m_SpriteRenderer.color = Color.magenta;
                m_AvailableTiles.Clear();
                Collapsed = true;
                
                return;
            }
            
            m_SpriteRenderer.sprite = tile.Sprite;
            m_SpriteRenderer.color = Color.white;
            m_CurrentTile = tile;
            m_AvailableTiles.Clear();
            Collapsed = true;
        }

        private Tile GetTileFromAvailable(ITileSelectionStrategy strategy)
        {
            if (m_AvailableTiles.Count == 0)
            {
                Debug.Log("WFC: Zero Available tiles");
                return null;
            }

            if (strategy == null)
            {
                Debug.Log("WFC: No strategy for choosing a tile was selected");
                return null;
            }

            return strategy.GetTile(m_AvailableTiles.ToArray());
        }

        public int CompareTo(GridCell other)
        {
            if (AvailableTiles.Count > other.AvailableTiles.Count)
            {
                return 1;
            }
            else if (AvailableTiles.Count < other.AvailableTiles.Count)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}

