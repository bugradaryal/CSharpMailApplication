using System.Collections.Generic;
using System.Linq;
using mail.DTO;
using mail.Services.Interfaces;

namespace mail.Services
{
    public class InboxService : IInboxService
    {
        public void Senkronize(int toplamMesaj, List<mail_tut> mailler,
            List<mail_bodyfile_tut> bodyfiles, List<mail_attachment_tut> attachmentlar)
        {
            using var db = new DataDbContext();

            foreach (var x in mailler)
            {
                bool zatenVar = db.MailGetUsers.Any(m =>
                    m.kisi_no == login_user.Instance.id &&
                    m.yollayan_kisi == x.yollayan &&
                    m.mail_alma_tarhi == x.tarih &&
                    m.alınan_mail_konu == x.baslik &&
                    m.alınan_mail_icerik == x.icerik);

                if (zatenVar)
                    continue;

                var yeniMail = new mail_get_user
                {
                    kisi_no = login_user.Instance.id,
                    yollayan_kisi = x.yollayan,
                    mail_alma_tarhi = x.tarih,
                    alınan_mail_konu = x.baslik,
                    alınan_mail_icerik = x.icerik
                };
                db.MailGetUsers.Add(yeniMail);
                db.SaveChanges();

                foreach (var y in bodyfiles.Where(b => b.id == x.id))
                {
                    db.MailGetUserBodyfile.Add(new mail_get_user_bodyfile
                    {
                        alınan_mail_no = yeniMail.id,
                        kisi_no = login_user.Instance.id,
                        alınan_mail_bodyfile = y.alınan_mail_bodyfile,
                        width = y.width,
                        height = y.height
                    });
                }

                foreach (var y in attachmentlar.Where(a => a.id == x.id))
                {
                    db.MailGetUserDosyalar.Add(new mail_get_user_dosyalar
                    {
                        alınan_mail_no = yeniMail.id,
                        kisi_no = login_user.Instance.id,
                        alınan_mail_dosyalar = y.alınan_mail_attachment,
                        attachment_name = y.attachment_name
                    });
                }

                db.SaveChanges();
            }


            int dbToplamMesaj = db.MailGetUsers.Count(m => m.kisi_no == login_user.Instance.id);
            if (toplamMesaj < dbToplamMesaj)
            {
                var dbKayitlari = db.MailGetUsers
                    .Where(m => m.kisi_no == login_user.Instance.id)
                    .ToList();

                foreach (var kayit in dbKayitlari)
                {
                    bool sunucudaVar = mailler.Any(m =>
                        m.baslik == kayit.alınan_mail_konu &&
                        m.tarih == kayit.mail_alma_tarhi &&
                        m.yollayan == kayit.yollayan_kisi);

                    if (!sunucudaVar)
                    {
                        db.MailGetUserDosyalar.RemoveRange(
                            db.MailGetUserDosyalar.Where(d => d.alınan_mail_no == kayit.id));
                        db.MailGetUserBodyfile.RemoveRange(
                            db.MailGetUserBodyfile.Where(b => b.alınan_mail_no == kayit.id));
                        db.MailGetUsers.Remove(kayit);
                    }
                }

                db.SaveChanges();
            }
        }

        public List<mail_get_user> GetMailler()
        {
            using var db = new DataDbContext();
            return db.MailGetUsers
                .Where(m => m.kisi_no == login_user.Instance.id)
                .OrderByDescending(m => m.mail_alma_tarhi)
                .ToList();
        }

        public List<mail_get_user_dosyalar> GetEklentiler()
        {
            using var db = new DataDbContext();
            return db.MailGetUserDosyalar
                .Where(d => d.kisi_no == login_user.Instance.id)
                .OrderByDescending(d => d.alınan_mail_no)
                .ToList();
        }

        public List<mail_get_user_bodyfile> GetBodyfiles()
        {
            using var db = new DataDbContext();
            return db.MailGetUserBodyfile
                .Where(b => b.kisi_no == login_user.Instance.id)
                .OrderByDescending(b => b.alınan_mail_no)
                .ToList();
        }


        public void CopeTasi(int donenId, List<mail_get_user> mailler,
            List<mail_get_user_dosyalar> ekler, List<mail_get_user_bodyfile> bodyfiles)
        {
            using var db = new DataDbContext();

            db.MailGetUserDosyalar.RemoveRange(
                db.MailGetUserDosyalar.Where(d => d.alınan_mail_no == donenId && d.kisi_no == login_user.Instance.id));
            db.MailGetUserBodyfile.RemoveRange(
                db.MailGetUserBodyfile.Where(b => b.alınan_mail_no == donenId && b.kisi_no == login_user.Instance.id));

            var silinecekMail = db.MailGetUsers.FirstOrDefault(m => m.id == donenId && m.kisi_no == login_user.Instance.id);
            if (silinecekMail != null)
                db.MailGetUsers.Remove(silinecekMail);

            db.SaveChanges();

            var x = mailler.Find(m => m.id == donenId);
            if (x == null)
                return;

            var trashMail = new trash_get_user
            {
                kisi_no = login_user.Instance.id,
                yollayan_kisi = x.yollayan_kisi,
                mail_alma_tarhi = x.mail_alma_tarhi,
                alınan_mail_konu = x.alınan_mail_konu,
                alınan_mail_icerik = x.alınan_mail_icerik
            };
            db.TrashGetUsers.Add(trashMail);
            db.SaveChanges();

            foreach (var y in ekler.Where(e => e.alınan_mail_no == x.id))
            {
                db.TrashGetUserDosyalar.Add(new trash_get_user_dosyalar
                {
                    alınan_mail_no = trashMail.id,
                    kisi_no = login_user.Instance.id,
                    alınan_mail_dosyalar = y.alınan_mail_dosyalar
                });
            }

            foreach (var z in bodyfiles.Where(b => b.alınan_mail_no == x.id))
            {
                db.TrashGetUserBodyfile.Add(new trash_get_user_bodyfile
                {
                    alınan_mail_no = trashMail.id,
                    kisi_no = login_user.Instance.id,
                    alınan_mail_bodyfile = z.alınan_mail_bodyfile,
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

            var mail = new mail_get_user
            {
                kisi_no = login_user.Instance.id,
                yollayan_kisi = x.yollayan_kisi,
                mail_alma_tarhi = x.mail_alma_tarhi,
                alınan_mail_konu = x.alınan_mail_konu,
                alınan_mail_icerik = x.alınan_mail_icerik
            };
            db.MailGetUsers.Add(mail);
            db.SaveChanges(); // mail.id burada dolar

            foreach (var y in ekler.Where(e => e.alınan_mail_no == x.id))
            {
                db.MailGetUserDosyalar.Add(new mail_get_user_dosyalar
                {
                    alınan_mail_no = mail.id,
                    kisi_no = login_user.Instance.id,
                    alınan_mail_dosyalar = y.alınan_mail_dosyalar
                });
            }

            foreach (var z in bodyfiles.Where(b => b.alınan_mail_no == x.id))
            {
                db.MailGetUserBodyfile.Add(new mail_get_user_bodyfile
                {
                    alınan_mail_no = mail.id,
                    kisi_no = login_user.Instance.id,
                    alınan_mail_bodyfile = z.alınan_mail_bodyfile,
                    width = z.width,
                    height = z.height
                });
            }

            db.SaveChanges();
        }
    }
}
