using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ApowoGames.Resources.External;
using UnityEngine;

namespace ApowoGames.Resources
{
    public static class RemoteResourceManager
    {
        private static Dictionary<string, Loader> _loaders = new Dictionary<string, Loader>();

        #region Load

        // load resources
        // if the resource is not found, it will return null
        // example:
        // SpriteSheetResource ss = await RemoteResourceManager.Load<SpriteSheetResource>(SpriteSheetResource.BuildRequest(uri));
        public static async Task<T> Load<T>(Request request) where T : Resource
        {
            try
            {
                var responses = await LoadFiles(request);
                return request.GenerateResource(responses) as T;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }

        public static void Initialize(string cacheRoot) {
            Loader.CacheRoot = cacheRoot;
        }

        private static async Task<ResponseEntity[]> LoadFiles(Request request)
        {
            var tasks = new List<Task<ResponseEntity>>();
            for (int i = 0; i < request.UriAndMimeTypes.Length; i++)
            {
                var uriAndMimeType = request.UriAndMimeTypes[i];
                tasks.Add(PerLoadFile(uriAndMimeType.Uri, uriAndMimeType.MimeType, request.CachePolicy,
                    request.UnloadPolicy));
            }

            return await Task.WhenAll(tasks);
        }

        private static async Task<ResponseEntity> PerLoadFile(string uri, MimeType mimeType, CachePolicy cachePolicy,
            UnloadPolicy unloadPolicy)
        {
            if (!_loaders.ContainsKey(uri))
            {
                var loader = new Loader(uri, mimeType, cachePolicy, unloadPolicy);
                _loaders[uri] = loader;
            }

            await _loaders[uri].Load();
            return _loaders[uri].ResponseEntity;
        }

        #endregion

        #region Unload

        public static void UnloadManually(Resource resource)
        {
            foreach (var responseFile in resource.Responses)
            {
                if (_loaders.ContainsKey(responseFile.Uri))
                {
                    _loaders[responseFile.Uri].Unload();
                    _loaders.Remove(responseFile.Uri);
                }
            }
        }

        #endregion

        #region ClearCache

        public static void ClearCache()
        {
            Debug.Log("RRM ClearCache");
            if (Directory.Exists(Loader.CacheRoot))
            {
                Directory.Delete(Loader.CacheRoot, true);
            }
        }

        #endregion
    }

    public abstract class Resource
    {
        public ResponseEntity[] Responses;

        protected Resource(ResponseEntity[] responseFiles)
        {
            Responses = responseFiles;
        }
    }

    public abstract class Request
    {
        public UriAndMimeType[] UriAndMimeTypes { get; set; }
        public CachePolicy CachePolicy { get; set; }
        public UnloadPolicy UnloadPolicy { get; set; }

        public abstract Resource GenerateResource(ResponseEntity[] responses);
    }

    public class UriAndMimeType
    {
        public string Uri { get; set; }
        public MimeType MimeType { get; set; }
    }

    public abstract class ResponseEntity
    {
        public object Data { get; } = null;
        public string Uri { get; private set; }
        public abstract MimeType MimeType { get; }
        public string Suffix { get; private set; }
        public abstract void Dispose();
    }

    public class MimeType : StringEnum
    {
        private MimeType(string value) : base(value)
        {
        }

        public static readonly MimeType AssetBundle = new MimeType("custom/assetbundle");
        public static readonly MimeType Image = new MimeType("image/png");
        public static readonly MimeType Json = new MimeType("application/json");
        public static readonly MimeType ArrayBuffer = new MimeType("application/arraybuffer");
    }

    public enum CachePolicy
    {
        永远 = 0,
    }

    public enum UnloadPolicy
    {
        从不 = 0,
        加载后 = 1,
        换场景后 = 2,
    }
}