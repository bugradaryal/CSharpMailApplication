using System.Configuration;
using Microsoft.EntityFrameworkCore;

namespace mail
{
    public class DataDbContext : DbContext
    {
        public DataDbContext()
        {
        }

        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connString = ConfigurationManager.AppSettings["MailDbConnection"];
                optionsBuilder.UseSqlServer(connString);
            }
        }

        public DbSet<login_user> LoginUsers { get; set; }

        public DbSet<mail_get_user> MailGetUsers { get; set; }
        public DbSet<mail_get_user_dosyalar> MailGetUserDosyalar { get; set; }
        public DbSet<mail_get_user_bodyfile> MailGetUserBodyfile { get; set; }

        public DbSet<mail_send_user> MailSendUsers { get; set; }
        public DbSet<mail_send_user_dosyalar> MailSendUserDosyalar { get; set; }
        public DbSet<mail_send_user_bodyfile> MailSendUserBodyfile { get; set; }

        public DbSet<trash_get_user> TrashGetUsers { get; set; }
        public DbSet<trash_get_user_dosyalar> TrashGetUserDosyalar { get; set; }
        public DbSet<trash_get_user_bodyfile> TrashGetUserBodyfile { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ---------------- login_user ----------------
            modelBuilder.Entity<login_user>().ToTable("login_user");
            modelBuilder.Entity<login_user>().HasKey(x => x.id);
            modelBuilder.Entity<login_user>().Property(x => x.Eposta).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<login_user>().Property(x => x.sifre).IsRequired().HasMaxLength(16);
            modelBuilder.Entity<login_user>().Property(x => x.IPV4).IsRequired().HasMaxLength(40);
            modelBuilder.Entity<login_user>().Property(x => x.pc_user_name).IsRequired().HasMaxLength(40);

            // ---------------- mail_get_user ----------------
            modelBuilder.Entity<mail_get_user>().ToTable("mail_get_user");
            modelBuilder.Entity<mail_get_user>()
                .HasOne(m => m.Kisi)
                .WithMany(l => l.AlinanMailler)
                .HasForeignKey(m => m.kisi_no)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<mail_get_user_dosyalar>().ToTable("mail_get_user_dosyalar");
            modelBuilder.Entity<mail_get_user_dosyalar>()
                .HasOne(d => d.MailGetUser)
                .WithMany(m => m.Dosyalar)
                .HasForeignKey(d => d.alınan_mail_no)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<mail_get_user_bodyfile>().ToTable("mail_get_user_bodyfile");
            modelBuilder.Entity<mail_get_user_bodyfile>()
                .HasOne(b => b.MailGetUser)
                .WithMany(m => m.Bodyfiles)
                .HasForeignKey(b => b.alınan_mail_no)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------------- mail_send_user ----------------
            modelBuilder.Entity<mail_send_user>().ToTable("mail_send_user");
            modelBuilder.Entity<mail_send_user>()
                .HasOne(m => m.Kisi)
                .WithMany(l => l.GonderilenMailler)
                .HasForeignKey(m => m.kisi_no)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<mail_send_user_dosyalar>().ToTable("mail_send_user_dosyalar");
            modelBuilder.Entity<mail_send_user_dosyalar>()
                .HasOne(d => d.MailSendUser)
                .WithMany(m => m.Dosyalar)
                .HasForeignKey(d => d.gonderilen_mail_no)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<mail_send_user_bodyfile>().ToTable("mail_send_user_bodyfile");
            modelBuilder.Entity<mail_send_user_bodyfile>()
                .HasOne(b => b.MailSendUser)
                .WithMany(m => m.Bodyfiles)
                .HasForeignKey(b => b.gonderilen_mail_no)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------------- trash_get_user ----------------
            modelBuilder.Entity<trash_get_user>().ToTable("trash_get_user");
            modelBuilder.Entity<trash_get_user>()
                .HasOne(m => m.Kisi)
                .WithMany(l => l.SilinenMailler)
                .HasForeignKey(m => m.kisi_no)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<trash_get_user_dosyalar>().ToTable("trash_get_user_dosyalar");
            modelBuilder.Entity<trash_get_user_dosyalar>()
                .HasOne(d => d.TrashGetUser)
                .WithMany(m => m.Dosyalar)
                .HasForeignKey(d => d.alınan_mail_no)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<trash_get_user_bodyfile>().ToTable("trash_get_user_bodyfile");
            modelBuilder.Entity<trash_get_user_bodyfile>()
                .HasOne(b => b.TrashGetUser)
                .WithMany(m => m.Bodyfiles)
                .HasForeignKey(b => b.alınan_mail_no)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}