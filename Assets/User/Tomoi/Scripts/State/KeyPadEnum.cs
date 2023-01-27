using System;

/// <summary>
/// マスターPCの入力に使用するKeyPadで使用する特殊な数字の一覧
/// </summary>
[Flags]
public enum KeyPadEnum
{
    Zero = 0,
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    X = 110,//入力をクリア
    V = 100//入力を確定
}