namespace UniversityMS.Application.Common.Behaviours;

public interface ICacheableRequest
{
    string CacheKey { get; }
    TimeSpan? CacheDuration { get; set; }
}