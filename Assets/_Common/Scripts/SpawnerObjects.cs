using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadersMagic
{
    public class SpawnerObjects : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int amount;
        
        private void Start()
        {
            for (int i = 0; i < amount; i++)
            {
                Instantiate(prefab, transform);
            }
        }
        
    }
}
