using System.Collections.Generic;
using UnityEngine;

public class HologramViewerManager : SingletonMonoBehaviour<HologramViewerManager>
{
    /*
    // イメージのホログラムの座標のList
    private List<HologramBaseObject> hologramList = new List<HologramBaseObject>();
    // オブジェクトの Transform
    private Transform playerViewTs = null;

    public HologramBaseObject nearObject { get; private set; }

    void Start()
    {
        playerViewTs = Camera.main.transform;
    }

    private void Update()
    {
        HideObject();       
    }

    /// <summary>
    /// オブジェクトが画像認識されたら、
    /// そのオブジェクトとプレイヤーとの距離を計る
    /// </summary>
    private HologramBaseObject MeasureTheDistance()
    {
        HologramBaseObject obj = null;
        float f = 0.0f;

        foreach (var v in hologramList)
        {
            if (v.isViewing == false) { continue; }
            float distance = Mathf.Abs(Vector3.Distance((Vector3)v.transform.position, playerViewTs.position));
            if (f == 0.0f) { f = distance; }
            else if (distance < f)
            {
                f = distance;
                Debug.Log(f);
                obj = v;
            }
        }

        return obj;
    }

    /// <summary>
    /// 一番近いオブジェクト以外を非表示にする
    /// </summary>
    private void HideObject()
    {
        HologramBaseObject obj = MeasureTheDistance();
        if (obj == null) { return; }
        foreach (var v in hologramList)
        {
            Debug.Log($"v != obj {v != obj} : {v.gameObject.name}");
            if (v != obj) { v.HideObject(); }
            else {
                Debug.Log(v.gameObject.name);
                v.ShowObject(); }
        }
    }

    public void SetUpObject(HologramBaseObject obj)
    {
        hologramList.Add(obj);
    }
    */

}
