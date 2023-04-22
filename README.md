# プレイ動画サンプル
[![動画リンクはこちら](https://github.com/tomoi/HoloDive-Hololens-Repository-Public/blob/main/Image/Wiki/COMGameSHOW/COMGameShow1.0.1%20PlayMovieSampleThumbnail.png?raw=false)](https://youtu.be/smT1GH2vxLM)

# 環境構築 
## - Vuforia
Vuforiaのアカウントを作成し、以下のリンクよりパッケージをダウンロードし、インポートしてください。<br>
https://developer.vuforia.com/downloads/sdk

## - MRTK
以下のリンクの手順より`Mixed Reality WinRT Projections`を追加してください。<br>
https://learn.microsoft.com/ja-jp/windows/mixed-reality/develop/unity/welcome-to-mr-feature-tool

![Feature Mixed Reality WinRT](/Image/FeatureMixedRealityWinRTImage.png)

## - DOTween (HOTween v2)
[DOTween (HOTween v2)](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676?aid=1100lNUQ&pubref=unity-install-dotween&utm_campaign=unity_affiliate&utm_medium=affiliate&utm_source=partnerize-linkmaker
) を Unity Assetstore よりImportしてください。

## - Platform
Build SettingよりUWPに変更してください。

## - 注意
- 初回はVuforiaアセットがインポートされていないためエラーが出ます。IgnoreやSafeModeでプロジェクトを立ち上げアセットをインポートしてください。
- Vuforiaをインポートする際、読み込まないことがあります。その場合はUnityを起動し直してください。
- DefaultObserverEventHandlerが見つからない(Vuforiaを正常にインポートできない)場合以下のファイルを編集し追記してください。

```
Packages/manifest.json

  "dependencies": {
    "com.ptc.vuforia.engine": {
      "version": "file:com.ptc.vuforia.engine-10.13.3.tgz", //任意のバージョン
      "depth": 0,
      "source": "local-tarball",
      "dependencies": {
        "com.unity.ugui": "1.0.0"
      }
    }
  }
```


```
Packages/packages-lock.json

"dependencies": {
    "com.ptc.vuforia.engine": "file:com.ptc.vuforia.engine-10.13.3.tgz" //任意のバージョン
    }
```

- 稀にMRTKのPrefabが見つからないことがありますが、気にせずにClearしてください。
- 開発で使用しているSceneは`Assets/Scenes/MainGame.unity`です。

# 遊び方
[Wiki](https://github.com/tomoi/HoloDive-Hololens-Repository-Public/wiki)からCOMGameSHOWビルドバージョンなど対象のバージョンのページを開き画像認識用画像を上から順番に読み込ませることでゲームを進めることができます。
## - 注意
このゲームはHololensにビルドして遊ぶことを想定しています。
Hololensがない場合はUnityEditor上で実行しwebカメラで画像認識をすることも可能ですが、オブジェクトの位置が重なったり、エフェクトが画面内にうまく表示されないといった問題が出る可能性があります。
# 開発期間
2022/10/05 ~ 2023/2/11
# 開発環境
|  Tools  |  Version  |
| ---- | ---- |
|  Unity  |  2021.3.3f1  |
|  Rider  |  2022.2.4  |

|  PakageName  |  Version  |
| ---- | ---- |
|  Universal RP  |  12.1.6   |
|  UniRx  |  6.2.2  |
|  UniTask  |  2.3.2  |
|  DOTween (HOTween v2)  |  1.2.705  |
|  Mixed Reality Toolkit<br>- Foundation<br>- Extensions<br>- StandardAssets<br>- Tools  |  2.8.2  |
|  Mixed Reality Scene Understanding  | 0.6.0 |
|  Mixed Reality OpenXR Plugin  |  1.6.0  |
|  Feature Mixed Reality WinRT (dotnetwinrt)  | 0.5.2009 |
|  Vuforia  |  10.13.3  |
