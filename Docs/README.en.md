[中文](../../../) | English
# AzurLane

Tools for azurlane

+ [AzurLane.Extractor](#AzurLaneExtractor)
  + Export paintings

## AzurLane.Extractor

### Usage

#### Setup

1. Download latest binary file from [releases](../../../releases).
   1. [optional] Download [Adb](https://developer.android.com/studio/releases/platform-tools#downloads) binary file if you want to use assets downloaded at first launch of the game.
2. Copy scripts in /samples to local.
3. Adjust arguments in scripts according to your environment.

#### Merge raw assets from android device, local directory and apk file.

| source | arguments |
| ----------- | ----------- |
| Android/Emulator | `adb`<br />`ip`<br />`port`<br />`assets-path-in-device` |
| Local | `local-assets-path` |
| Apk | `apk` |
+ Sample: `/samples/merge.bat`
+ We can merge raw assets from multiple sources into `output` directory.
+ We can set `asset-types` to specify assets types we need, default value is `painting` and `paintingface`
+ Console will produce output like below if the job is successfully done.
```console
>"../bin/Debug/net5.0/AzurLane.Extractor.exe" merge --ip 127.0.0.1 --port 7555 --adb "D:\OneDrive\Program Files\platform-tools\adb.exe" --apk D:\BaiduNetdiskDownload\BLHX.apk --assets-path-in-device /storage/emulated/0/Android/data/com.bilibili.azurlane/files/AssetBundles --local-assets-path D:\BaiduNetdiskDownload\Full-Paintings --asset-types painting paintingface -o D:\BaiduNetdiskDownload\Test\Raw

Connecting device: 127.0.0.1:7555.
Connected.
Getting file list in device.
Got 3546 files to pull. Start pulling...
/storage/emulated/0/Android/data/com.bilibili.azurlane/fil...led, 0 skipped. -1.2 MB/s (1563766514 bytes in -1257.497s)
/storage/emulated/0/Android/data/com.bilibili.azurlane/fil...es pulled, 0 skipped. 2.6 MB/s (33864405 bytes in 12.253s)
Pull from device job is done.
Unzipping apk file...
Apk unzipped.
Moving files from D:\BaiduNetdiskDownload\Test\Raw\tmp to D:\BaiduNetdiskDownload\Test\Raw with asset types [painting,paintingface]
All files are moved
Delete tmp directory.
Moving files from D:\BaiduNetdiskDownload\Full-Paintings to D:\BaiduNetdiskDownload\Test\Raw with asset types [painting,paintingface]
All files are moved
```

#### Convert raw assets to Textures and Meshes.

1. Use [Asset Studio](https://github.com/Perfare/AssetStudio) to load folders produced in the last step
2. Select `Texture2D` and `Mesh` filters on `Menu`>`Filter Type`
3. Click `Filtered Assets` in `Menu`>`Export`

#### Generate 

+ Sample: `/samples/extract.bat`
+ Console will produce output like below if the job is successfully done.
```console
>"../bin/Debug/net5.0/AzurLane.Extractor.exe" extract --tex-and-mesh-path D:\BaiduNetdiskDownload\Test\Assets -o D:\BaiduNetdiskDownload\Test\Output

1602/1602, ETA: 0:0All resources are extracted.
```

### Todo
+ Combination for images like '1_sub' is not supported for now.
+ Support for unix systems.
+ Localization.

### Roadmap
| Version      | Description |
| ----------- | ----------- |
| 1.0.0     | Day0 version.        |

### Contribution

Feel free to submit issues or pull requests.<br />
Thanks.