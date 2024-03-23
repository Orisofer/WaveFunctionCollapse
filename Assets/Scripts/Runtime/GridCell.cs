using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaveFunctionCollapse
{
    public class GridCell : MonoBehaviour
    {
        private Tile m_CurrentTile;
        private SpriteRenderer m_SpriteRenderer;

        private void Awake()
        {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}

