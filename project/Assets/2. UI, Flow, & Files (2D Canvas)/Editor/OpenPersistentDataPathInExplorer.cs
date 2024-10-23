using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR

public class OpenPersistentDataPath
{
    [MenuItem("Tools/Open Persistent Data Path")]
    private static void OpenPersistentDataPathInExplorer()
    {
        string path = Application.persistentDataPath;
        
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            EditorUtility.RevealInFinder(path);
        }
        else if (Application.platform == RuntimePlatform.OSXEditor)
        {
            EditorUtility.RevealInFinder(path);
        }
        else if (Application.platform == RuntimePlatform.LinuxEditor)
        {
            // Linux implementation, since EditorUtility.RevealInFinder doesn't work on Linux
            string command = "xdg-open " + path;
            System.Diagnostics.Process.Start(command);
        }
        else
        {
            Debug.LogWarning("Platform not supported for opening persistent data path.");
        }
    }
    
    
    [MenuItem("Tools/Clear Reports Directory")]
    private static void ClearReportsDirectory()
    {
        string reportsPath = Path.Combine(Application.persistentDataPath, "Reports");

        if (Directory.Exists(reportsPath))
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(reportsPath);

                foreach (FileInfo file in directoryInfo.GetFiles())
                {
                    file.Delete();
                }

                foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
                {
                    dir.Delete(true);
                }

                Debug.Log("Reports directory cleared successfully.");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to clear Reports directory: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Reports directory does not exist.");
        }
    }
}
#endif
