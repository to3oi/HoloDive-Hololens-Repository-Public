using System;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using UniRx;
using UnityEngine;

/// <summary>
/// 時間軸変更時に使用するメッシュを生成、更新、公開するマネージャー
/// </summary>
public class SpatialMeshManager : SingletonMonoBehaviour<SpatialMeshManager>
{
    /// <summary>
    /// すべての空間メッシュを保持するオブジェクト
    /// </summary>
    public GameObject SpatialMeshRoot { get; private set; }

    /// <summary>
    /// すべての空間メッシュのMeshRenderer
    /// </summary>
    public MeshRenderer SpatialMeshRenderer { get; private set; }

    /// <summary>
    /// SpatialMeshRendererに指定されているマテリアル
    /// </summary>
    public Material SpatialMeshMaterial { get; private set; }

    /// <summary>
    /// SpatialMeshRootのメッシュを合成するのに使用するMeshFilter
    /// </summary>
    private MeshFilter _spatialMeshFilter;

    [SerializeField, Header("空間メッシュに指定するマテリアル")]
    private Material _material;

    [SerializeField, Header("デフォルトのマテリアルカラー"), ColorUsage(false, true)]
    private Color _defoultMaterialColor;

    //マテリアルのプロパティID
    //波紋用のプロパティ
    private static readonly int IsHoloView = Shader.PropertyToID("_isHoloView");
    private static readonly int HoloViewColor = Shader.PropertyToID("_HoloViewColor");
    private static readonly int HoloViewTime = Shader.PropertyToID("_HoloViewTime");

    //壁用のプロパティ
    private static readonly int WallBaseColor = Shader.PropertyToID("_WallBaseColor");
    private static readonly int WallAfterColor = Shader.PropertyToID("_WallAfterColor");
    private static readonly int ColorChangeElapsedTime = Shader.PropertyToID("_ColorChangeElapsedTime");
    
    
    /// <summary>
    /// すべての空間メッシュを合成する
    /// </summary>
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
        _spatialMeshFilter.mesh.CombineMeshes(combine);
    }

    private void Start()
    {
        SpatialMeshRoot = new GameObject("SpatialMeshRoot");
        _spatialMeshFilter = SpatialMeshRoot.AddComponent<MeshFilter>();
        SpatialMeshRenderer = SpatialMeshRoot.AddComponent<MeshRenderer>();
        SpatialMeshRenderer.material = new Material(_material);
        SpatialMeshMaterial = SpatialMeshRenderer.material;
        //マテリアルの初期化
        //初期化使用時にも初期化するが影響のあるものは適当な値で初期化
        SpatialMeshMaterial.SetInt(IsHoloView, 1);
        SpatialMeshMaterial.SetColor(HoloViewColor, _defoultMaterialColor);
        SpatialMeshMaterial.SetFloat(HoloViewTime, 0);
        SpatialMeshMaterial.SetColor(WallBaseColor, _defoultMaterialColor);
        SpatialMeshMaterial.SetColor(WallAfterColor, _defoultMaterialColor);
        SpatialMeshMaterial.SetFloat(ColorChangeElapsedTime, 1000);


        //アプリ起動時から3秒ごとに「ShowSpatialMesh」を実行
        Observable.Interval(TimeSpan.FromSeconds(3))
            .Subscribe(_ => { ShowSpatialMesh(); })
            .AddTo(this);
    }
}