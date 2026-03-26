using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSTool
{
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField] private int frameRange = 60;

        private int[] _fpsBuffer;
        private int _fpsBufferIndex;
        
        public int AverageFPS { get; private set; }
        public int MaximumFPS { get; private set; }
        public int MinimumFPS { get; private set; }

        private void Update()
        {
            if (_fpsBuffer == null || _fpsBuffer.Length != frameRange)
            {
                InitializeBuffer();
            }
            UpdateBuffer();
            CalculateFPS();
        }

        private void InitializeBuffer()
        {
            if (frameRange <= 0)
                frameRange = 1;
            
            _fpsBuffer = new int[frameRange];
            _fpsBufferIndex = 0;
        }

        private void UpdateBuffer()
        {
            _fpsBuffer[_fpsBufferIndex++] = (int)(1f / Time.unscaledDeltaTime);
            if (_fpsBufferIndex >= frameRange)
                _fpsBufferIndex = 0;    
        }

        private void CalculateFPS()
        {
            int sum = 0;
            int min = int.MaxValue;
            int max = 0;
            
            for (int i = 0; i < frameRange; i++)
            {
                int fps = _fpsBuffer[i];
                sum += fps;
                if (fps > max) max = fps;
                if (fps < min) min = fps;
            }

            MaximumFPS = max;
            AverageFPS = sum / frameRange;
            MinimumFPS = min;
        }
    }
}
