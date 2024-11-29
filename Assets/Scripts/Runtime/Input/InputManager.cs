using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private KeyCode m_ManualGenerate;
    [SerializeField] private KeyCode m_Restart;

    private FrameInput m_CurrFrameInput;

    private void Update()
    {
        m_CurrFrameInput = new FrameInput();
        
        CheckManualGenerate();
        CheckRestart();
        
        EventManager.InputChanged?.Invoke(m_CurrFrameInput);
    }

    private void CheckRestart()
    {
        if (Input.GetKeyDown(m_Restart))
        {
            m_CurrFrameInput.Restart = true;
            return;
        }
        m_CurrFrameInput.Restart = false;
    }

    private void CheckManualGenerate()
    {
        if (Input.GetKeyDown(m_ManualGenerate))
        {
            m_CurrFrameInput.ManualGenerate = true;
            return;
        }
        m_CurrFrameInput.Restart = true;
    }
}
