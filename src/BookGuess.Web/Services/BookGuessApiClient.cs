using System.Net.Http.Headers;
using System.Net.Http.Json;
using BookGuess.Web.Models;

namespace BookGuess.Web.Services;

public class BookGuessApiClient(HttpClient http, AppState appState)
{
    private void SetAuth()
    {
        if (!string.IsNullOrEmpty(appState.Token))
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", appState.Token);
    }

    public async Task<ApiResponse<AuthResponse>?> RegisterAsync(string name, string email, string password)
    {
        var response = await http.PostAsJsonAsync("/api/auth/register", new { name, email, password });
        return await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
    }

    public async Task<ApiResponse<AuthResponse>?> LoginAsync(string email, string password)
    {
        var response = await http.PostAsJsonAsync("/api/auth/login", new { email, password });
        return await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
    }

    public async Task<ApiResponse<StartMatchResponse>?> StartMatchAsync(bool daily = false)
    {
        SetAuth();
        var response = await http.PostAsync($"/api/matches/start?daily={daily}", null);
        return await response.Content.ReadFromJsonAsync<ApiResponse<StartMatchResponse>>();
    }

    public async Task<ApiResponse<GuessResponse>?> SubmitGuessAsync(Guid matchId, string guessText, string guessType)
    {
        SetAuth();
        var response = await http.PostAsJsonAsync($"/api/matches/{matchId}/guess", new { guessText, guessType });
        return await response.Content.ReadFromJsonAsync<ApiResponse<GuessResponse>>();
    }

    public async Task<ApiResponse<HintResponse>?> RequestHintAsync(Guid matchId)
    {
        SetAuth();
        var response = await http.PostAsync($"/api/matches/{matchId}/hint", null);
        return await response.Content.ReadFromJsonAsync<ApiResponse<HintResponse>>();
    }

    public async Task<ApiResponse<IEnumerable<RankingEntry>>?> GetRankingAsync(string tab = "global")
    {
        var response = await http.GetAsync($"/api/ranking/{tab}");
        return await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<RankingEntry>>>();
    }

    public async Task<ApiResponse<ProfileResponse>?> GetProfileAsync()
    {
        SetAuth();
        var response = await http.GetAsync("/api/profile");
        return await response.Content.ReadFromJsonAsync<ApiResponse<ProfileResponse>>();
    }
}
