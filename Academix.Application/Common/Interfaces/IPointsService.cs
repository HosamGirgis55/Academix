namespace Academix.Application.Common.Interfaces
{
    public interface IPointsService
    {
        Task<bool> AddPointsToUserAsync(string userId, int points);
        Task<int> GetUserPointsAsync(string userId);
        Task<bool> DeductPointsFromUserAsync(string userId, int points);
        Task<bool> HasSufficientPointsAsync(string userId, int requiredPoints);
    }
} 