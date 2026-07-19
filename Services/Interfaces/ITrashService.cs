using System.Collections.Generic;
using mail.DTO;

namespace mail.Services.Interfaces
{
    public interface ITrashService
    {
        void Senkronize(int toplamMesaj, List<mail_tut> mailler,
            List<mail_bodyfile_tut> bodyfiles, List<mail_attachment_tut> attachmentlar);

        List<trash_get_user> GetMailler();
        List<trash_get_user_dosyalar> GetEklentiler();
        List<trash_get_user_bodyfile> GetBodyfiles();
        void KaliciSil(int donenId);
    }
}
