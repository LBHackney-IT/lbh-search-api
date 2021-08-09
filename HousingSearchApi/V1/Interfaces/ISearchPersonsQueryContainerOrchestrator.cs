using HousingSearchApi.V1.Boundary.Requests;
using HousingSearchApi.V1.Gateways.Models;
using Nest;

namespace HousingSearchApi.V1.Interfaces
{
    public interface ISearchPersonsQueryContainerOrchestrator
    {
        QueryContainer CreatePerson(GetPersonListRequest request,
            QueryContainerDescriptor<QueryablePerson> q);

        QueryContainer CreateTenure(GetTenureListRequest request, QueryContainerDescriptor<QueryableTenure> queryContainerDescriptor);
    }
}
