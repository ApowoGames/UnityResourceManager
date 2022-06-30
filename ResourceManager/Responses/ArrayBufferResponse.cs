namespace ApowoGames.Resources
{
    public class ArrayBufferResponse : Response
    {
        public override MimeType MimeType => MimeType.ArrayBuffer;

        public byte[] Data { get; }
    }
}