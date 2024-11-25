using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace WFC
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private GameObject GridCellPrefab;
        [SerializeField] private WaveFunctionCollapse m_WaveFuncionCollapse;
        [SerializeField] private InputManager m_InputManager;
        
        [SerializeField] private int GridWidth;
        [SerializeField] private int GridHeight;
        [SerializeField] private bool m_ManualCollapse = false;

        private Camera m_MainCamera;

        private void Awake()
        {
            m_MainCamera = Camera.main;
            m_WaveFuncionCollapse.SetGridDimentions(GridWidth, GridHeight);
        }

        private async void Start()
        {
            GenerateGrid();
            
            if (!m_ManualCollapse)
            {
                await m_WaveFuncionCollapse.GenerateAuto();
            }
        }

        private void Update()
        {
            if (!m_ManualCollapse) return;

            if (m_InputManager.ManualGenerate)
            {
                m_WaveFuncionCollapse.GenerateStep();
            }
        }

        private void GenerateGrid()
        {
            Vector3 alignCenter = new Vector3(((float)GridWidth / 2f) - 0.5f, ((float)GridHeight / 2f) - 0.5f, -10f);
            m_MainCamera.transform.position = alignCenter;
                
            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GridHeight; y++)
                {
                    GameObject newCellGameObject = Instantiate(GridCellPrefab, this.gameObject.transform);
                    
                    Vector3 position = new Vector3(x, y, 0);
                    newCellGameObject.transform.position = position;
                    newCellGameObject.name = $"GridCell:({x},{y})";
                    
                    m_WaveFuncionCollapse.InitCell(newCellGameObject, new Vector2Int((int)position.x, (int)position.y));
                }
            }
        }
    }
}

