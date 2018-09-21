# How to pack the assemblies togther

You can use [ILMerge](https://www.microsoft.com/en-us/download/details.aspx?id=17630) to create one single executable file.

```
ILMerge.exe /targetplatform:"v4,C:\Windows\Microsoft.NET\Framework\v4.0.30319" msgpackinspector.exe MsgPack.dll Be.Windows.Forms.HexBox.dll /out:MsgPackInspectorPacked.exe
```
