using System;
using System.Collections.Generic;
using System.Linq;
using Hackney.Core.ElasticSearch.Interfaces;
using Hackney.Shared.HousingSearch.Gateways.Models.Tenures;
using HousingSearchApi.V1.Boundary.Requests;
using HousingSearchApi.V1.Interfaces.Factories;
using Nest;

namespace HousingSearchApi.V1.Infrastructure.Factories
{
    public class TenureQueryGenerator : IQueryGenerator<QueryableTenure>
    {
        private readonly IQueryBuilder<QueryableTenure> _queryBuilder;

        public TenureQueryGenerator(IQueryBuilder<QueryableTenure> queryBuilder)
        {
            _queryBuilder = queryBuilder;
        }

        public QueryContainer Create<TRequest>(TRequest request, QueryContainerDescriptor<QueryableTenure> q)
        {
            switch (request)
            {
                case GetTenureListRequest tenureListRequest:
                    if (string.IsNullOrWhiteSpace(tenureListRequest.SearchText)){
                        return null;
                    }

                    return _queryBuilder
                   .WithWildstarQuery(tenureListRequest.SearchText, new List<string>
                   {
                            "paymentReference",
                            "tenuredAsset.fullAddress^3",
                            "householdMembers",
                            "householdMembers.fullName^3"
                   }).Build(q);

                case GetTenureListByPrnListRequest tenureListRequestByPrnList:
                    if((tenureListRequestByPrnList.PrnList.Count == 0
                        || tenureListRequestByPrnList.PrnList.Any(t => string.IsNullOrWhiteSpace(t))))
                    {
                        return null;
                    }

                    foreach (var prn in tenureListRequestByPrnList.PrnList)
                    {
                        _queryBuilder
                       .WithWildstarQuery(prn, new List<string>
                       {
                            "paymentReference",
                            "tenuredAsset.fullAddress^3",
                            "householdMembers",
                            "householdMembers.fullName^3"
                       });

                    }

                    return _queryBuilder.Build(q);

                default:
                    throw new ArgumentNullException($"{nameof(request).ToString()} shouldn't be null.");
            }
        }
    }
}
