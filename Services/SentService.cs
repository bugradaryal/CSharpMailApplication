using System.Collections.Generic;
using System.Linq;
using mail.DTO;
using mail.Services.Interfaces;

namespace mail.Services
{
    public class SentService : ISentService
    {
        public void Senkronize(int toplamMesaj, List<mail_tut> mailler,
            List<mail_bodyfile_tut> bodyfiles, List<mail_attachment_tut> attachmentlar)
        {
            using var db = new DataDbContext();

            foreach (var x in mailler)
            {
                bool zatenVar = db.MailSendUsers.Any(m =>
                    m.kisi_no == login_user.Instance.id &&
                    m.mail_yollama_tarhi == x.tarih &&
                    m.gonderilen_mail_konu == x.baslik &&
                    m.gonderilen_mail_icerik == x.icerik);

                if (zatenVar)
                    continue;

                var yeniMail = new mail_send_user
                {
                    kisi_no = login_user.Instance.id,
                    mail_yollama_tarhi = x.tarih,
                    gonderilen_mail_konu = x.baslik,
                    gonderilen_mail_icerik = x.icerik
                };
                db.MailSendUsers.Add(yeniMail);
                db.SaveChanges(); 

                foreach (var y in bodyfiles.Where(b => b.id == x.id))
                {
                    db.MailSendUserBodyfile.Add(new mail_send_user_bodyfile
                    {
                        gonderilen_mail_no = yeniMail.id,
                        kisi_no = login_user.Instance.id,
                        gonderilen_mail_bodyfile = y.alınan_mail_bodyfile,
                        width = y.width,
                        height = y.height
                    });
                }

                foreach (var y in attachmentlar.Where(a => a.id == x.id))
                {
                    db.MailSendUserDosyalar.Add(new mail_send_user_dosyalar
                    {
                        gonderilen_mail_no = yeniMail.id,
                        kisi_no = login_user.Instance.id,
                        gonderilen_mail_dosyalar = y.alınan_mail_attachment,
                        attachment_name = y.attachment_name
                    });
                }

                db.SaveChanges();
            }

            int dbToplamMesaj = db.MailSendUsers.Count(m => m.kisi_no == login_user.Instance.id);
            if (toplamMesaj < dbToplamMesaj)
            {
                var dbKayitlari = db.MailSendUsers
                    .Where(m => m.kisi_no == login_user.Instance.id)
                    .ToList();

                foreach (var kayit in dbKayitlari)
                {
                    bool sunucudaVar = mailler.Any(m =>
                        m.baslik == kayit.gonderilen_mail_konu &&
                        m.tarih == kayit.mail_yollama_tarhi);

                    if (!sunucudaVar)
                    {
                        db.MailSendUserDosyalar.RemoveRange(
                            db.MailSendUserDosyalar.Where(d => d.gonderilen_mail_no == kayit.id));
                        db.MailSendUserBodyfile.RemoveRange(
                            db.MailSendUserBodyfile.Where(b => b.gonderilen_mail_no == kayit.id));
                        db.MailSendUsers.Remove(kayit);
                    }
                }

                db.SaveChanges();
            }
        }

        public List<mail_send_user> GetMailler()
        {
            using var db = new DataDbContext();
            return db.MailSendUsers
                .Where(m => m.kisi_no == login_user.Instance.id)
                .OrderByDescending(m => m.mail_yollama_tarhi)
                .ToList();
        }

        public List<mail_send_user_dosyalar> GetEklentiler()
        {
            using var db = new DataDbContext();
            return db.MailSendUserDosyalar
                .Where(d => d.kisi_no == login_user.Instance.id)
                .OrderByDescending(d => d.gonderilen_mail_no)
                .ToList();
        }

        public List<mail_send_user_bodyfile> GetBodyfiles()
        {
            using var db = new DataDbContext();
            return db.MailSendUserBodyfile
                .Where(b => b.kisi_no == login_user.Instance.id)
                .OrderByDescending(b => b.gonderilen_mail_no)
                .ToList();
        }


        public void CopeTasi(int donenId, List<mail_send_user> mailler,
            List<mail_send_user_dosyalar> ekler, List<mail_send_user_bodyfile> bodyfiles)
        {
            using var db = new DataDbContext();

            db.MailSendUserDosyalar.RemoveRange(
                db.MailSendUserDosyalar.Where(d => d.gonderilen_mail_no == donenId && d.kisi_no == login_user.Instance.id));
            db.MailSendUserBodyfile.RemoveRange(
                db.MailSendUserBodyfile.Where(b => b.gonderilen_mail_no == donenId && b.kisi_no == login_user.Instance.id));

            var silinecekMail = db.MailSendUsers.FirstOrDefault(m => m.id == donenId && m.kisi_no == login_user.Instance.id);
            if (silinecekMail != null)
                db.MailSendUsers.Remove(silinecekMail);

            db.SaveChanges();

            var x = mailler.Find(m => m.id == donenId);
            if (x == null)
                return;

            var trashMail = new trash_get_user
            {
                kisi_no = login_user.Instance.id,
                yollayan_kisi = login_user.Instance.Eposta,
                mail_alma_tarhi = x.mail_yollama_tarhi,
                alınan_mail_konu = x.gonderilen_mail_konu,
                alınan_mail_icerik = x.gonderilen_mail_icerik
            };
            db.TrashGetUsers.Add(trashMail);
            db.SaveChanges();

            foreach (var y in ekler.Where(e => e.gonderilen_mail_no == x.id))
            {
                db.TrashGetUserDosyalar.Add(new trash_get_user_dosyalar
                {
                    alınan_mail_no = trashMail.id,
                    kisi_no = login_user.Instance.id,
                    alınan_mail_dosyalar = y.gonderilen_mail_dosyalar
                });
            }

            foreach (var z in bodyfiles.Where(b => b.gonderilen_mail_no == x.id))
            {
                db.TrashGetUserBodyfile.Add(new trash_get_user_bodyfile
                {
                    alınan_mail_no = trashMail.id,
                    kisi_no = login_user.Instance.id,
                    alınan_mail_bodyfile = z.gonderilen_mail_bodyfile,
                    width = z.width,
                    height = z.height
                });
            }

            db.SaveChanges();
        }


        public void GeriYukle(int donenId, List<trash_get_user> trashMailler,
            List<trash_get_user_dosyalar> ekler, List<trash_get_user_bodyfile> bodyfiles)
        {
            using var db = new DataDbContext();

            var x = trashMailler.Find(m => m.id == donenId);
            if (x == null)
                return;

            var mail = new mail_send_user
            {
                kisi_no = login_user.Instance.id,
                mail_yollama_tarhi = x.mail_alma_tarhi,
                gonderilen_mail_konu = x.alınan_mail_konu,
                gonderilen_mail_icerik = x.alınan_mail_icerik
            };
            db.MailSendUsers.Add(mail);
            db.SaveChanges();

            foreach (var y in ekler.Where(e => e.alınan_mail_no == x.id))
            {
                db.MailSendUserDosyalar.Add(new mail_send_user_dosyalar
                {
                    gonderilen_mail_no = mail.id,
                    kisi_no = login_user.Instance.id,
                    gonderilen_mail_dosyalar = y.alınan_mail_dosyalar
                });
            }

            foreach (var z in bodyfiles.Where(b => b.alınan_mail_no == x.id))
            {
                db.MailSendUserBodyfile.Add(new mail_send_user_bodyfile
                {
                    gonderilen_mail_no = mail.id,
                    kisi_no = login_user.Instance.id,
                    gonderilen_mail_bodyfile = z.alınan_mail_bodyfile,
                    width = z.width,
                    height = z.height
                });
            }

            db.SaveChanges();
        }
    }
}
