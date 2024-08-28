
using pfie.http.sourcegen;

namespace pfie.animalfacts.infra
{
    [GenerateHttpClient("AnimalFactsApi", "http://localhost:7070/swagger/v1/swagger.json")]
    public partial class AnimalFactsApiClient 
    { }
}
