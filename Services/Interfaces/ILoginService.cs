namespace mail.Services.Interfaces
{
    public interface ILoginService
    {
        string BilgisayarDegerKontrol(string ipv4, string pcUserName);
        bool KontrolPosta();
        bool KontrolSifre();
        bool KontrolIpv4Username();
        void UpdateIpv4Username();
        void UpdateSifre();
        void UpdateTarih();
        bool GirisIslemi(bool internetVarMi);
    }
}
