using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public float Timer { get; private set;}
    void Update()
    {
        //スタートデバイスを確認したらTimerを開始する
        if(GetState(GameState.StartDevice))
        {
            //使わないと思うけど一応
            Timer = Time.deltaTime;
        }
    }


    #region ステート周りの実装

    private GameState _state;

    /// <summary>
    /// 現在のステートを追加する
    /// </summary>
    /// <param name="state"></param>
    public void SetState(GameState state)
    {
        _state |= state;
    }

    /// <summary>
    /// 現在のステートを削除する
    /// </summary>
    /// <param name="state"></param>
   public  void RemoveState(GameState state)
    {
        _state &= ~state;
    }

    /// <summary>
    /// 現在のステートを確認する
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public bool GetState(GameState state)
    {
        return _state.HasFlag(state);
    }

    #endregion
}