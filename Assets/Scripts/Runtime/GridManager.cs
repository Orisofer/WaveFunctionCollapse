using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject GridCellPrefab;
    
    [SerializeField] private int GridWidth;
    [SerializeField] private int GridHeight;

    private float m_CellSize;

    private void Awake()
    {
        SpriteRenderer renderer = GridCellPrefab.GetComponent<SpriteRenderer>();
        m_CellSize = renderer.sprite.pixelsPerUnit;
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
        }

        Vector3 alignCenter = new Vector3(-1 * (float)(GridWidth / 2) + 0.5f, -1 * (float)(GridHeight / 2)+ 0.5f, 0);
        transform.position = alignCenter;
    }
}
