using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFC
{
    [CreateAssetMenu(fileName = "Tile", menuName = "Wave Function Collapse/Tile")]
    public class Tile : ScriptableObject
    {
        [SerializeField] private Texture2D Texture;

        [SerializeField] private EdgeType EdgeUp;
        [SerializeField] private EdgeType EdgeDown;
        [SerializeField] private EdgeType EdgeLeft;
        [SerializeField] private EdgeType EdgeRight;

        private Dictionary<string, EdgeType> m_EdgeDefenitions;

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


