using UnityEngine;

namespace ApowoGames.Resources
{
    public class ImageResponseFile : ResponseFile
    {
        public override MimeType MimeType => MimeType.Image;

        public Texture2D Data => _data;
        
        private Texture2D _data;

        public ImageResponseFile(Texture2D data)
        {
            _data = data;
        }

        public ImageResponseFile(byte[] bytes)
        {
            _data = Texture2D.whiteTexture;
            _data.LoadImage(bytes);
        }
        
        public override void Dispose()
        {
            _data = null;
        }
    }
}