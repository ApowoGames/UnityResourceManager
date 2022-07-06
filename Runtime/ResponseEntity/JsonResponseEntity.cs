namespace ApowoGames.Resources
{
    public class JsonResponseEntity : ResponseEntity
    {
        public override MimeType MimeType => MimeType.Json;

        public new string Data => _data;
        
        private string _data;

        public JsonResponseEntity(string data)
        {
            _data = data;
        }

        public JsonResponseEntity(byte[] bytes)
        {
            _data = System.Text.Encoding.UTF8.GetString(bytes);
        }
        
        public override void Dispose()
        {
            _data = null;
        }
    }
}