namespace pfie.http.sourcegen
{
    public class GenerateHttpClientAttribute : Attribute
    {

        public GenerateHttpClientAttribute(string url, string name)
        {
            Url = url;
            Name = name;
        }
        public string Url { get; set; }
        public string Name { get; set; }
    }
}
