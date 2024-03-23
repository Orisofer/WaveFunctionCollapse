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

        private List<Tile> m_AvailableTiles;
        private Tile m_CurrentTile;
        private Vector2Int m_Position;
        private bool m_Collapsed;

        public List<Tile> AvailableTiles => m_AvailableTiles;
        public Tile CurrentTile => m_CurrentTile;
        public Vector2Int Position => m_Position;
        public bool Collapsed => m_Collapsed;

        public void InitCell(Vector2Int position, List<Tile> availableTiles)
        {
            m_Collapsed = false;
            m_Position = position;
            m_AvailableTiles = availableTiles;
        }

        public void Collapse()
        {
            Tile tile = GetTileFromAvailable();
            m_SpriteRenderer.sprite = tile.Sprite;
            m_SpriteRenderer.color = Color.white;
            m_CurrentTile = tile;
            m_AvailableTiles.Clear();
            m_Collapsed = true;
        }

        private Tile GetTileFromAvailable()
        {
            int index = Random.Range(0, m_AvailableTiles.Count - 1);
            return m_AvailableTiles[index];
        }

        public void SetAvailableTiles(List<Tile> tiles)
        {
            m_AvailableTiles.Clear();
            m_AvailableTiles = tiles;
        }
    }
}

