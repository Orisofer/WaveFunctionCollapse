using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFC
{
    public class GridCell : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer m_SpriteRenderer;
        
        private Tile m_CurrentTile;
        private bool m_Collapsed;
    }
}

