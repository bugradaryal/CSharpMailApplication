using mail.Services;
using mail.Services.Interfaces;
using System;
using System.Windows.Forms;

namespace mail
{
    static class Program
    {
        /// <summary>
        /// Uygulamanın ana girdi noktası.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ILoginService loginService = new LoginService();
            IInboxService inboxService = new InboxService();
            ISentService sentService = new SentService();
            ITrashService trashService = new TrashService();

            Application.Run(new LoginScreen(loginService, inboxService, sentService, trashService));
        }
    }
}
