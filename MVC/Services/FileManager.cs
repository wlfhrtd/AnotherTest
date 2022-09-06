namespace MVC.Services
{
    public class FileManager : IFileManager
    {
        private string projectRootPath;
        private string filePath;
        // TODO Configuration pattern
        public FileManager()
        {
            projectRootPath = Directory.GetCurrentDirectory();
            filePath = projectRootPath + Path.DirectorySeparatorChar + "departments.json";
        }

        public string ProjectRootPath => projectRootPath;
        public string FilePath => filePath;
    }
}
