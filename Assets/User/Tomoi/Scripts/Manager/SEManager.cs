using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// SEの種類とそのクリップを保持する構造体
/// </summary>
[Serializable]
public class SEData
{
    public SEType SEType;
    public AudioClip Clip;
}

/// <summary>
/// SEManager.Instance.PlaySE()の引数でSEの種類と座標を指定してあげると一度だけSEを再生し、オブジェクトプールに戻す一連の処理を発火するクラス
/// </summary>
public class SEManager : SingletonMonoBehaviour<SEManager>
{
    /// <summary>
    /// すべてのSEの種類とAudioClipを持つList
    /// </summary>
    [SerializeField] private List<SEData> SeDatas = new List<SEData>();
    
    /// <summary>
    /// SEObjectのオブジェクトプール
    /// </summary>
    private ObjectPool<SEObject> SEObjectPool;
    
    /// <summary>
    /// オブジェクトプールで生成したオブジェクトのルートオブジェクト
    /// </summary>
    private GameObject ParentSEObject;
    protected override void Awake()
    {
        ParentSEObject = new GameObject("ParentSEObject");
        //オブジェクトプールを初期化
        SEObjectPool = new ObjectPool<SEObject>(
            OnCreatePooledSEObject,
            OnGetFromPoolSEObject,
            OnReleaseToPoolSEObject,
            OnDestroyPooledSEObject
        );
    }

    #region オブジェクトプールで使用する関数
    //生成
    SEObject OnCreatePooledSEObject()
    {
        var g = new GameObject("SEObject").AddComponent<SEObject>();
        //ルートオブジェクトを変更する
        g.transform.parent = ParentSEObject.transform;
        return g;
    }
    //取得
    void OnGetFromPoolSEObject(SEObject seObject)
    {
        seObject.gameObject.SetActive(true);
    }
    //オブジェクトプールに戻す
    void OnReleaseToPoolSEObject(SEObject seObject)
    {
        seObject.gameObject.SetActive(false);
    }
    //削除
    void OnDestroyPooledSEObject(SEObject seObject)
    {
        Destroy(seObject.gameObject);
    }
    #endregion

    /// <summary>
    /// 引数で指定したSETypeのAudioClipをVector3の地点で一度だけ再生する
    /// </summary>
    /// <param name="seType"></param>
    /// <param name="position"></param>
    public void PlaySE(SEType seType,Vector3 position)
    {
        //SEObjectをオブジェクトプールから取得
        SEObject seObject = SEObjectPool.Get();
        //再生
        seObject.PlaySE(GetSE(seType),position);
    }

    /// <summary>
    /// SEObjectをオブジェクトプールに戻す関数
    /// </summary>
    /// <param name="seObject"></param>
    public void ReleaseSEObject(SEObject seObject)
    {
        SEObjectPool.Release(seObject);
    }

    /// <summary>
    /// SETypeからAudioClipを取得する関数
    /// </summary>
    /// <param name="seType"></param>
    /// <returns></returns>
    private AudioClip GetSE(SEType seType)
    {
        foreach (var seData in SeDatas)
        {
            if (seData.SEType == seType)
            {
                return seData.Clip;
            }
        }
        return null;
    }
}