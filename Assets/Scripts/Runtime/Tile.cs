using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Texture2D Texture;

    [SerializeField] private string EdgeUp;
    [SerializeField] private string EdgeDown;
    [SerializeField] private string EdgeLeft;
    [SerializeField] private string EdgeRight;

    private Dictionary<string, string> m_EdgeDefenitions;

    private void Awake()
    {
        m_EdgeDefenitions = new Dictionary<string, string>()
        {
            {TileUtility.UP, EdgeUp},
            {TileUtility.DOWN, EdgeDown},
            {TileUtility.LEFT, EdgeLeft},
            {TileUtility.RIGHT, EdgeRight},
        };
    }
}
