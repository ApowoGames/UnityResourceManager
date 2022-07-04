namespace ApowoGames.Resources
{
    public class ArrayBufferResponseFile : ResponseFile
    {
        public override MimeType MimeType => MimeType.ArrayBuffer;

        public byte[] Data => _data;
        
        private byte[] _data;

        public ArrayBufferResponseFile(byte[] data)
        {
            _data = data;
        }

        public override void Dispose()
        {
            _data = null;
        }
    }
}