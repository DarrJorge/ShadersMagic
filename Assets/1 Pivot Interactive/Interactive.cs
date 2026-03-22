using UnityEngine;

namespace ShadersMagic.PivotInteractive
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Interactive : MonoBehaviour
    {
        private static readonly int MousePositionProp = Shader.PropertyToID("_MousePosition");
        
        private Material _material;
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Start()
        {
            _material = GetComponent<MeshRenderer>().sharedMaterial;
            Debug.Assert(_material != null, "Material is null");
        }

        private void Update()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            var plane = new Plane(transform.up, transform.position);
            if (!plane.Raycast(ray, out float enter)) return;
            
            _material.SetVector(MousePositionProp, ray.GetPoint(enter));
        }
    }
}
