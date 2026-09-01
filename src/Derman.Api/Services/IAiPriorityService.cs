using Derman.Core.Entities;

namespace Derman.Api.Services;

public interface IAiPriorityService
{
    Task<(string Summary, List<Guid> PriorityIds)> AnalyzeRegionAsync(List<HelpRequest> nearbyRequests);
}