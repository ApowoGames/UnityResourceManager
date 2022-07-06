using System.IO;
using System.Threading.Tasks;
using ApowoGames.Resources.External;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace ApowoGames.Resources
{
    public class Loader
    {
        public ResponseEntity ResponseEntity;

        public string Uri;
        public MimeType MimeType;
        public CachePolicy CachePolicy;
        public UnloadPolicy UnloadPolicy;

        private string _cacheFileName = string.Empty;

        public string CacheFileName
        {
            get
            {
                if (string.IsNullOrEmpty(Uri)) return string.Empty;

                if (string.IsNullOrEmpty(_cacheFileName))
                {
                    _cacheFileName = MD5Helper.EncryptString(Uri);
                }

                return _cacheFileName;
            }
        }

        private bool _started = false;

        public Loader(string uri, MimeType mimeType, CachePolicy cachePolicy, UnloadPolicy unloadPolicy)
        {
            Uri = uri;
            MimeType = mimeType;
            CachePolicy = cachePolicy;
            UnloadPolicy = unloadPolicy;
        }

        public async Task Load()
        {
            if (ResponseEntity != null)
            {
                return;
            }

            if (_started)
            {
                while (ResponseEntity == null)
                {
                    await Task.Yield();
                }
                return;
            }

            _started = true;

            if (CheckInCache())
            {
                await LoadFromCache();
            }
            else
            {
                await LoadFromNetwork();
            }

            _started = false;
        }

        private bool CheckInCache()
        {
            string cachePath = Path.Combine(CacheRoot, CacheFileName);
            // Debug.Log("RRM CheckInCache: " + cachePath);
            if (File.Exists(cachePath))
            {
                return true;
            }

#if UNITY_ANDROID
            using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    //Debugger.Log(()=>"CacheFileName---->>" + CacheFileName,5);
                    byte[] re = null;
                    try
                    {
                        re = jo.Call<byte[]>("GetFileBytes", CacheFileName);
                   
                        if (re.Length==1)
                        {
                            Debugger.Log(() => ">>>>>>> ###" + CacheFileName + "读取 失败 length is 1. function undefined? ",5);
                            return false;
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debugger.Log(() => ">>>>>>> ###" + CacheFileName + "读取 失败 " + e.Message,5);
                        return false;
                    }
                    //Debugger.Log(() => "re.Length---->>" + re.Length,5);
                    return true;
                }
            }
#endif
            return false;
        }

        private async Task LoadFromCache()
        {
            string cachePath = Path.Combine(CacheRoot, CacheFileName);
            
            Debug.Log("RRM LoadFromCache: " + cachePath);
            
            byte[] bytes = await File.ReadAllBytesAsync(cachePath);
            if (MimeType == MimeType.Image)
            {
                ResponseEntity = new ImageResponseEntity(bytes);
            }
            else if (MimeType == MimeType.Json)
            {
                ResponseEntity = new JsonResponseEntity(bytes);
            }
            else if (MimeType == MimeType.ArrayBuffer)
            {
                ResponseEntity = new ArrayBufferResponseEntity(bytes);
            }
            else if (MimeType == MimeType.AssetBundle)
            {
                ResponseEntity = new AssetBundleResponseEntity(bytes);
            }
            else
            {
                Debug.LogError("MimeType error, check url: " + (Uri));
            }
        }

        // TODO: 封装
        private async Task LoadFromNetwork()
        {
            Debug.Log("RRM LoadFromNetwork: " + Uri);
            
            byte[] data = new byte[0];
            if (MimeType == MimeType.Image)
            {
                UnityWebRequest request = UnityWebRequestTexture.GetTexture(Uri);
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("404 Load Texture error, check url: " + (Uri));
                    return;
                }

                data = request.downloadHandler.data;
                Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
                ResponseEntity = new ImageResponseEntity(tex);
            }
            else if (MimeType == MimeType.Json)
            {
                UnityWebRequest request = UnityWebRequest.Get(Uri);
                request.SetRequestHeader("Content-Type", "application/json");
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("404 Load Json error, check url: " + (Uri));
                    return;
                }

                data = request.downloadHandler.data;
                var jsonStr = request.downloadHandler.text;
                ResponseEntity = new JsonResponseEntity(jsonStr);
            }
            else if (MimeType == MimeType.ArrayBuffer)
            {
                UnityWebRequest request = UnityWebRequest.Get(Uri);
                request.SetRequestHeader("Content-Type", "application/arraybuffer");
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("404 Load ArrayBuffer error, check url: " + Uri);
                    return;
                }

                data = request.downloadHandler.data;
                byte[] pi = request.downloadHandler.data;
                ResponseEntity = new ArrayBufferResponseEntity(pi);
            }
            else if (MimeType == MimeType.AssetBundle)
            {
                UnityWebRequest request = UnityWebRequest.Get(Uri);
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("404 Load AssetBundle error, check url: " + (Uri));
                    return;
                }

                data = request.downloadHandler.data;
                ResponseEntity = new AssetBundleResponseEntity(data);
            }
            else
            {
                Debug.LogError("MimeType error, check url: " + (Uri));
            }

            if (data.Length > 0 && CachePolicy == CachePolicy.永远)
            {
                // save to cache
                var cachePath = Path.Combine(CacheRoot, CacheFileName);
                FileStream cacheFs = new FileStream(cachePath, FileMode.Create, FileAccess.ReadWrite);
                cacheFs.Write(data, 0, data.Length);
                cacheFs.Close();
                cacheFs.Dispose();
            }
        }
        
        public void Unload()
        {
            if (ResponseEntity != null)
            {
                ResponseEntity.Dispose();
                ResponseEntity = null;
            }
        }

        #region RootSettings

        static string _cacheRoot;

        /// <summary>
        /// 可写更新目录
        /// </summary>
        private static string CacheRoot
        {
            get
            {
                if (_cacheRoot == null)
                {
                    _cacheRoot =
#if UNITY_EDITOR
                GetEditorAssetPath();
#elif UNITY_STANDALONE_WIN
                Application.dataPath + "/StreamingAssets/Cache";
#elif UNITY_STANDALONE_OSX
		        Application.dataPath + "/StreamingAssets/Cache";
#elif UNITY_ANDROID
	            Application.persistentDataPath;
#elif UNITY_IPHONE
	            Application.persistentDataPath;
#elif UNITY_WEBGL
	            Application.dataPath + "/StreamingAssets/Cache";
#else
                        string.Empty;
#endif
                    
                    if (!Directory.Exists(_cacheRoot))
                    {
                        Directory.CreateDirectory(_cacheRoot);
                    }
                }

                return _cacheRoot;
            }
        }

#if UNITY_EDITOR
        static string GetEditorAssetPath()
        {
            string re = Path.Combine(Application.dataPath, "../TempStreamingAssets/" + BuildTargetFolderName() + "/Cache");
            return re;
        }

        static string BuildTargetFolderName()
        {
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.iOS:
                    return "IOS";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
                case BuildTarget.StandaloneOSX:
                    return "MacOS";
                case BuildTarget.WebGL:
                    return "WebGL";
                default:
                    return "unkown";
            }
        }
#endif

        #endregion
    }
}