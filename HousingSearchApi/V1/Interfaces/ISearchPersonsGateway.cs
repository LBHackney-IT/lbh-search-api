using System.Linq;
using System.Threading.Tasks;
using HousingSearchApi.V1.Boundary.Requests;
using HousingSearchApi.V1.Boundary.Response;
using HousingSearchApi.V1.Domain;

namespace HousingSearchApi.V1.Interfaces
{
    public interface ISearchPersonsGateway
    {
        Task<GetPersonListResponse> GetListOfPersons(GetPersonListRequest query);
    }

    public class SearchPersonsGateway : ISearchPersonsGateway
    {
        private readonly ISearchPersonElasticSearchHelper _elasticSearchHelper;

        public SearchPersonsGateway(ISearchPersonElasticSearchHelper elasticSearchHelper)
        {
            _elasticSearchHelper = elasticSearchHelper;
        }

        public async Task<GetPersonListResponse> GetListOfPersons(GetPersonListRequest request)
        {
            var searchResponse = await _elasticSearchHelper.Search(request).ConfigureAwait(false);
            var personListResponse = new GetPersonListResponse();

            personListResponse.Persons.AddRange(searchResponse.Documents.Select(queryablePerson =>
                Person.Create(queryablePerson)
            ));

            personListResponse.SetTotal(searchResponse.Total);

            return personListResponse;
        }
    }
}
