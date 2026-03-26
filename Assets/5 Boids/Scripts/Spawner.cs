using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShadersMagic.Boids
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private Boid boidPrefab;
        [SerializeField] private float spawnRadius;
        [SerializeField] private int spawnCount;
        [SerializeField] private Color color;
        
        private List<Boid> _boids;
        
        public Boid[] Boids => _boids.ToArray();

        private void Awake()
        {
            _boids = new List<Boid>(spawnCount);
            
            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
                Boid boid = Instantiate(boidPrefab, transform);
                boid.transform.position = pos;
                boid.transform.forward = Random.insideUnitSphere;
                boid.SetColor(color);
                _boids.Add(boid);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(color.r, color.g, color.b, 0.3f);
            Gizmos.DrawSphere(transform.position, spawnRadius);
        }
    }
}