using UnityEngine;

namespace ShadersMagic.Boids
{
    public class BoidsController : MonoBehaviour
    {
        [SerializeField] private Spawner spawner;
        [SerializeField] private BoidConfigSO config;
        [SerializeField] private ComputeShader computeShader;

        private Boid[] _boids;
        private ComputeBuffer _boidBuffer;
        
        private const int THREAD_COUNT = 1024;
        
        private void Start()
        {
            _boids = spawner.Boids;

            foreach (var boid in _boids)
            {
                boid.Initialize(config);
            }
            
            _boidBuffer = new ComputeBuffer (_boids.Length, BoidData.GetSize());
            
            computeShader.SetFloat("ViewRadius", config.PerceptionRadius);
            computeShader.SetFloat("AvoidRadius", config.AvoidanceRadius);
        }

        private void Update()
        {
            if (_boids == null || _boids.Length == 0) return;
            
            int numBoids = _boids.Length;
            var boidsData = new BoidData[numBoids];

            for (int i = 0; i < numBoids; i++)
            {
                boidsData[i].Position = _boids[i].Position;
                boidsData[i].Direction = _boids[i].Forward;
            }
            
            _boidBuffer.SetData(boidsData);
            
            computeShader.SetBuffer(0, "Boids", _boidBuffer);
            computeShader.SetInt("NumBoids", numBoids);
            
            int threadGroups = Mathf.CeilToInt(numBoids / (float)THREAD_COUNT);
            computeShader.Dispatch(0, threadGroups, 1, 1);
            
            _boidBuffer.GetData(boidsData);

            for (int i = 0; i < numBoids; i++)
            {
                _boids[i].UpdateBoid(boidsData[i]);
            }
        }
        

        private void OnDestroy()
        {
            _boidBuffer?.Dispose();
        }
    }
}