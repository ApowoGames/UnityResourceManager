namespace ApowoGames.Resources
{
    public class SpriteSheetRequest : Request
    {
        public string ImageUri => UriAndMimeTypes[0].Uri;
        public MimeType ImageMimeType => UriAndMimeTypes[0].MimeType;
        public string JsonUri => UriAndMimeTypes[1].Uri;
        public MimeType JsonMimeType => UriAndMimeTypes[1].MimeType;

        public SpriteSheetRequest(string uri)
        {
            if (uri.EndsWith(".png") || uri.EndsWith(".json"))
            {
                uri = uri.Substring(0, -5);
            }
            
            UriAndMimeTypes = new[]
            {
                new UriAndMimeType()
                {
                    Uri = uri + ".png",
                    MimeType = MimeType.Image
                },
                new UriAndMimeType()
                {
                    Uri = uri + ".json",
                    MimeType = MimeType.Json
                },
            };
            CachePolicy = CachePolicy.永远;
            UnloadPolicy = UnloadPolicy.从不;
        }
        
        public SpriteSheetRequest(string imgUri, string jsonUri)
        {
            if (!imgUri.EndsWith(".png"))
            {
                imgUri += ".png";
            }
            if (!jsonUri.EndsWith(".json"))
            {
                jsonUri += ".json";
            }
            
            UriAndMimeTypes = new[]
            {
                new UriAndMimeType()
                {
                    Uri = imgUri,
                    MimeType = MimeType.Image
                },
                new UriAndMimeType()
                {
                    Uri = jsonUri,
                    MimeType = MimeType.Json
                },
            };
            CachePolicy = CachePolicy.永远;
            UnloadPolicy = UnloadPolicy.从不;
        }
    }
}