namespace ApowoGames.Resources.External
{
    public class StringEnum
    {
        protected StringEnum(string value) { Value = value; }
        public string Value { get; }
        public override string ToString() => Value;
    }
}