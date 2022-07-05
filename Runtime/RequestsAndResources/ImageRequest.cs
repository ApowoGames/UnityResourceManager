namespace ApowoGames.Resources
{
    public class ImageRequest : Request
    {
        public string ImageUri => UriAndMimeTypes[0].Uri;
        public MimeType ImageMimeType => UriAndMimeTypes[0].MimeType;

        public ImageRequest(string uri)
        {
            if (!uri.EndsWith(".png"))
            {
                uri += ".png";
            }
            
            UriAndMimeTypes = new[]
            {
                new UriAndMimeType()
                {
                    Uri = uri,
                    MimeType = MimeType.Image
                }
            };
            CachePolicy = CachePolicy.永远;
            UnloadPolicy = UnloadPolicy.从不;
        }

        public override Resource GenerateResource(ResponseFile[] responses)
        {
            return new ImageResource(responses);
        }
    }
}