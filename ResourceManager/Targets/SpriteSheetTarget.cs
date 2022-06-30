namespace ApowoGames.Resources
{
    public class SpriteSheetTarget : TargetBase
    {
        public Texture2D Data { get; set; }
        
        public SpriteSheetTarget(Response[] responses) : base(responses)
        {
        }
    }
}