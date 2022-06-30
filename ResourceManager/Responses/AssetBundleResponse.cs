namespace ApowoGames.Resources
{
    public class AssetBundleResponse : Response
    {
        public override MimeType MimeType => MimeType.AssetBundle;

        public UnityEngine.Object Data { get; }
    }
}