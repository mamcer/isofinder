namespace IsoFinder.Data
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using Core;

    public class IsoFinderEntities : DbContext, IIsoFinderEntities
    {
        public IsoFinderEntities()
            : base("name=IsoFinderEntities")
        {
        }

        public virtual DbSet<IsoFile> IsoFiles { get; set; }

        public virtual DbSet<IsoFolder> IsoFolders { get; set; }

        public virtual DbSet<IsoVolume> IsoVolumes { get; set; }

        public virtual DbSet<IsoRequest> IsoRequests { get; set; }

        public virtual DbSet<IsoUser> IsoUsers { get; set; }

        public EntityState GetState(object entity)
        {
            return Entry(entity).State;
        }

        public void SetModified(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }

        public DbSet<T> GetSet<T>() where T : class
        {
            return Set<T>();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IsoRequest>()
                .HasMany(c => c.Files)
                .WithMany(m => m.IsoRequests)
                      .Map(
                           m =>
                           {
                               m.MapLeftKey("IsoRequestId");
                               m.MapRightKey("IsoFileId");
                               m.ToTable("IsoFileRequest");
                           });

            modelBuilder.Entity<IsoRequest>()
                .HasMany(c => c.Folders)
                .WithMany(m => m.IsoRequests)
                    .Map(
                           m =>
                           {
                               m.MapLeftKey("IsoRequestId");
                               m.MapRightKey("IsoFolderId");
                               m.ToTable("IsoFolderRequest");
                           });
        }
    }
}