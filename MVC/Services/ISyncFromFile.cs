using DAL.EfStructures;


namespace MVC.Services
{
    public interface ISyncFromFile
    {
        Task<int> Sync();
    }
}
