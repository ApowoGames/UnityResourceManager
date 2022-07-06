using UnityEngine;

namespace ApowoGames.Resources
{
    public class ImageResponseEntity : ResponseEntity
    {
        public override MimeType MimeType => MimeType.Image;

        public new Texture2D Data => _data;
        
        private Texture2D _data;

        public ImageResponseEntity(Texture2D data)
        {
            _data = data;
        }

        public ImageResponseEntity(byte[] bytes)
        {
            _data = new Texture2D(2, 2);
            _data.LoadImage(bytes);
        }
        
        public override void Dispose()
        {
            _data = null;
        }
    }
}