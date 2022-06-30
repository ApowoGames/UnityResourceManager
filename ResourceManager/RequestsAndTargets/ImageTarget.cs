using UnityEngine;

namespace ApowoGames.Resources
{
    public class ImageTarget : Target
    {
        public Texture2D Texture2D;
        
        public ImageTarget(Response[] responses) : base(responses)
        {
            ImageResponse imageResponse = null;
            foreach (var response in responses)
            {
                if (response.MimeType == MimeType.Image)
                {
                    imageResponse = response as ImageResponse;
                }
            }
            if (imageResponse?.Data == null) return;

            Texture2D = imageResponse.Data;
        }
    }
}