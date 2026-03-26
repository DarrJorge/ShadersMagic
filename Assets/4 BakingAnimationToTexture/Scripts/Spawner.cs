using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _4_BakingAnimationToTexture.Scripts
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private float interval = 1f;
        [SerializeField] private float spawnRadius = 1f;
        [SerializeField] private GameObject prefab;
        
        private float _time;

        public int Count = 0;
        
        private List<Vector2> _positions = new List<Vector2>();
        
        private void Update()
        {
            _time += Time.deltaTime;
            if (_time > interval)
            {
                _time -= interval;
                SpawnNewObject();
            }
        }

        private void SpawnNewObject()
        {
            var newPoint = Random.insideUnitCircle * spawnRadius;
            var newObject = Instantiate(prefab, transform);
            newObject.transform.localPosition = new Vector3(newPoint.x, 0, newPoint.y);
            newObject.SetActive(true);
            _positions.Add(newPoint);
            Count = _positions.Count;
        }

        private void OnDestroy()
        {
            _positions.Clear();
        }
    }
}