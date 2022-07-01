using System.Collections.Generic;
using System.Threading.Tasks;
using ApowoGames.Resources.External;
using UnityEngine;

namespace ApowoGames.Resources
{
    public static class ResourceManager
    {   
        private static Dictionary<string, Loader> _loaders = new Dictionary<string, Loader>();

        #region Load

        // example:
        // SpriteSheetResource ss = await ResourceManager.Load(SpriteSheetResource.BuildRequest(uri)) as SpriteSheetResource;
        public static async Task<Resource> Load(Request request)
        {
            var responses = await LoadFiles(request);
            return request.GenerateResource(responses);
        }
        
        private static async Task<ResponseFile[]> LoadFiles(Request request)
        {
            var tasks = new List<Task<ResponseFile>>();
            for (int i = 0; i < request.UriAndMimeTypes.Length; i++)
            {
                var uriAndMimeType = request.UriAndMimeTypes[i];
                tasks.Add(PerLoadFile(uriAndMimeType.Uri, uriAndMimeType.MimeType, request.CachePolicy, request.UnloadPolicy));
            }

            var responses = await Task.WhenAll(tasks);
            return responses;
        }

        private static async Task<ResponseFile> PerLoadFile(string uri, MimeType mimeType, CachePolicy cachePolicy, UnloadPolicy unloadPolicy)
        {
            if (!_loaders.ContainsKey(uri))
            {
                var loader = new Loader(uri, mimeType, cachePolicy, unloadPolicy);
                _loaders[uri] = loader;
            }

            await _loaders[uri].Load();
            return _loaders[uri].ResponseFile;
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
                }
            }
        }

        #endregion
    }

    public abstract class Resource
    {
        public ResponseFile[] Responses;
        
        protected Resource(ResponseFile[] responseFiles)
        {
            Responses = responseFiles;
        }
    }

    public abstract class Request
    {
        public UriAndMimeType[] UriAndMimeTypes { get; set; }
        public CachePolicy CachePolicy { get; set; }
        public UnloadPolicy UnloadPolicy { get; set; }
        
        public abstract Resource GenerateResource(ResponseFile[] responses);
    }
    
    public class UriAndMimeType
    {
        public string Uri { get; set; }
        public MimeType MimeType { get; set; }
    }

    public abstract class ResponseFile
    {
        public object Data { get; }
        public string Uri { get; private set; }
        public abstract MimeType MimeType { get; }
        public string Suffix { get; private set; }
        public abstract void Dispose();
    }
    
    public class MimeType : StringEnum
    { 
        private MimeType(string value) : base(value) {}

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