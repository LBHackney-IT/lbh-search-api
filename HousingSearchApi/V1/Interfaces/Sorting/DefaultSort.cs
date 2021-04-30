using HousingSearchApi.V1.Gateways;
using HousingSearchApi.V1.Gateways.Models;
using Nest;

namespace HousingSearchApi.V1.Interfaces.Sorting
{
    public class DefaultSort : IPersonListSort
    {
        public SortDescriptor<QueryablePerson> GetSortDescriptor(SortDescriptor<QueryablePerson> descriptor)
        {
            return descriptor;
        }
    }
}
