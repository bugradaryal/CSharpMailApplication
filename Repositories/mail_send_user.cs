using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mail
{
    [Table("mail_send_user")]
    public class mail_send_user
    {
        [Key]
        public int id { get; set; }

        public int kisi_no { get; set; }

        public DateTimeOffset mail_yollama_tarhi { get; set; }

        [MaxLength(1000)]
        public string gonderilen_mail_konu { get; set; }

        public string gonderilen_mail_icerik { get; set; }

        [NotMapped]
        public string tam_deger
        {
            get
            {
                return $"{gonderilen_mail_konu}     -/-     {login_user.Instance.Eposta}     -/-     {mail_yollama_tarhi}";
            }
        }

        [ForeignKey("kisi_no")]
        public virtual login_user Kisi { get; set; }

        public virtual ICollection<mail_send_user_dosyalar> Dosyalar { get; set; }
        public virtual ICollection<mail_send_user_bodyfile> Bodyfiles { get; set; }

        public mail_send_user()
        {
            Dosyalar = new HashSet<mail_send_user_dosyalar>();
            Bodyfiles = new HashSet<mail_send_user_bodyfile>();
        }
    }

    [Table("mail_send_user_dosyalar")]
    public class mail_send_user_dosyalar
    {
        [Key]
        public int id { get; set; }

        public int kisi_no { get; set; }

        public int gonderilen_mail_no { get; set; }

        public byte[] gonderilen_mail_dosyalar { get; set; }

        [MaxLength(300)]
        public string attachment_name { get; set; }

        [ForeignKey("gonderilen_mail_no")]
        public virtual mail_send_user MailSendUser { get; set; }
    }

    [Table("mail_send_user_bodyfile")]
    public class mail_send_user_bodyfile
    {
        [Key]
        public int id { get; set; }

        public int kisi_no { get; set; }

        public int gonderilen_mail_no { get; set; }

        public byte[] gonderilen_mail_bodyfile { get; set; }

        public int width { get; set; }
        public int height { get; set; }

        [ForeignKey("gonderilen_mail_no")]
        public virtual mail_send_user MailSendUser { get; set; }
    }
}
