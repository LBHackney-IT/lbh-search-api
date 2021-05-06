using HousingSearchApi.V1.Boundary.Requests;
using HousingSearchApi.V1.Gateways;
using HousingSearchApi.V1.Gateways.Models;
using Nest;

namespace HousingSearchApi.V1.Interfaces
{
    public class SearchPersonsQueryContainerOrchestrator : ISearchPersonsQueryContainerOrchestrator
    {
        public QueryContainer Create(GetPersonListRequest request,
            QueryContainerDescriptor<QueryablePerson> q)
        {
            return new SearchFirstNames().Create(request, q) ||
                   new SearchSurnames().Create(request, q);
        }
    }
}