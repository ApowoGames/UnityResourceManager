using UnityEngine;

namespace ApowoGames.Resources
{
    public class ImageResponse : Response
    {
        public override MimeType MimeType => MimeType.Image;

        public Texture2D Data => _data;
        
        private Texture2D _data;

        public ImageResponse(Texture2D data)
        {
            _data = data;
        }
    }
}