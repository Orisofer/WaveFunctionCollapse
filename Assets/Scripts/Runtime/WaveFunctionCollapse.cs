using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFC
{
    public class WaveFunctionCollapse : IWaveFunctionCollapseService
    {
        private List<GridCell> m_Cells;

        public bool Finished { get; }

        public WaveFunctionCollapse(List<GridCell> cells)
        {
            m_Cells = cells;
        }

        public void AddCell(GridCell cell)
        {
            m_Cells.Add(cell);
        }
        
        public void Generate()
        {
            Debug.Log("WFC: Started");
        }
    }
}

