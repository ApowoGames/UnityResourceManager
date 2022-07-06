using UnityEngine;

namespace ApowoGames.Resources
{
    public class ImageResource : Resource
    {
        public Texture2D Texture2D;
        
        public static ImageRequest BuildRequest(string uri)
        {
            return new ImageRequest(uri);
        }
        
        public ImageResource(ResponseEntity[] responses) : base(responses)
        {
            ImageResponseEntity imageResponseEntity = null;
            foreach (var response in responses)
            {
                if (response.MimeType == MimeType.Image)
                {
                    imageResponseEntity = response as ImageResponseEntity;
                }
            }
            if (imageResponseEntity?.Data == null) return;

            Texture2D = imageResponseEntity.Data;
        }
    }
}