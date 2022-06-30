using System.Collections.Generic;
using System.Threading.Tasks;
using ApowoGames.Resources.External;

namespace ApowoGames.Resources
{
    public static class ResourceManager
    {
        private static Dictionary<string, Loader> _loaders = new Dictionary<string, Loader>();

        public static async Task<T> Load<T>(Request request) where T : TargetBase
        {
            if (request.Uris.Length != request.MimeTypes.Length)
            {
                return null;
            }
            var tasks = new List<Task<Response>>();
            for (int i = 0; i < request.Uris.Length; i++)
            {
                var uri = request.Uris[i];
                var mimeType = request.MimeTypes[i];
                tasks.Add(PerLoad(uri, mimeType, request.CachePolicy, request.UnloadPolicy));
            }
            
            var responses = await Task.WhenAll(tasks);
            return new T(responses);
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

    public abstract class TargetBase
    {
        public TargetBase(Response[] responses)
        {
            
        }
    }

    public class Request
    {
        public string[] Uris { get; set; }
        public MimeType[] MimeTypes { get; set; }
        public CachePolicy CachePolicy { get; set; }
        public UnloadPolicy UnloadPolicy { get; set; }
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