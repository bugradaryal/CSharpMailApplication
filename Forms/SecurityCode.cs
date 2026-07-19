using System;
using System.Configuration;
using System.Windows.Forms;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Utils;
using mail.Services.Interfaces;

namespace mail
{
    public partial class SecurityCode : Form
    {
        int sayac = 30;
        bool cıkıs;
        GetMail sayfa3;
        private readonly ILoginService loginService;
        int kod_istenme_sayısı;
        int rndm_tutan_deger;
        public int random_fonksiyon()
        {
            Random random = new Random();
            int rand_sayı = random.Next(10000, 99999); //doğrulama codu (5 haneli)
            return rand_sayı;
        }
        public void random_sayi_yolla(int random_sayı)
        {
            var message = new MimeMessage();
            var builder = new BodyBuilder();
            var image = builder.LinkedResources.Add(@"image.jpg");
            image.ContentId = MimeUtils.GenerateMessageId();
            builder.HtmlBody = string.Format
                (
                    @"<b>XyzMail Doğrulama Kodunuz: </b> " + random_sayı +
                    @"<p>Bizi Tercih Ettiğiniz İçin teşekkür ederiz. <br/></p>

                    <img src=""cid:{0}"" width=500 height=200 > ", image.ContentId
                );

            message.From.Add(MailboxAddress.Parse("bugraverify@gmail.com"));
            message.To.Add(MailboxAddress.Parse(login_user.Instance.Eposta));
            message.Subject = "XyzMail - Güvenlik Doğrulama Kodu.";
            message.Body = builder.ToMessageBody();
            var mailUser = ConfigurationManager.AppSettings["MailUser"];
            var mailPass = ConfigurationManager.AppSettings["MailPassword"];
            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
                client.Authenticate(mailUser, mailPass);

                client.Send(message);
                client.Disconnect(true);
            }
        }


        public SecurityCode(ILoginService loginService, IInboxService inboxService,
            ISentService sentService, ITrashService trashService)
        {
            InitializeComponent();
            this.loginService = loginService;
            sayfa3 = new GetMail(inboxService, sentService, trashService);
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            kod_istenme_sayısı = 0;
            rndm_tutan_deger = random_fonksiyon();
            label2.Text = string.Empty;
            label3.Text = string.Empty;
            random_sayi_yolla(rndm_tutan_deger);
            timer1.Interval = 1000;
            timer1.Enabled = true;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (sayac > 0)
            {
                sayac--;
                button2.Text = sayac.ToString();
            }
            else if (sayac == 0)
            {
                button2.Text = "Yeni Kod";
                timer1.Enabled = false;
                sayac = 31;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (sayac == 31)
            {
                kod_istenme_sayısı++;
                rndm_tutan_deger = random_fonksiyon();
                random_sayi_yolla(rndm_tutan_deger);
                label2.Text = "(" + kod_istenme_sayısı + ") Kod Gönderildi!";
                timer1.Enabled = true;
            }
            else
                label2.Text = "Süreyi Beklemelisin.";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == "" || int.Parse(textBox1.Text) < 10000)
                {
                    label3.Text = "Doğrulama Kodu Hatalı!";
                }
                else
                {
                    if (int.Parse(textBox1.Text) == rndm_tutan_deger)
                    {
                        //doğrulama geçerse db işlemleri başlicak
                        if (loginService.GirisIslemi(true) == true)   //db işlemleri
                        {
                            label3.Text = "Güvenlik Doğrulandı!";
                            this.Close();
                            sayfa3.Show();
                        }
                    }
                    else
                    {
                        label3.Text = "Doğrulama Kodu Hatalı!";
                    }
                }
            }
            catch (Exception)
            {
                label3.Text = "Doğrulama Kodu Hatalı! - Exeption";
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Çıkış Yapmak İstediğinize Emin misiniz?", "Çıkış", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                cıkıs = true;   //diablog içinde çıkış yapınca hata veriyor(sanırsam dialogu kapatmaya çalışıyor.)
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

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
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