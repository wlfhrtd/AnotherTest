namespace MVC.Services
{
    public interface IFileManager
    {
        string ProjectRootPath { get; }
        string FilePath { get; }
    }
}
