namespace ApowoGames.Resources
{
    public class JsonResponseFile : ResponseFile
    {
        public override MimeType MimeType => MimeType.Json;

        public new string Data => _data;
        
        private string _data;

        public JsonResponseFile(string data)
        {
            _data = data;
        }

        public JsonResponseFile(byte[] bytes)
        {
            _data = System.Text.Encoding.UTF8.GetString(bytes);
        }
        
        public override void Dispose()
        {
            _data = null;
        }
    }
}