"../bin/Debug/net5.0/AzurLane.Extractor.exe" merge ^
--ip 127.0.0.1 ^
--port 7555 ^
--adb "D:\OneDrive\Program Files\platform-tools\adb.exe" ^
--apk D:\BaiduNetdiskDownload\BLHX.apk ^
--assets-path-in-device /storage/emulated/0/Android/data/com.bilibili.azurlane/files/AssetBundles ^
--local-assets-path D:\BaiduNetdiskDownload\Full-Paintings ^
--asset-types painting paintingface ^
-o D:\BaiduNetdiskDownload\Test\Raw
pause