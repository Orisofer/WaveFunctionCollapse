using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WFC;

public class AavailableTilesText : MonoBehaviour
{
    private TextMeshPro text;
    private GridCell cell;

    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
        cell = GetComponentInParent<GridCell>();
    }

    private void Update()
    {
        text.text = cell.AvailableTiles.Count.ToString();
    }
}
