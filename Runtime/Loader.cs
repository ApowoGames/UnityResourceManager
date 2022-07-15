using System;
using System.IO;
using System.Text.RegularExpressions;
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

                    _cacheFileName += Path.GetExtension(Uri);
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

            try
            {
                if (CheckInCache())
                {
                    var path = Regex.Replace(Path.Combine(CacheRoot, CacheFileName), @"(http(s)?):\/\/", "");
                    //var path = Path.Combine(CacheRoot, CacheFileName);
                    await LoadFromNetwork("file://" + path);
                }
                else
                {
                    var bytes = await LoadFromNetwork(Uri);
                    SaveCache(bytes);
                }

                _started = false;
            }
            catch (Exception e)
            {
                _started = false;
                throw e;
            }
        }

        private bool CheckInCache()
        {
            return File.Exists(Path.Combine(CacheRoot, CacheFileName));
        }

        private void LoadFromCache(string url)
        {
            string cachePath = Path.Combine(CacheRoot, CacheFileName);

            //// Debug.Log("RRM LoadFromCache: " + cachePath);

            byte[] bytes = File.ReadAllBytes(cachePath);
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
                throw new Exception("MimeType error, check url: " + (Uri));
            }
        }

        // TODO: 封装
        private async Task<byte[]> LoadFromNetwork(string url)
        {
            byte[] data;
            if (MimeType == MimeType.Image)
            {
                UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
                request.SetRequestHeader("Content-Type", "image/png");
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception("404 Load Texture error, check url: " + (url));
                }

                data = request.downloadHandler.data;
                Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
                ResponseEntity = new ImageResponseEntity(tex);
            }
            else if (MimeType == MimeType.Json)
            {
                UnityWebRequest request = UnityWebRequest.Get(url);
                request.SetRequestHeader("Content-Type", "application/json");
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception("404 Load Json error, check url: " + (url));
                }

                data = request.downloadHandler.data;
                var jsonStr = request.downloadHandler.text;
                ResponseEntity = new JsonResponseEntity(jsonStr);
            }
            else if (MimeType == MimeType.ArrayBuffer)
            {
                UnityWebRequest request = UnityWebRequest.Get(url);
                request.SetRequestHeader("Content-Type", "application/octet-stream");
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    
                    throw new Exception("404 Load ArrayBuffer error, check url: " + (url) + "downloadProgress: " + request.downloadProgress + "downloadedBytes:" + request.downloadedBytes + "--->>" + request.result.ToString());
                }

                data = request.downloadHandler.data;
                byte[] pi = request.downloadHandler.data;
                ResponseEntity = new ArrayBufferResponseEntity(pi);
            }
            else if (MimeType == MimeType.AssetBundle)
            {
                UnityWebRequest request = UnityWebRequest.Get(url);
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception("404 Load AssetBundle error, check url: " + (url));
                }

                data = request.downloadHandler.data;
                ResponseEntity = new AssetBundleResponseEntity(data);
            }
            else
            {
                throw new Exception("MimeType error, check url: " + (url));
            }

            return data;

           
        }

        private void SaveCache(byte[] data)
        {
            if (data != null && data.Length > 0 && CachePolicy == CachePolicy.永远)
            {
                // save to cache
                var cachePath = Path.Combine(CacheRoot, CacheFileName);

                try
                {
                    FileStream cacheFs = new FileStream(cachePath, FileMode.Create, FileAccess.ReadWrite);
                    cacheFs.Write(data, 0, data.Length);
                    cacheFs.Close();
                    cacheFs.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine("SaveCache error: " + e);
                    throw;
                }
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
        internal static string CacheRoot
        {
            get
            {
                if (_cacheRoot == null)
                {
                    _cacheRoot = "Cache";
//#if UNITY_EDITOR
//                GetEditorAssetPath();
//#elif UNITY_STANDALONE_WIN
//                Application.dataPath + "/StreamingAssets/Cache";
//#elif UNITY_STANDALONE_OSX
//		        Application.dataPath + "/StreamingAssets/Cache";
//#elif UNITY_ANDROID
//	            Application.persistentDataPath;
//#elif UNITY_IPHONE
//	            Application.persistentDataPath;
//#elif UNITY_WEBGL
//	            Application.dataPath + "/StreamingAssets/Cache";
//#else
//                        string.Empty;
//#endif

                    if (!Directory.Exists(_cacheRoot))
                    {
                        Directory.CreateDirectory(_cacheRoot);
                    }
                }

                return _cacheRoot;
            }
            set
            {
                _cacheRoot = value;
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