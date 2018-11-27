namespace SimpleHelper
{
    public class RequestHeaderItems
    {
        public enum HeaderType
        {
            Authorization,
            Others
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}