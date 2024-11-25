using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private KeyCode m_ManualGenerate;
    
    public bool ManualGenerate { get; private set; }

    private void Update()
    {
        CheckManualGenerate();
    }

    private void CheckManualGenerate()
    {
        if (Input.GetKeyDown(m_ManualGenerate))
        {
            ManualGenerate = true;
            return;
        }
        ManualGenerate = false;
    }
}
