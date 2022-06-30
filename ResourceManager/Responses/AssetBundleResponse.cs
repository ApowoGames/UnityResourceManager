using UnityEngine;

namespace ApowoGames.Resources
{
    public class AssetBundleResponse : Response
    {
        public override MimeType MimeType => MimeType.AssetBundle;

        public UnityEngine.Object Data => _data;
        
        private UnityEngine.Object _data;

        public AssetBundleResponse(Object data)
        {
            _data = data;
        }
    }
}