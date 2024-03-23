using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject GridCellPrefab;
    
    [SerializeField] private int GridWidth;
    [SerializeField] private int GridHeight;

    private Camera m_MainCamera;

    private void Awake()
    {
        m_MainCamera = Camera.main;
    }

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                GameObject newCell = Instantiate(GridCellPrefab, this.gameObject.transform);
                Vector3 position = new Vector3(x, y, 0);
                newCell.transform.position = position;
                newCell.name = $"GridCell:({x},{y})";
            }

            Vector3 alignCenter = new Vector3(((float)GridWidth / 2f) - 0.5f, ((float)GridHeight / 2f) - 0.5f, -10f);
            m_MainCamera.transform.position = alignCenter;
        }
    }
}
