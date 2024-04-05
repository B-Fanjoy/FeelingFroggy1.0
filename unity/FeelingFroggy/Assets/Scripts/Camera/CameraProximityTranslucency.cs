using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Camera
{
    public class CameraProximityTranslucency : MonoBehaviour
    {
        public PlayerCameraController cameraController;

        public float maxDist = 1f;
        public Renderer[] renderers;

        private Material[] _materials;

        private float _prevOpacity = 1f;

        private void Start()
        {
            // Make copy of materials so:
            // 1. We don't modify the original, which would save the asset file
            // 2. We can modify the color of the material without affecting other objects using the same material

            var materialCopies = new Dictionary<Material, Material>();

            foreach (var renderer in renderers)
            {
                // Copy renderer's materials
                var materials = renderer.materials.ToArray();

                // Substitute materials with copies
                for (var i = 0; i < materials.Length; i++)
                {
                    if (materialCopies.TryGetValue(materials[i], out var materialCopy))
                    {
                        materials[i] = materialCopy;
                    }
                    else
                    {
                        var newMaterial = new Material(materials[i]);
                        materialCopies[materials[i]] = newMaterial;
                        materials[i] = newMaterial;
                    }
                }

                // Set new materials on renderer
                renderer.materials = materials;
            }

            _materials = materialCopies.Values.ToArray();

            Debug.Log("Created copies of materials: " + _materials.Length);
        }

        private void Update()
        {
            var cameraDistSqr = (cameraController.camera.transform.position - transform.position).sqrMagnitude;
            var maxDistSqr = maxDist * maxDist;

            var opacity = cameraDistSqr >= maxDistSqr ? 1 : cameraDistSqr / maxDistSqr;
            opacity = Mathf.Lerp(_prevOpacity, opacity, Time.deltaTime * 10);

            /*if (Mathf.Approximately(_prevOpacity, opacity))
            {
                return;
            }*/

            foreach (var material in _materials)
            {
                var color = material.color;
                color.a = opacity;
                material.color = color;
            }

            _prevOpacity = opacity;
        }
    }
}
