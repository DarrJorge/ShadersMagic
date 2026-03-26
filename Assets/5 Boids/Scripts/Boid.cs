using System;
using UnityEngine;

namespace ShadersMagic.Boids
{
    public struct BoidData
    {
        public Vector3 Position;
        public Vector3 Direction;
        public Vector3 FlockHeading;
        public Vector3 FlockCenter;
        public Vector3 AvoidanceHeading;
        public int NumberFlockmates;

        public static int GetSize()
        {
            return sizeof(float) * 3 * 5 + sizeof(int);
        }
    }
    
    public class Boid : MonoBehaviour
    {
        [SerializeField] private Material material;
        
        private BoidConfigSO _config;
        
        private Transform _cachedTransform;
        private Vector3 _position;
        private Vector3 _velocity;
        private Vector3 _forward;
        private Vector3 _averageFlockHeading;
        private Vector3 _averageAvoidanceHeading;
        private Vector3 _centerOfFlockmates;
        private int _numPerceivedFlockmates;
        
        public Vector3 Position => _position;
        public Vector3 Forward => _forward;

        private void Awake()
        {
            _cachedTransform = transform;
        }

        public void Initialize(BoidConfigSO config)
        {
            _config = config;
            _position = _cachedTransform.position;
            _forward = _cachedTransform.forward;
            
            var startSpeed = (config.MinSpeed + config.MaxSpeed) / 2;
            _velocity = transform.forward * startSpeed;
        }

        public void UpdateBoid(in BoidData data)
        {
            _averageFlockHeading = data.FlockHeading;
            _centerOfFlockmates = data.FlockCenter;
            _averageAvoidanceHeading = data.AvoidanceHeading;
            _numPerceivedFlockmates= data.NumberFlockmates;

            Vector3 acceleration = Vector3.zero;

            if (_numPerceivedFlockmates != 0)
            {
                _centerOfFlockmates /= _numPerceivedFlockmates;

                Vector3 offsetToFlockmatesCenter = _centerOfFlockmates - _position;
                
                var alignmentForce = SteerTowards(_averageFlockHeading) * _config.AlignWeight;
                var cohesionForce = SteerTowards(offsetToFlockmatesCenter) * _config.CohesionWeight;
                var separationForce = SteerTowards(_averageAvoidanceHeading) * _config.SeparationWeight;
                
                acceleration += alignmentForce;
                acceleration += cohesionForce;
                acceleration += separationForce;
            }

            if (IsHeadingCollision())
            {
                Vector3 avoidDir = ObstacleRays();
                Vector3 avoidForce = SteerTowards(avoidDir) * _config.AvoidCollisionWeight;
                acceleration += avoidForce;
            }
            
            _velocity += acceleration * Time.deltaTime;
            
            float speed = _velocity.magnitude;
            Vector3 dir = _velocity / speed;
            speed = Mathf.Clamp(speed, _config.MinSpeed, _config.MaxSpeed);
            _velocity =  dir * speed;
            
            _cachedTransform.position += _velocity * Time.deltaTime;
            _cachedTransform.forward = dir;
            _position = _cachedTransform.position;
            _forward = dir;
        }

        public void SetColor(Color color)
        {
            //material.color = color;
        }

        private Vector3 ObstacleRays()
        {
            Vector3[] directions = BoidHelper.directions;

            for (int i = 0; i < directions.Length; i++)
            {
                Vector3 dir = _cachedTransform.TransformDirection(directions[i]);
                Ray ray = new Ray(_position, dir);
                
                if (!Physics.SphereCast(ray, _config.BoundsRadius, _config.CollisionAvoidDst, _config.ObstacleMask))
                    return dir;
            }
            
            return _forward;
        }

        private bool IsHeadingCollision()
        {
            RaycastHit hit;
            return Physics.SphereCast(_position, _config.BoundsRadius, _forward, out hit, 
                _config.CollisionAvoidDst, _config.ObstacleMask);
        }

        private Vector3 SteerTowards(Vector3 target)
        {
            Vector3 v = target.normalized * _config.MaxSpeed - _velocity;
            return Vector3.ClampMagnitude(v, _config.MaxSteerForce);
        }
    }
    
    
    public static class BoidHelper {

        const int numViewDirections = 300;
        public static readonly Vector3[] directions;

        static BoidHelper () {
            directions = new Vector3[BoidHelper.numViewDirections];

            float goldenRatio = (1 + Mathf.Sqrt (5)) / 2;
            float angleIncrement = Mathf.PI * 2 * goldenRatio;

            for (int i = 0; i < numViewDirections; i++) {
                float t = (float) i / numViewDirections;
                float inclination = Mathf.Acos (1 - 2 * t);
                float azimuth = angleIncrement * i;

                float x = Mathf.Sin (inclination) * Mathf.Cos (azimuth);
                float y = Mathf.Sin (inclination) * Mathf.Sin (azimuth);
                float z = Mathf.Cos (inclination);
                directions[i] = new Vector3 (x, y, z);
            }
        }
    }
}
