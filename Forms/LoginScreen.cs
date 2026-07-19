using System;
using System.Windows.Forms;
using System.Net;
using System.Net.NetworkInformation;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Drawing;
using mail.Services.Interfaces;

namespace mail
{
    public partial class LoginScreen : Form
    {
        private readonly ILoginService loginService;
        private readonly IInboxService inboxService;
        private readonly ISentService sentService;
        private readonly ITrashService trashService;

        string pc_name, ip, posta = "", sifre, db_postadeger, tekrar_sifre;
        bool hata_varmı = false, cıkıs;
        SecurityCode sayfa2;
        GetMail sayfa3;

        public LoginScreen(ILoginService loginService, IInboxService inboxService,
            ISentService sentService, ITrashService trashService)
        {
            InitializeComponent();
            this.loginService = loginService;
            this.inboxService = inboxService;
            this.sentService = sentService;
            this.trashService = trashService;

            sayfa2 = new SecurityCode(loginService, inboxService, sentService, trashService);
            sayfa3 = new GetMail(inboxService, sentService, trashService);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label4.Text = "";
            textBox2.UseSystemPasswordChar = true;
            textBox3.UseSystemPasswordChar = true;
            try // database bağlantısı kontrolü
            {
                pc_name = Environment.MachineName;
                ip = Dns.GetHostAddresses(Environment.MachineName)[1].ToString();
                login_user.Instance.IPV4 = ip;
                login_user.Instance.pc_user_name = pc_name;

                db_postadeger = loginService.BilgisayarDegerKontrol(ip, pc_name);
            }
            catch (Exception)
            {
                label4.Text = "Database bağlantı hatası!";
            }

            if (db_postadeger == "")
                textBox1.Text = "E posta giriniz...";
            else
            {
                posta = db_postadeger;
                textBox1.Text = posta;       //Oto doldurma kısmı
            }

        }
        public void işlemler()
        {
            posta = textBox1.Text;
            if (posta == "" || posta == "E posta giriniz...")
            {
                label4.ForeColor = Color.Maroon;
                label4.Text = "Eposta kısmı boş geçilemez!";
            }
            else
            {
                sifre = textBox2.Text;
                tekrar_sifre = textBox3.Text;
                if (sifre == tekrar_sifre && sifre != "")   //şifreler uyuyorsa ve boş değilse data kontrol olcak(int için)
                {                                          //ve int varsa bağlantı kotnrolü olcak
                    login_user.Instance.Eposta = posta;
                    login_user.Instance.sifre = sifre;
                    if (NetworkInterface.GetIsNetworkAvailable() == true)    //internet bağlantısı var ise mail hesabı senin mi diye kontrol
                    {
                        try
                        {
                            var client = new SmtpClient();

                            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                            client.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
                            client.Authenticate(login_user.Instance.Eposta, login_user.Instance.sifre);
                            client.Disconnect(true);
                        }
                        catch (Exception)
                        {
                            hata_varmı = true;
                        }
                        if (hata_varmı == true)
                        {
                            label4.ForeColor = Color.Maroon;
                            label4.Text = "Giriş İşlemi Başarısız / Şifre veya Mail Hatalı!";
                        }
                        else //hata vermezse internetli giriş başlicak
                        {
                            sayfa2.Show();
                            this.Hide();
                        }
                    }
                    else
                    {
                        if (loginService.GirisIslemi(false) == true)
                        {
                            sayfa3.Show();
                            this.Hide();
                        }
                        else
                        {
                            label4.ForeColor = Color.Maroon;
                            label4.Text = "Giriş İşlemi Başarısız / Şifre veya Mail Hatalı!";
                        }
                    }
                }
                else
                {
                    label4.ForeColor = Color.Maroon;
                    if (sifre == "")
                        label4.Text = "Şifre boş geçilemez!";
                    else
                        label4.Text = "Şifre uyuşmazlığı!";
                }

            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox2.UseSystemPasswordChar = false;
                textBox3.UseSystemPasswordChar = false;
            }
            else
            {
                textBox2.UseSystemPasswordChar = true;
                textBox3.UseSystemPasswordChar = true;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label4.ForeColor = Color.Green;
            label4.Text = "Giriş İşlemi Başlatılıyor...";
            işlemler();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Çıkış Yapmak İstediğinize Emin misiniz?", "Çıkış", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                cıkıs = true;
            }
            if (cıkıs == true)
                Environment.Exit(1);
        }
        bool move;
        int mouse_x, mouse_y;
        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            move = true;
            mouse_x = e.X;
            mouse_y = e.Y;
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            move = false;
        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if (move)
            {
                this.SetDesktopLocation(MousePosition.X - mouse_x, MousePosition.Y - mouse_y);
            }
        }
    }
}