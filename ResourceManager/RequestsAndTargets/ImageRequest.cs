namespace ApowoGames.Resources
{
    public class ImageRequest : Request
    {
        public string ImageUri => UriAndMimeTypes[0].Uri;
        public MimeType ImageMimeType => UriAndMimeTypes[0].MimeType;

        public ImageRequest(string uri)
        {
            UriAndMimeTypes = new[]
            {
                new UriAndMimeType()
                {
                    Uri = uri + ".png",
                    MimeType = MimeType.Image
                }
            };
            CachePolicy = CachePolicy.永远;
            UnloadPolicy = UnloadPolicy.从不;
        }
    }
}