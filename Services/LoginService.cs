using System;
using System.Linq;
using mail.Services.Interfaces;

namespace mail.Services
{
    public class LoginService : ILoginService
    {
        public string BilgisayarDegerKontrol(string ipv4, string pcUserName)
        {
            using var db = new DataDbContext();

            var kayit = db.LoginUsers
                .Where(u => u.IPV4 == ipv4 && u.pc_user_name == pcUserName)
                .OrderByDescending(u => u.tarih)
                .FirstOrDefault();

            return kayit?.Eposta ?? "";
        }


        public bool KontrolPosta()
        {
            using var db = new DataDbContext();

            var kayit = db.LoginUsers.FirstOrDefault(u => u.Eposta == login_user.Instance.Eposta);
            if (kayit == null)
                return false;

            login_user.Instance.id = kayit.id;
            return true;
        }

        public bool KontrolSifre()
        {
            using var db = new DataDbContext();

            return db.LoginUsers.Any(u =>
                u.id == login_user.Instance.id &&
                u.sifre == login_user.Instance.sifre);
        }

        public bool KontrolIpv4Username()
        {
            using var db = new DataDbContext();

            return db.LoginUsers.Any(u =>
                u.id == login_user.Instance.id &&
                u.IPV4 == login_user.Instance.IPV4 &&
                u.pc_user_name == login_user.Instance.pc_user_name);
        }

        public void UpdateIpv4Username()
        {
            using var db = new DataDbContext();

            var kayit = db.LoginUsers.First(u => u.id == login_user.Instance.id);
            kayit.IPV4 = login_user.Instance.IPV4;
            kayit.pc_user_name = login_user.Instance.pc_user_name;
            db.SaveChanges();
        }

        public void UpdateSifre()
        {
            using var db = new DataDbContext();

            var kayit = db.LoginUsers.First(u => u.id == login_user.Instance.id);
            kayit.sifre = login_user.Instance.sifre;
            db.SaveChanges();
        }

        public void UpdateTarih()
        {
            using var db = new DataDbContext();

            var kayit = db.LoginUsers.First(u => u.id == login_user.Instance.id);
            kayit.tarih = DateTime.Now;
            db.SaveChanges();
        }


        public bool GirisIslemi(bool internetVarMi)
        {
            if (internetVarMi)
            {
                if (KontrolPosta())
                {
                    if (!KontrolSifre())
                        UpdateSifre(); 

                    if (!KontrolIpv4Username())
                        UpdateIpv4Username();
                }
                else
                {
                    using (var db = new DataDbContext())
                    {
                        var yeni = new login_user
                        {
                            Eposta = login_user.Instance.Eposta,
                            sifre = login_user.Instance.sifre,
                            IPV4 = login_user.Instance.IPV4,
                            pc_user_name = login_user.Instance.pc_user_name,
                            tarih = DateTime.Now
                        };
                        db.LoginUsers.Add(yeni);
                        db.SaveChanges();
                        login_user.Instance.id = yeni.id;
                    }
                }

                UpdateTarih();
                return true;
            }
            else
            {
                if (KontrolPosta() && KontrolSifre())
                {
                    if (!KontrolIpv4Username())
                        UpdateIpv4Username();

                    UpdateTarih();
                    return true;
                }

                return false;
            }
        }
    }
}
