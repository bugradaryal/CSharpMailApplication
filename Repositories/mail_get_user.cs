using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mail
{
    [Table("mail_get_user")]
    public class mail_get_user
    {
        [Key]
        public int id { get; set; }

        public int kisi_no { get; set; }

        [Required]
        [MaxLength(100)]
        public string yollayan_kisi { get; set; }

        public DateTimeOffset mail_alma_tarhi { get; set; }

        [MaxLength(1000)]
        public string alınan_mail_konu { get; set; }

        public string alınan_mail_icerik { get; set; }

        [NotMapped]
        public string tam_deger
        {
            get
            {
                return $"{alınan_mail_konu}     -/-     {yollayan_kisi}     -/-     {mail_alma_tarhi}";
            }
        }

        [ForeignKey("kisi_no")]
        public virtual login_user Kisi { get; set; }

        public virtual ICollection<mail_get_user_dosyalar> Dosyalar { get; set; }
        public virtual ICollection<mail_get_user_bodyfile> Bodyfiles { get; set; }

        public mail_get_user()
        {
            Dosyalar = new HashSet<mail_get_user_dosyalar>();
            Bodyfiles = new HashSet<mail_get_user_bodyfile>();
        }
    }

    [Table("mail_get_user_dosyalar")]
    public class mail_get_user_dosyalar
    {
        [Key]
        public int id { get; set; }

        public int kisi_no { get; set; }

        public int alınan_mail_no { get; set; }

        public byte[] alınan_mail_dosyalar { get; set; }

        [MaxLength(300)]
        public string attachment_name { get; set; }

        [ForeignKey("alınan_mail_no")]
        public virtual mail_get_user MailGetUser { get; set; }
    }

    [Table("mail_get_user_bodyfile")]
    public class mail_get_user_bodyfile
    {
        [Key]
        public int id { get; set; }

        public int kisi_no { get; set; }

        public int alınan_mail_no { get; set; }

        public byte[] alınan_mail_bodyfile { get; set; }

        public int width { get; set; }
        public int height { get; set; }

        [ForeignKey("alınan_mail_no")]
        public virtual mail_get_user MailGetUser { get; set; }
    }
}
