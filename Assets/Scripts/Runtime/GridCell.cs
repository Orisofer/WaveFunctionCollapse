using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WFC
{
    public class GridCell : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer m_SpriteRenderer;
        [SerializeField] private Tile m_ErrorTile;

        private Tile[] m_AvailableTiles;
        private Tile m_CurrentTile;
        private Vector2Int m_Position;
        private bool m_Collapsed;

        public Tile[] AvailableTiles => m_AvailableTiles;
        public Tile CurrentTile => m_CurrentTile;
        public Vector2Int Position => m_Position;
        public bool Collapsed => m_Collapsed;

        public void InitCell(Vector2Int position, Tile[] availableTiles)
        {
            m_Collapsed = false;
            m_Position = position;
            m_AvailableTiles = availableTiles;
        }

        public void Collapse()
        {
            Tile tile = GetTileFromAvailable();

            if (tile == null)
            {
                m_SpriteRenderer.sprite = m_ErrorTile.Sprite;
                m_CurrentTile = m_ErrorTile;
                m_SpriteRenderer.color = Color.magenta;
                m_AvailableTiles = Array.Empty<Tile>();
                m_Collapsed = true;
                
                return;
            }
            
            m_SpriteRenderer.sprite = tile.Sprite;
            m_SpriteRenderer.color = Color.white;
            m_CurrentTile = tile;
            m_AvailableTiles = Array.Empty<Tile>();
            m_Collapsed = true;
        }

        private Tile GetTileFromAvailable()
        {
            if (m_AvailableTiles.Length == 0)
            {
                Debug.Log("WFC: Zero Available tiles");
                return null;
            }
            
            int index = Random.Range(0, m_AvailableTiles.Length - 1);
            return m_AvailableTiles[index];
        }

        public void SetAvailableTiles(List<Tile> tiles)
        {
            m_AvailableTiles = Array.Empty<Tile>();
            m_AvailableTiles = tiles.ToArray();
        }
    }
}

