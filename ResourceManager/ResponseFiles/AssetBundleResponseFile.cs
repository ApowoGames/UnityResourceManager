using UnityEngine;

namespace ApowoGames.Resources
{
    public class AssetBundleResponseFile : ResponseFile
    {
        public override MimeType MimeType => MimeType.AssetBundle;

        public AssetBundle Data => _data;
        
        private AssetBundle _data;

        public AssetBundleResponseFile(AssetBundle data)
        {
            _data = data;
        }

        public AssetBundleResponseFile(byte[] byts)
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