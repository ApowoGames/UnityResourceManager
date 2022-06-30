namespace ApowoGames.Resources
{
    public class ArrayBufferResponse : Response
    {
        public override MimeType MimeType => MimeType.ArrayBuffer;

        public byte[] Data => _data;
        
        private byte[] _data;

        public ArrayBufferResponse(byte[] data)
        {
            _data = data;
        }
    }
}