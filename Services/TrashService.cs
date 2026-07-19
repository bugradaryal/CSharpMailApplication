using System.Collections.Generic;
using System.Linq;
using mail.DTO;
using mail.Services.Interfaces;

namespace mail.Services
{
    public class TrashService : ITrashService
    {
        public void Senkronize(int toplamMesaj, List<mail_tut> mailler,
            List<mail_bodyfile_tut> bodyfiles, List<mail_attachment_tut> attachmentlar)
        {
            using var db = new DataDbContext();

            foreach (var x in mailler)
            {
                bool zatenVar = db.TrashGetUsers.Any(m =>
                    m.kisi_no == login_user.Instance.id &&
                    m.yollayan_kisi == x.yollayan &&
                    m.mail_alma_tarhi == x.tarih &&
                    m.alınan_mail_konu == x.baslik &&
                    m.alınan_mail_icerik == x.icerik);

                if (zatenVar)
                    continue;

                var yeniMail = new trash_get_user
                {
                    kisi_no = login_user.Instance.id,
                    yollayan_kisi = x.yollayan,
                    mail_alma_tarhi = x.tarih,
                    alınan_mail_konu = x.baslik,
                    alınan_mail_icerik = x.icerik
                };
                db.TrashGetUsers.Add(yeniMail);
                db.SaveChanges();

                foreach (var y in bodyfiles.Where(b => b.id == x.id))
                {
                    db.TrashGetUserBodyfile.Add(new trash_get_user_bodyfile
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
                    db.TrashGetUserDosyalar.Add(new trash_get_user_dosyalar
                    {
                        alınan_mail_no = yeniMail.id,
                        kisi_no = login_user.Instance.id,
                        alınan_mail_dosyalar = y.alınan_mail_attachment,
                        attachment_name = y.attachment_name
                    });
                }

                db.SaveChanges();
            }

            int dbToplamMesaj = db.TrashGetUsers.Count(m => m.kisi_no == login_user.Instance.id);
            if (toplamMesaj < dbToplamMesaj)
            {
                var dbKayitlari = db.TrashGetUsers
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
                        db.TrashGetUserDosyalar.RemoveRange(
                            db.TrashGetUserDosyalar.Where(d => d.alınan_mail_no == kayit.id));
                        db.TrashGetUserBodyfile.RemoveRange(
                            db.TrashGetUserBodyfile.Where(b => b.alınan_mail_no == kayit.id));
                        db.TrashGetUsers.Remove(kayit);
                    }
                }

                db.SaveChanges();
            }
        }

        public List<trash_get_user> GetMailler()
        {
            using var db = new DataDbContext();
            return db.TrashGetUsers
                .Where(m => m.kisi_no == login_user.Instance.id)
                .OrderByDescending(m => m.mail_alma_tarhi)
                .ToList();
        }

        public List<trash_get_user_dosyalar> GetEklentiler()
        {
            using var db = new DataDbContext();
            return db.TrashGetUserDosyalar
                .Where(d => d.kisi_no == login_user.Instance.id)
                .OrderByDescending(d => d.alınan_mail_no)
                .ToList();
        }

        public List<trash_get_user_bodyfile> GetBodyfiles()
        {
            using var db = new DataDbContext();
            return db.TrashGetUserBodyfile
                .Where(b => b.kisi_no == login_user.Instance.id)
                .OrderByDescending(b => b.alınan_mail_no)
                .ToList();
        }
        public void KaliciSil(int donenId)
        {
            using var db = new DataDbContext();

            db.TrashGetUserDosyalar.RemoveRange(
                db.TrashGetUserDosyalar.Where(d => d.alınan_mail_no == donenId && d.kisi_no == login_user.Instance.id));
            db.TrashGetUserBodyfile.RemoveRange(
                db.TrashGetUserBodyfile.Where(b => b.alınan_mail_no == donenId && b.kisi_no == login_user.Instance.id));

            var silinecek = db.TrashGetUsers.FirstOrDefault(m => m.id == donenId && m.kisi_no == login_user.Instance.id);
            if (silinecek != null)
                db.TrashGetUsers.Remove(silinecek);

            db.SaveChanges();
        }
    }
}
