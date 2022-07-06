namespace ApowoGames.Resources
{
    public class ArrayBufferResponseEntity : ResponseEntity
    {
        public override MimeType MimeType => MimeType.ArrayBuffer;

        public new byte[] Data => _data;
        
        private byte[] _data;

        public ArrayBufferResponseEntity(byte[] data)
        {
            _data = data;
        }

        public override void Dispose()
        {
            _data = null;
        }
    }
}