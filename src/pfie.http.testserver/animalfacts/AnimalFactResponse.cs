namespace pfie.http.testserver.animalfacts;

internal class AnimalFactResponse
{
    public AnimalFactResponse(string fact)
    {
        this.Fact = fact;
        this.Length = fact.Length;
    }

    public string Fact { get; }

    public int Length { get; }
};

