# How to pack the assemblies togther

```
ILMerge.exe /targetplatform:"v4,C:\Windows\Microsoft.NET\Framework\v4.0.30319" msgpackinspector.exe MsgPack.dll Be.Windows.Forms.HexBox.dll /out:MsgPackInspectorPacked.exe
```
