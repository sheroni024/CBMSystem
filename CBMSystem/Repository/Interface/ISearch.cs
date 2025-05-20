namespace CBMSystem.Repository.Interface
{
    public interface ISearch
    {
        string? GetRedirectUrlForSearch(string searchTerm);
        IEnumerable<string> GetSuggestions(string searchTerm);

    }
}
