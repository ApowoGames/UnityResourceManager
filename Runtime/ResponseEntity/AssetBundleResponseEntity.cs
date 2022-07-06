using UnityEngine;

namespace ApowoGames.Resources
{
    public class AssetBundleResponseEntity : ResponseEntity
    {
        public override MimeType MimeType => MimeType.AssetBundle;

        public new AssetBundle Data => _data;
        
        private AssetBundle _data;

        public AssetBundleResponseEntity(AssetBundle data)
        {
            _data = data;
        }

        public AssetBundleResponseEntity(byte[] byts)
        {
            _data = AssetBundle.LoadFromMemory(byts);
        }
        
        public override void Dispose()
        {
            _data.UnloadAsync(true);
            _data = null;
        }
    }
}