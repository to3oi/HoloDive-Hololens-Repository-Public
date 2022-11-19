using System;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using UniRx;
using UnityEngine;

public class SpatialMeshManager : SingletonMonoBehaviour<SpatialMeshManager>
{
    public GameObject SpatialMeshRoot { get; private set; }
    public MeshRenderer SpatialMeshRenderer { get; private set; }
    public Material SpatialMeshMaterial { get;private set; }
    private MeshFilter spatialMeshFilter;

    [SerializeField,Header("空間メッシュに指定するマテリアル")] private Material _material;
    [SerializeField,Header("デフォルトのマテリアルカラー"), ColorUsage(false, true)] private Color _defoultMaterialColor;

    private void ShowSpatialMesh()
    {
        //空間メッシュの取得
        var observer = CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();

        //複数のメッシュを組み合わせる
        CombineInstance[] combine = new CombineInstance[observer.Meshes.Count];

        var i = 0;
        foreach (SpatialAwarenessMeshObject meshObject in observer.Meshes.Values)
        {
            combine[i].mesh = meshObject.Filter.sharedMesh;
            combine[i].transform = meshObject.Filter.transform.localToWorldMatrix;

            i++;
        }
        
        //MeshFilterにメッシュを設定
        spatialMeshFilter.mesh.CombineMeshes(combine);
    }

    void Start()
    {
        SpatialMeshRoot = new GameObject("SpatialMeshRoot");
        spatialMeshFilter = SpatialMeshRoot.AddComponent<MeshFilter>();
        SpatialMeshRenderer = SpatialMeshRoot.AddComponent<MeshRenderer>();
        SpatialMeshRenderer.material = new Material(_material);
        SpatialMeshMaterial = SpatialMeshRenderer.material;
        SpatialMeshMaterial.SetInt("_isHoloView", 1);
        SpatialMeshMaterial.SetColor("_HoloColor",_defoultMaterialColor);
        SpatialMeshMaterial.SetFloat("_HoloViewTime", 0);

        //アプリ起動時から3秒ごとに「ShowSpatialMesh」を実行
        Observable.Interval(TimeSpan.FromSeconds(3))
            .Subscribe(_ => { ShowSpatialMesh(); })
            .AddTo(this);
    }
}