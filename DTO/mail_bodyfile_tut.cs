namespace mail.DTO
{
    // Mail gövdesinin görsele çevrilmiş (bodyfile) halini geçici olarak tutar.
    public class mail_bodyfile_tut
    {
        public int id { get; set; }
        public byte[] alınan_mail_bodyfile { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
}
