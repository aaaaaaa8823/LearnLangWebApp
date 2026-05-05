using LearnEnglishWebApp.DTOs.Request;

namespace LearnEnglishWebApp.Services.Interfaces
{
    public interface ISeedService
    {
        Task SeedAllDataAsync();
    }
}