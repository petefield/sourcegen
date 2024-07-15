
using pfie.animalfacts.domain;
using pfie.http;

namespace pfie.animalfacts.infrastructure;

public interface IAnimalFactsApiClient
{
    Task<ResultOf<AnimalFact>> GetFact();

    Task<Result> PostFact(AnimalFact fact);

    Task<ResultOf<IEnumerable<AnimalFact>>> GetFacts();

    Task<Result> Fail(AnimalFact fact);

}