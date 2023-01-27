using System;

/// <summary>
/// ゲームの進行管理用のステート
/// </summary>
[Flags]
public enum GameState
{
    StartHoloDive = 0, //最初のステート
    StartDevice = 1, //スタート用の画像を認識したときに使用する
    PressBombSwitch = 2, //爆弾のスイッチを押したときに表示される画像を認識したときに使用する
    LookManual = 4, //研究所のマニュアルを見たときに使用する
    LookSurveillanceMonitor = 8,//監視モニターを見たときに使用する
    LookDiary = 16,//日誌を見たときに使用する
    LookMasterPC = 32, //ロード中のマスターPCを見たときに使用する
    TrueEnd = 64, //爆弾を解除したときの画像を認識したときに使用する
    BadEnd = 128,//爆弾の解除に失敗した時の画像を認識したときに使用する
    TimeShifted = 256//手の甲を2回叩くことで時間軸の移動をしたことがあるか
}