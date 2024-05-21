namespace RepositoriesStorage.RepositoriesStorage;

public readonly struct DefaultRepositoryIdentity : IRepositoryIdentity
{
    private readonly string _userName;
    private readonly string _repositoryTitle;

    public DefaultRepositoryIdentity(string userName, string repositoryTitle)
    {
        _userName = userName;
        _repositoryTitle = repositoryTitle;
    }

    public string RepositoryName => Path.Combine(_userName, _repositoryTitle);
}