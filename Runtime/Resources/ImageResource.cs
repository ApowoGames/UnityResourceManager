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
        
        public ImageResource(ResponseFile[] responses) : base(responses)
        {
            ImageResponseFile imageResponseFile = null;
            foreach (var response in responses)
            {
                if (response.MimeType == MimeType.Image)
                {
                    imageResponseFile = response as ImageResponseFile;
                }
            }
            if (imageResponseFile?.Data == null) return;

            Texture2D = imageResponseFile.Data;
        }
    }
}