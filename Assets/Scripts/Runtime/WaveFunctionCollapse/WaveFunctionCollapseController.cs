using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WFC
{
    public class WaveFunctionCollapseController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private WaveFunctionCollapse m_WaveFuncionCollapse;
        
        [Space(10)]
        
        [Header("Grid Settings")]
        [SerializeField] private int m_GridWidth;
        [SerializeField] private int m_GridHeight;
        [SerializeField] private int m_NumberOfManualStepps = 1;
        [SerializeField] private int m_AutoGenerateDelayMS = 5;
        [SerializeField] private bool m_ManualCollapse = false;

        private CancellationTokenSource m_CTS;
        private Camera m_MainCamera;
        private FrameInput m_FrameInput;

        private void Awake()
        {
            m_MainCamera = Camera.main;

            m_CTS = new CancellationTokenSource();

            EventManager.InputChanged += OnFrameInput;
        }

        private async void Start()
        {
            m_WaveFuncionCollapse.Initialize(m_GridWidth, m_GridHeight, new TileSelectionWeightedRandomStrategy());
            
            Vector3 alignCenter = new Vector3(((float)m_GridWidth / 2f) - 0.5f, ((float)m_GridHeight / 2f) - 0.5f, -10f);
            m_MainCamera.transform.position = alignCenter;
            
            //todo: make camera to fit nicely for the screen in relation to grid dimensions
            //m_MainCamera.orthographicSize = Mathf.Sqrt(m_GridHeight);
            
            if (!m_ManualCollapse)
            {
                await StartWaveAsync();
            }
        }

        private async void Update()
        {
            if (m_ManualCollapse && m_FrameInput.ManualGenerate)
            {
                m_WaveFuncionCollapse.GenerateStep(m_NumberOfManualStepps);
            }

            if (m_FrameInput.Restart)
            {
                m_WaveFuncionCollapse.ClearData();
                
                if (!m_ManualCollapse)
                {
                    await StartWaveAsync();
                }
            }
        }

        private async UniTask StartWaveAsync()
        {
            await m_WaveFuncionCollapse.GenerateAuto(m_AutoGenerateDelayMS, () => Debug.Log("callback working"));
        }

        void OnFrameInput(FrameInput frameInput)
        {
            m_FrameInput = frameInput;
        }

        private void OnDestroy()
        {
            EventManager.InputChanged -= OnFrameInput;
        }
    }
}

