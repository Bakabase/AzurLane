中文 | [English](./Docs/README.en.md)

[![Release](https://github.com/Bakabase/AzurLane/actions/workflows/publish.yml/badge.svg)](https://github.com/Bakabase/AzurLane/actions/workflows/publish.yml)

# AzurLane

碧蓝航线工具包/Tools for azurlane

+ [AzurLane.Extractor](#AzurLaneExtractor)
  + 导出立绘/Export paintings

## AzurLane.Extractor

### 使用说明

#### 准备工作
1. 下载最新版本可执行文件
   1. [可选]如果需要获取app内部更新的资源，请下载[Adb](https://developer.android.com/studio/releases/platform-tools#downloads)
2. 复制`/samples`下的脚本到本地
3. 根据你自己的环境修改脚本内的参数

#### 提取原始资源

| 源 | 参数名 |
| ----------- | ----------- |
| 安卓设备/模拟器 | `adb`<br />`ip`<br />`port`<br />`assets-path-in-device` |
| 本地目录 | `local-assets-path` |
| Apk文件 | `apk` |

+ 对应样例`/samples/merge.bat`
+ 可以从多个源提取资源并合并至`output`指定的目录
+ 可通过`asset-types`设置需要提取的资源类型，默认为`painting`和`paintingface`
+ 如果提取完成，你会看到类似以下内容的控制台输出
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

#### 转换原始资源为Texture和Mesh

1. 通过[Asset Studio](https://github.com/Perfare/AssetStudio)加载上一步产生的文件夹
2. 菜单栏`Filter Type`选择`Mesh`和`Texture2D`
3. 菜单栏`Export`选择`Filtered Assets`

#### 合成最终图片

+ 对应样例`/samples/extract.bat`
+ 如果合成完成，你会看到类似以下内容的控制台输出
```console
>"../bin/Debug/net5.0/AzurLane.Extractor.exe" extract --tex-and-mesh-path D:\BaiduNetdiskDownload\Test\Assets -o D:\BaiduNetdiskDownload\Test\Output

1602/1602, ETA: 0:0All resources are extracted.
```

### Todo

+ 部分图片如1_sub合并异常，待支持
+ Unix系统支持
+ 本地化支持

### Roadmap

| Version | Description |
| ----------- | ----------- |
| 1.0.0 | 初版 |

### Contribution

请在issue中提出宝贵建议<br/>
谢谢！
