using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mail
{
    [Table("login_user")]
    public class login_user
    {
        [Key]
        public int id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Eposta { get; set; }

        [Required]
        [MaxLength(16)]
        public string sifre { get; set; }

        [Required]
        [MaxLength(40)]
        public string IPV4 { get; set; }

        [Required]
        [MaxLength(40)]
        public string pc_user_name { get; set; }

        // NOT: orijinal sınıfta yoktu ama tabloda var (default GETDATE()).
        // Code First eşleşmesi için eklendi.
        public DateTime tarih { get; set; }

        // Navigation property'ler (1 login_user - N mail)
        public virtual ICollection<mail_get_user> AlinanMailler { get; set; }
        public virtual ICollection<mail_send_user> GonderilenMailler { get; set; }
        public virtual ICollection<trash_get_user> SilinenMailler { get; set; }

        public login_user()
        {
            AlinanMailler = new HashSet<mail_get_user>();
            GonderilenMailler = new HashSet<mail_send_user>();
            SilinenMailler = new HashSet<trash_get_user>();
        }

        // Uygulama içinde "aktif giriş yapan kullanıcı" için kullanılan singleton.
        // Static olduğu için EF bunu zaten hiçbir zaman DB alanı olarak görmez, mapping'e girmez.
        public static login_user Instance = new login_user();
    }
}
