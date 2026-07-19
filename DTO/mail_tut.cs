using System;

namespace mail.DTO
{
    // IMAP/POP3'ten çekilen ham maili DB'ye kaydetmeden önce geçici olarak tutar.
    public class mail_tut
    {
        public int id { get; set; }
        public int unid { get; set; }
        public string yollayan { get; set; }
        public DateTimeOffset tarih { get; set; }
        public string baslik { get; set; }
        public string icerik { get; set; }
    }
}
