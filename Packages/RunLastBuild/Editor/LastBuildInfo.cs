using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;

namespace udonba.RunLastBuild
{
    /// <summary>
    /// The ScriptableObject to save build-output and run it from MenuItem.
    /// </summary>
    public class LastBuildInfo : ScriptableObject, IPostprocessBuildWithReport
    {
        private const string AssetPath = "Packages/com.udonba.runlastbuild/Editor/LastBuildInfo.asset";

        [SerializeField]
        private string _lastOutputPath = string.Empty;
        public string LastOutputPath
        {
            get => _lastOutputPath;
            private set => _lastOutputPath = value;
        }

        #region interface implement

        int IOrderedCallback.callbackOrder => 0;

        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
        {
            SaveOutputPath(report.summary.outputPath);
            //Debug.Log(report.summary.outputPath);
        }

        #endregion

        #region static member

        [MenuItem("File/Run Last Build")]
        public static void RunLastBuild()
        {
            var asset = LastBuildInfo.LoadAsset();

            if (File.Exists(asset.LastOutputPath))
            {
                asset.OpenLastOutput();
            }
            else
            {
                Debug.Log("Last build not found");
            }
        }

        private static void SaveOutputPath(string outputPath)
        {
            var asset = LoadAsset();
            asset.LastOutputPath = outputPath;

            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
        }

        private static LastBuildInfo LoadAsset()
        {
            var asset = AssetDatabase.LoadAssetAtPath<LastBuildInfo>(AssetPath);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<LastBuildInfo>();
                var dir = Path.GetDirectoryName(AssetPath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                AssetDatabase.CreateAsset(asset, AssetPath);
            }

            return asset;
        }

        #endregion

        public void OpenLastOutput()
        {
            if (!File.Exists(_lastOutputPath))
                return;

            string extension = Path.GetExtension(_lastOutputPath).ToLower();

            switch (extension)
            {
                case ".exe":
                    Application.OpenURL(_lastOutputPath);
                    Debug.Log($"Run \"{_lastOutputPath}\"");
                    break;

                default:
                    Application.OpenURL(Path.GetDirectoryName(_lastOutputPath));
                    Debug.Log($"Open \"{Path.GetDirectoryName(_lastOutputPath)}\"");
                    break;
            }
        }
    }
}
