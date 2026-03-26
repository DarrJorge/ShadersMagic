using UnityEngine;

namespace ShadersMagic.Boids
{
    [CreateAssetMenu(fileName = "BoidConfigSO", menuName = "SO/BoidConfig")]
    public class BoidConfigSO : ScriptableObject
    {
        public float MinSpeed = 3;
        public float MaxSpeed = 6;
        public float MaxSteerForce = 4;
        
        public float PerceptionRadius = 0.5f;
        public float AvoidanceRadius = 0.5f;
        
        public float AlignWeight = 1;
        public float CohesionWeight = 1;
        public float SeparationWeight = 1;
        public float TargetWeight = 1;
        
        [Header("Collision")]
        public LayerMask ObstacleMask;
        public float BoundsRadius = 0.27f;
        public float AvoidCollisionWeight = 10;
        public float CollisionAvoidDst = 5;
    }
}