using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Microsoft.MixedReality.SceneUnderstanding.Samples.Unity;
using UnityEngine;

public class Debug_TimeAxisShader : MonoBehaviour
{
    [SerializeField] private Material mat;

    [SerializeField, ColorUsage(false, true)]
    private Color featureColor;

    [SerializeField, ColorUsage(false, true)]
    private Color pastColor;

    [SerializeField] private bool featureBool = false;

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "PlayEffect"))
        {
            PlayEffect(SceneUnderstandingManager.Instance._suMeshRenderers).Forget();
        }
    }

    async UniTask PlayEffect(List<MeshRenderer> collection)
    {
        foreach (MeshRenderer mesh in collection)
        {
            mesh.material = new Material(mat);
            mesh.material.SetInt("_isHoloView", 1);
            //色の変更
            mesh.material.SetColor("_HoloColor", GetColor());
        }

        float _time = 0;
        while (_time < 1)
        {
            foreach (MeshRenderer mesh in collection)
            {
                mesh.material.SetFloat("_HoloViewTime", _time);
            }

            _time += Time.deltaTime;
            await UniTask.Yield();
        }

        foreach (MeshRenderer mesh in collection)
        {
            mesh.material.SetInt("_isHoloView", 0);
        }
    }

    Color GetColor()
    {
        return featureBool ? featureColor : pastColor;
    }
}