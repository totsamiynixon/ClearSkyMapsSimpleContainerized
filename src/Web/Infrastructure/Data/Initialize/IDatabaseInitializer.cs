using System.Threading.Tasks;

namespace Web.Infrastructure.Data.Initialize
{
    public interface IDatabaseInitializer
    {
        Task InitializeDbAsync();
    }
}