using System.Collections.Generic;
using System.Threading.Tasks;
using ApowoGames.Resources.External;
using UnityEngine;

namespace ApowoGames.Resources
{
    public static class ResourceManager
    {
        private static Dictionary<string, Loader> _loaders = new Dictionary<string, Loader>();

        public static async Task<Response[]> Load(Request request)
        {
            var tasks = new List<Task<Response>>();
            for (int i = 0; i < request.UriAndMimeTypes.Length; i++)
            {
                var uriAndMimeType = request.UriAndMimeTypes[i];
                tasks.Add(PerLoad(uriAndMimeType.Uri, uriAndMimeType.MimeType, request.CachePolicy, request.UnloadPolicy));
            }
            
            var responses = await Task.WhenAll(tasks);
            return responses;
        }

        public static async Task<SpriteSheetTarget> Load<SpriteSheetTarget>(SpriteSheetRequest request)
        {
            var responses = await Load(request);
            return new SpriteSheetTarget(responses);
        }
        
        public static async Task<ImageTarget> Load<ImageTarget>(ImageRequest request)
        {
            var responses = await Load(request);
            return new ImageTarget(responses);
        }

        private static async Task<Response> PerLoad(string uri, MimeType mimeType, CachePolicy cachePolicy, UnloadPolicy unloadPolicy)
        {
            if (!_loaders.ContainsKey(uri))
            {
                var loader = new Loader(uri, mimeType, cachePolicy, unloadPolicy);
                _loaders[uri] = loader;
            }

            var response = await _loaders[uri].Load();
            return response;
        }
    }

    public class Target
    {
        public Target(Response[] responses)
        {
            
        }
    }

    public class Request
    {
        public UriAndMimeType[] UriAndMimeTypes { get; set; }
        public CachePolicy CachePolicy { get; set; }
        public UnloadPolicy UnloadPolicy { get; set; }
    }
    
    public class UriAndMimeType
    {
        public string Uri { get; set; }
        public MimeType MimeType { get; set; }
    }

    public abstract class Response
    {
        public object Data { get; }
        public string Uri { get; private set; }
        public abstract MimeType MimeType { get; }
        public string Suffix { get; private set; }
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