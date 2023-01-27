using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

/// <summary>
/// 1ページあたりの画像とページの移動に関する情報を持った構造体
/// </summary>
[Serializable]
public class ViewData
{
    /// <summary>
    /// 表示するTextureデータ
    /// </summary>
    public Texture Texture;

    /// <summary>
    /// 次のViewDataへ切り替えるのをボタンの代わりに任意のステートがtrueになったときに変更する
    /// </summary>
    public bool isCheckState = false;

    /// <summary>
    /// チェックするステート
    /// </summary>
    public GameState CheckState = GameState.StartHoloDive;

    /// <summary>
    /// 進むボタンを表示するかどうか
    /// </summary>
    public bool nextButtonActive = true;

    /// <summary>
    /// 戻るボタンを表示するかどうか
    /// </summary>
    public bool backButtonActive = true;
}

/// <summary>
/// チュートリアル用の画像を表示するオブジェクトの画像切り替えをするスクリプト
/// </summary>
public class TutorialCanvasChanger : MonoBehaviour
{
    /// <summary>
    /// キャンバスに表示するデータの配列
    /// </summary>
    [SerializeField] private List<ViewData> canvasViewData = new List<ViewData>();

    /// <summary>
    /// 表示するエリア
    /// </summary>
    [SerializeField] private RawImage outputArea;

    /// <summary>
    /// 現在表示している配列の番号
    /// </summary>
    private int index;

    /// <summary>
    /// 表示できる配列の最大値
    /// </summary>
    private int maxIndex;

    /// <summary>
    /// 次へ表示を送るボタンのオブジェクト
    /// </summary>
    [SerializeField] private GameObject nextButton;

    /// <summary>
    /// 前へ表示を送るボタンのオブジェクト
    /// </summary>
    [SerializeField] private GameObject backButton;

    void Start()
    {
        index = -1;
        outputArea.texture = canvasViewData[0].Texture;
        maxIndex = canvasViewData.Count;

        nextPage().Forget();
        nextButton.SetActive(canvasViewData[index].nextButtonActive);
        //最初のページのときのみ確実に前へ戻るボタンを非表示にする
        backButton.SetActive(false);
    }

    /// <summary>
    /// UnityEventから指定できるNextPage関数
    /// </summary>
    [ContextMenu("NextPage")]
    public void NextPage()
    {
        nextPage().Forget();
    }

    /// <summary>
    /// UnityEventから指定できるBackPage関数
    /// </summary>
    [ContextMenu("BackPage")]
    public void BackPage()
    {
        backPage().Forget();
    }

    /// <summary>
    /// indexを+1し、maxIndexを超えなければcanvasViewData[index]の要素に切り替える
    /// </summary>
    private async UniTask nextPage()
    {
        index++;
        //インデックスが範囲外ならインデックスを戻して終了
        if (maxIndex <= index)
        {
            index--;
            return;
        }

        //表示しているテクスチャの差し替え
        outputArea.texture = canvasViewData[index].Texture;

        //最初のページのときのみ確実に前へ戻るボタンを非表示にする
        backButton.SetActive(index != 0 && canvasViewData[index].backButtonActive);
        //最後のページのときのみ確実に次へ進むボタンを非表示にする
        nextButton.SetActive(index != maxIndex - 1 && canvasViewData[index].nextButtonActive);

        //ステートの確認が必要なら確認する
        if (canvasViewData[index].isCheckState)
        {
            //ボタンを非表示にする
            backButton.SetActive(false);
            nextButton.SetActive(false);


            //任意のステートがtrueになるのを待つ
            await UniTask.WaitUntil(() => GameManager.Instance.GetState(canvasViewData[index].CheckState));


            //ステートの確認を解除
            canvasViewData[index].isCheckState = false;
            canvasViewData[index].nextButtonActive = true;
            canvasViewData[index].backButtonActive = true;

            //次のページに推移
            nextPage().Forget();
        }
    }

    /// <summary>
    /// indexを-1し、0を超えなければcanvasViewData[index]の要素に切り替える
    /// </summary>
    private async UniTask backPage()
    {
        index--;
        //インデックスが範囲外ならインデックスを戻して処理を終了
        if (index < 0)
        {
            index = 0;
            return;
        }

        //表示しているテクスチャの差し替え
        outputArea.texture = canvasViewData[index].Texture;

        //最初のページのときのみ確実に前へ戻るボタンを非表示にする
        backButton.SetActive(index != 0 && canvasViewData[index].backButtonActive);
        //最後のページのときのみ確実に次へ進むボタンを非表示にする
        nextButton.SetActive(index != maxIndex - 1 && canvasViewData[index].nextButtonActive);


        //ステートの確認が必要なら確認する
        if (canvasViewData[index].isCheckState)
        {
            //ボタンを非表示にする
            backButton.SetActive(false);
            nextButton.SetActive(false);


            //任意のステートがtrueになるのを待つ
            await UniTask.WaitUntil(() => GameManager.Instance.GetState(canvasViewData[index].CheckState));


            //ステートの確認を解除
            canvasViewData[index].isCheckState = false;
            canvasViewData[index].nextButtonActive = true;
            canvasViewData[index].backButtonActive = true;

            //前のページに推移
            backPage().Forget();
        }
    }
}