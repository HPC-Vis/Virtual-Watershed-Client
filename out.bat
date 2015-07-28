start "some test window" /WAIT "C:\Program Files\Unity\Editor\Unity.exe" -pc -batchmode -projectpath %cd%\WatershedTest -executeMethod UnityTest.Batch.RunUnitTests -resultFilePath=%cd%\out.xml 
