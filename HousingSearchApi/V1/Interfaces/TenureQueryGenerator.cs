using System.Collections.Generic;
using HousingSearchApi.V1.Boundary.Requests;
using HousingSearchApi.V1.Gateways.Models.Tenures;
using HousingSearchApi.V1.Infrastructure;
using Nest;

namespace HousingSearchApi.V1.Interfaces
{
    public class TenureQueryGenerator : IQueryGenerator<QueryableTenure>
    {
        private readonly IQueryBuilder<QueryableTenure> _queryBuilder;
        private readonly IWildCardAppenderAndPrepender _wildCardAppenderAndPrepender;

        public TenureQueryGenerator(IQueryBuilder<QueryableTenure> queryBuilder,
            IWildCardAppenderAndPrepender wildCardAppenderAndPrepender)
        {
            _queryBuilder = queryBuilder;
            _wildCardAppenderAndPrepender = wildCardAppenderAndPrepender;
        }

        public QueryContainer Create(HousingSearchRequest request, QueryContainerDescriptor<QueryableTenure> containerDescriptor)
        {
            if (string.IsNullOrWhiteSpace(request.SearchText)) return null;

            var searchFields = new List<string> { "paymentReference", "tenuredAsset.fullAddress^3", "householdMembers", "householdMembers.fullName^3" };

            return _queryBuilder.CreateWildstarSearchQuery(request.SearchText)
                .SpecifyFieldsToBeSearched(searchFields)
                .FilterAndRespectSearchScore(containerDescriptor);
        }
    }
}
