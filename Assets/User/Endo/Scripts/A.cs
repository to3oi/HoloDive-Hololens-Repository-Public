using System;
using Cysharp.Threading.Tasks;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using UnityEngine;

namespace User.Endo.Scripts
{
    public class A : MonoBehaviour
    {
        private async void Start()
        {
            await UniTask.Delay(3000);

            // Get the first Mesh Observer available, generally we have only one registered
            var observer = CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();

            Debug.Log($"Mesh count from observer: {observer.Meshes.Count}");
            observer.Meshes[0].Renderer.material = null;
// Loop through all known Meshes
            // foreach (SpatialAwarenessMeshObject meshObject in observer.Meshes.Values)
            // {
            //     Mesh mesh = meshObject.Filter.mesh;
            //     // Do something with the Mesh object
            // }
        }
    }
}
