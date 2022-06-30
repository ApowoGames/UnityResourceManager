namespace ApowoGames.Resources
{
    public class JsonResponse : Response
    {
        public override MimeType MimeType => MimeType.Json;

        public string Data { get; }
    }
}