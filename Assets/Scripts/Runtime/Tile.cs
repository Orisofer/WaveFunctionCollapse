using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFC
{
    [CreateAssetMenu(fileName = "Tile", menuName = "Wave Function Collapse/Tile")]
    public class Tile : ScriptableObject
    {
        [SerializeField] private Sprite TileSprite;

        [SerializeField] private EdgeType EdgeUp;
        [SerializeField] private EdgeType EdgeDown;
        [SerializeField] private EdgeType EdgeLeft;
        [SerializeField] private EdgeType EdgeRight;
        
        public EdgeType GetEdgeUp => EdgeUp;
        public EdgeType GetEdgeDown => EdgeDown;
        public EdgeType GetEdgeLeft => EdgeLeft;
        public EdgeType GetEdgeRight => EdgeRight;
        

        private Dictionary<string, EdgeType> m_EdgeDefenitions;
        public Sprite Sprite => TileSprite;

        private void Awake()
        {
            m_EdgeDefenitions = new Dictionary<string, EdgeType>()
            {
                {TileUtility.UP, EdgeUp},
                {TileUtility.DOWN, EdgeDown},
                {TileUtility.LEFT, EdgeLeft},
                {TileUtility.RIGHT, EdgeRight},
            };
        }
    }
}