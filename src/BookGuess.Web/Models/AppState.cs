namespace BookGuess.Web.Models;

public class AppState
{
    public string? Token { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public int Level { get; set; }
    public int XP { get; set; }
    public StartMatchResponse? ActiveMatch { get; set; }
    public List<string> ActiveHints { get; set; } = [];

    public bool IsAuthenticated => !string.IsNullOrEmpty(Token);

    public event Action? OnChange;

    public void SetUser(AuthResponse auth)
    {
        Token = auth.Token;
        UserName = auth.Name;
        UserEmail = auth.Email;
        Level = auth.Level;
        XP = auth.XP;
        NotifyStateChanged();
    }

    public void UpdateXP(int earned)
    {
        XP += earned;
        NotifyStateChanged();
    }

    public void AddHint(string hint)
    {
        ActiveHints.Add(hint);
        NotifyStateChanged();
    }

    public void ClearMatch()
    {
        ActiveMatch = null;
        ActiveHints.Clear();
        NotifyStateChanged();
    }

    public void Clear()
    {
        Token = null;
        UserName = null;
        UserEmail = null;
        Level = 0;
        XP = 0;
        ActiveMatch = null;
        ActiveHints.Clear();
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
