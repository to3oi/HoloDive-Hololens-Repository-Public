# 環境構築 
## - Vuforia
Vuforiaのアカウントを作成し、以下のリンクよりパッケージをダウンロードし、インポートしてください。<br>
https://developer.vuforia.com/downloads/sdk

## - MRTK
以下のリンクの手順より`Mixed Reality WinRT Projections`を追加してください。<br>
https://learn.microsoft.com/ja-jp/windows/mixed-reality/develop/unity/welcome-to-mr-feature-tool

![Feature Mixed Reality WinRT](/Image/FeatureMixedRealityWinRTImage.png)

## - Platform
Build SettingよりUWPに変更してください。

## - 注意
- 初回はVuforiaアセットがインポートされていないためエラーが出ます。IgnoreやSafeModeでプロジェクトを立ち上げアセットをインポートしてください。
- Vuforiaをインポートする際、読み込まないことがあります。その場合はUnityを起動し直してください。
- 稀にMRTKのPrefabが見つからないことがありますが、気にせずにClearしてください。
- 開発で使用しているSceneは`Assets/Scenes/MainGame.unity`です。

# 開発期間
2022/10/05 ~ 
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
|  Mixed Reality Toolkit<br>- Foundation<br>- Extensions<br>- StandardAssets<br>- Tools  |  2.8.2  |
|  Mixed Reality Scene Understanding  | 0.6.0 |
|  Mixed Reality OpenXR Plugin  |  1.6.0  |
|  Feature Mixed Reality WinRT (dotnetwinrt)  | 0.5.2009 |
|  Vuforia  |  10.11.3  |
