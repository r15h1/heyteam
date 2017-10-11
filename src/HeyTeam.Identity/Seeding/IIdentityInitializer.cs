using System.Threading.Tasks;

namespace HeyTeam.Identity.Seeding {
    public interface IIdentityInitializer {
        Task Initialize();
    }
}