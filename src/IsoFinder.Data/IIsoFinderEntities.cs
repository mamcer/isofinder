using System.Data.Entity;
using IsoFinder.Core;

namespace IsoFinder.Data
{
    public interface IIsoFinderEntities
    {
        DbSet<IsoFile> IsoFiles { get; set; }

        DbSet<IsoFolder> IsoFolders { get; set; }

        DbSet<IsoVolume> IsoVolumes { get; set; }

        DbSet<IsoRequest> IsoRequests { get; set; }

        DbSet<IsoUser> IsoUsers { get; set; }

        EntityState GetState(object entity);

        void SetModified(object entity);

        DbSet<T> GetSet<T>() where T : class;
    }
}