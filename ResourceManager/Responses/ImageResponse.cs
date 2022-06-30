namespace ApowoGames.Resources
{
    public class ImageResponse : Response
    {
        public override MimeType MimeType => MimeType.Image;

        public Texture2D Data { get; }
    }
}