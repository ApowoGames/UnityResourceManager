namespace ApowoGames.Resources
{
    public class JsonResponse : Response
    {
        public override MimeType MimeType => MimeType.Json;

        public string Data => _data;
        
        private string _data;

        public JsonResponse(string data)
        {
            _data = data;
        }
    }
}