using System.Collections.Generic;
using mail.DTO;

namespace mail.Services.Interfaces
{
    public interface IInboxService
    {
        void Senkronize(int toplamMesaj, List<mail_tut> mailler,
            List<mail_bodyfile_tut> bodyfiles, List<mail_attachment_tut> attachmentlar);

        List<mail_get_user> GetMailler();
        List<mail_get_user_dosyalar> GetEklentiler();
        List<mail_get_user_bodyfile> GetBodyfiles();

        void CopeTasi(int donenId, List<mail_get_user> mailler,
            List<mail_get_user_dosyalar> ekler, List<mail_get_user_bodyfile> bodyfiles);

        void GeriYukle(int donenId, List<trash_get_user> trashMailler,
            List<trash_get_user_dosyalar> ekler, List<trash_get_user_bodyfile> bodyfiles);
    }
}
