using System.Threading.Tasks;
using UnityEngine;

namespace ApowoGames.Resources
{
    public class Loader
    {
        private Response _response;
        private bool _started = false;

        public Loader(string uri, MimeType mimeType, CachePolicy cachePolicy, UnloadPolicy unloadPolicy)
        {
        }

        public async Task<Response> Load()
        {
            if (_response != null)
            {
                return _response;
            }

            if (_started)
            {
                await new WaitUntil(() => _response != null);
                return _response;
            }

            _started = true;

            if (CheckInCache())
            {
                // load from cache
            }
            else
            {
                // load from network
            }

            _started = false;
        }

        private bool CheckInCache()
        {
            
        }
    }
}