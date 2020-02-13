using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace qrcode3
{
    public partial class MephiWebAppContext : DbContext
    {
        public MephiWebAppContext()
        {
        }

        public MephiWebAppContext(DbContextOptions<MephiWebAppContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Auth> Auth { get; set; }
        public virtual DbSet<RoomPerson> RoomPerson { get; set; }
        public virtual DbSet<TPerson> TPerson { get; set; }
        public virtual DbSet<TRoom> TRoom { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseNpgsql("Host=194.87.232.95;Port=5432;Database=MephiWebApp;Username=MephiWebApp;Password=myPass12");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Auth>(entity =>
            {
                entity.ToTable("auth");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasColumnName("login")
                    .HasMaxLength(20);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<RoomPerson>(entity =>
            {
                entity.HasKey(e => new { e.IdPerson, e.IdRoom })
                    .HasName("room_person_pkey");

                entity.ToTable("room_person");

                entity.Property(e => e.IdPerson).HasColumnName("id_person");

                entity.Property(e => e.IdRoom).HasColumnName("id_room");
            });

            modelBuilder.Entity<TPerson>(entity =>
            {
                entity.HasKey(e => e.IdPerson)
                    .HasName("t_person_pkey");

                entity.ToTable("t_person");

                entity.Property(e => e.IdPerson)
                    .HasColumnName("id_person")
                    .ValueGeneratedNever();

                entity.Property(e => e.EmailP)
                    .HasColumnName("email_p")
                    .HasMaxLength(20);

                entity.Property(e => e.Name1)
                    .IsRequired()
                    .HasColumnName("name1")
                    .HasMaxLength(15);

                entity.Property(e => e.Name2)
                    .HasColumnName("name2")
                    .HasMaxLength(15);

                entity.Property(e => e.PhonePerson)
                    .HasColumnName("phone_person")
                    .HasMaxLength(15);

                entity.Property(e => e.PhotoP)
                    .HasColumnName("photo_p")
                    .HasMaxLength(100);

                entity.Property(e => e.Surname)
                    .IsRequired()
                    .HasColumnName("surname")
                    .HasMaxLength(15);
            });

            modelBuilder.Entity<TRoom>(entity =>
            {
                entity.HasKey(e => e.IdRoom)
                    .HasName("T_ROOM_pkey");

                entity.ToTable("t_room");

                entity.Property(e => e.IdRoom)
                    .HasColumnName("id_room")
                    .HasDefaultValueSql("nextval('\"T_ROOM_Id_room_seq\"'::regclass)");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(20);

                entity.Property(e => e.LongDesc).HasColumnName("long_desc");

                entity.Property(e => e.NumberRoom).HasColumnName("number_room");

                entity.Property(e => e.Pavilion)
                    .IsRequired()
                    .HasColumnName("pavilion")
                    .HasMaxLength(1);

                entity.Property(e => e.PhoneNumber)
                    .HasColumnName("phone_number")
                    .HasMaxLength(20);

                entity.Property(e => e.ShortDesc)
                    .HasColumnName("short_desc")
                    .HasMaxLength(110);

                //entity.Property(e => e.Pathqr)
                //    .HasColumnName("pathqr")
               //     .HasMaxLength(45);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
