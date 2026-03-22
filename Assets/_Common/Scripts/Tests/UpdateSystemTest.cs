using System;
using UnityEngine;

namespace ShadersMagic
{
    public class UpdateSystemTest : MonoBehaviour
    {
        private IDisposable _subscription;
        
        private void Start()
        {
            _subscription = Loops.Update.Start(OnUpdate);
        }

        private void OnUpdate()
        {
            Debug.Log($"OnUpdate");
        }
        
        void OnDestroy()
        {
            _subscription.Dispose();
        }
    }
}
