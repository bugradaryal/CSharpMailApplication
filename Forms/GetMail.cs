using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Web;
using MarkupConverter;
using MailKit.Net.Imap;
using MailKit.Security;
using MailKit;
using MimeKit;
using System.IO;
using MailKit.Search;
using mail.DTO;
using mail.Services.Interfaces;

namespace mail
{
    public partial class GetMail : Form
    {
        private readonly IInboxService inboxService;
        private readonly ISentService sentService;
        private readonly ITrashService trashService;
        SendMail sayfa4 = new SendMail();
        bool cıkıs;
        int button_no = 1;
        List<mail_get_user> gelen_kutusu = new List<mail_get_user>();
        List<mail_get_user_dosyalar> gelen_kutusu_dosya = new List<mail_get_user_dosyalar>();
        List<mail_get_user_bodyfile> gelen_kutusu_bodyfile = new List<mail_get_user_bodyfile>();

        List<mail_send_user> giden_kutusu = new List<mail_send_user>();
        List<mail_send_user_dosyalar> giden_kutusu_dosya = new List<mail_send_user_dosyalar>();
        List<mail_send_user_bodyfile> giden_kutusu_bodyfile = new List<mail_send_user_bodyfile>();

        List<trash_get_user> trash_kutusu = new List<trash_get_user>();
        List<trash_get_user_dosyalar> trash_kutusu_dosya = new List<trash_get_user_dosyalar>();
        List<trash_get_user_bodyfile> trash_kutusu_bodyfile = new List<trash_get_user_bodyfile>();

        public GetMail(IInboxService inboxService, ISentService sentService, ITrashService trashService)
        {
            InitializeComponent();
            this.inboxService = inboxService;
            this.sentService = sentService;
            this.trashService = trashService;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            backgroundWorker1.WorkerReportsProgress = true;
            pictureBox1.BackColor = Color.Transparent;
            listBox2.HorizontalScrollbar = true;
            listBox1.HorizontalScrollbar = true;
            gelen_kutusu = inboxService.GetMailler();
            gelen_kutusu_bodyfile = inboxService.GetBodyfiles();
            gelen_kutusu_dosya = inboxService.GetEklentiler();
            listBox2.DataSource = gelen_kutusu;
            listBox2.DisplayMember = "tam_deger";
            label2.Text = $"Total: {gelen_kutusu.Count}";
            label3.Text = "Gelen Kutusu:";
            giden_kutusu = sentService.GetMailler();
            giden_kutusu_bodyfile = sentService.GetBodyfiles();
            giden_kutusu_dosya = sentService.GetEklentiler();
            trash_kutusu = trashService.GetMailler();
            trash_kutusu_bodyfile = trashService.GetBodyfiles();
            trash_kutusu_dosya = trashService.GetEklentiler();


            if (NetworkInterface.GetIsNetworkAvailable() == true)
            {
                backgroundWorker1.WorkerReportsProgress = true;
                backgroundWorker1.WorkerSupportsCancellation = true;
                button6.BorderColor = Color.DarkGreen;
                label1.Text = "Mailler güncelleniyor...";
                backgroundWorker1.RunWorkerAsync();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (NetworkInterface.GetIsNetworkAvailable() == true)
                sayfa4.Show();
            else
                MessageBox.Show("İnternet Bağlantısı olmadan Mail gönderemezsiniz!!!", "Hata");
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

        public static string ConvertHtmlToText(string html)
        {

            string tut;
            tut = html.Replace("\r", " ");
            tut = tut.Replace("\n", " ");
            tut = tut.Replace("\t", string.Empty);
            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                                                                  @"( )+", " ");

            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     @"<( )*head([^>])*>", "<head>",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     @"(<( )*(/)( )*head( )*>)", "</head>",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     "(<head>).*(</head>)", string.Empty,
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     @"<( )*script([^>])*>", "<script>",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     @"(<( )*(/)( )*script( )*>)", "</script>",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);



            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     @"(<script>).*(</script>)", string.Empty,
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);



            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     @"<( )*style([^>])*>", "<style>",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     @"(<( )*(/)( )*style( )*>)", "</style>",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     "(<style>).*(</style>)", string.Empty,
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);


            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     @"<( )*td([^>])*>", "\t",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);


            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     @"<( )*br( )*>", "\r",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     @"<( )*li( )*>", "\r",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);


            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     @"<( )*div([^>])*>", "\r\r",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     @"<( )*tr([^>])*>", "\r\r",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     @"<( )*p([^>])*>", "\r\r",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);



            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     @"<[^>]*>", string.Empty,
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);


            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     @"&nbsp;", " ",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     @"&bull;", " * ",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     @"&lsaquo;", "<",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     @"&rsaquo;", ">",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     @"&trade;", "(tm)",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     @"&frasl;", "/",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     @"<", "<",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     @">", ">",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     @"&copy;", "(c)",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     @"&reg;", "(r)",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);


            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     @"&(.{2,6});", string.Empty,
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);


            tut = tut.Replace("\n", "\r");

            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     "(\r)( )+(\r)", "\r\r",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     "(\t)( )+(\t)", "\t\t",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     "(\t)( )+(\r)", "\t\r",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     "(\r)( )+(\t)", "\r\t",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     "(\r)(\t)+(\r)", "\r\r",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            tut = System.Text.RegularExpressions.Regex.Replace(tut,
                     "(\r)(\t)+", "\r\t",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            string breaks = "\r\r\r";

            string tabs = "\t\t\t\t\t";
            for (int i = 0; i < tut.Length; i++)
            {
                tut = tut.Replace(breaks, "\r\r");
                tut = tut.Replace(tabs, "\t\t\t\t");
                breaks = breaks + "\r";
                tabs = tabs + "\t";
            }
            return tut;
        }



        int metinbaslangicIndex = 0;
        int dizi;
        bool hataa = false;
        Graphics _graphics;
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                dizi = listBox2.SelectedIndex;
                string tut;
                if (listBox2.SelectedItems.Count > 0)
                {
                    if (pictureBox3.BackColor == Color.Green)
                        pictureBox3.BackColor = Color.Red;
                    okundumu = true;
                    richTextBox1.Clear();
                    metinbaslangicIndex = 0;
                    hataa = false;
                    IMarkupConverter markupConverter = new MarkupConverter.MarkupConverter();
                    if (button_no == 1)
                    {
                        bool htmlkontrol = (gelen_kutusu[dizi].alınan_mail_icerik != HttpUtility.HtmlEncode(gelen_kutusu[dizi].alınan_mail_icerik));
                        tut = gelen_kutusu[dizi].alınan_mail_icerik;
                        if (checkBox1.Checked != true)
                        {
                            if (htmlkontrol == true)
                            {
                                string text;
                                string convert;
                                foreach (var x in gelen_kutusu_bodyfile)
                                {
                                    try
                                    {
                                        int kontrol = tut.IndexOf("< img src", metinbaslangicIndex);
                                        if (kontrol == -1)
                                            kontrol = tut.IndexOf("<img src", metinbaslangicIndex);
                                        if (kontrol == -1)
                                            hataa = true;
                                        if (hataa == false)
                                        {
                                            if (x.alınan_mail_no == gelen_kutusu[dizi].id)
                                            {
                                                int deger1 = tut.IndexOf(">", kontrol) + 1;
                                                text = tut.Substring(metinbaslangicIndex, kontrol - metinbaslangicIndex);
                                                try
                                                {
                                                    convert = markupConverter.ConvertHtmlToRtf(text);
                                                    richTextBox1.SelectedRtf = convert;
                                                }
                                                catch (Exception)
                                                {
                                                    try
                                                    {
                                                        convert = ConvertHtmlToText(text);
                                                        richTextBox1.SelectedText = convert;
                                                    }
                                                    catch (Exception)
                                                    {
                                                        richTextBox1.SelectedText = text;
                                                    }
                                                }
                                                metinbaslangicIndex = deger1;

                                                StringBuilder ab = new StringBuilder();

                                                _graphics = richTextBox1.CreateGraphics();
                                                int picw = (int)Math.Round((x.width / _graphics.DpiX) * 2540);
                                                int pich = (int)Math.Round((x.height / _graphics.DpiY) * 2540);
                                                int picwgoal = (int)Math.Round((x.width / _graphics.DpiX) * 1440);
                                                int pichgoal = (int)Math.Round((x.height / _graphics.DpiY) * 1440);

                                                int rictextwgoal = (int)Math.Round((richTextBox1.Width / _graphics.DpiX) * 1440);
                                                if (picwgoal >= rictextwgoal)
                                                {
                                                    picwgoal = rictextwgoal - 700;
                                                }
                                                string imagetortf = BitConverter.ToString(x.alınan_mail_bodyfile, 0).Replace("-", string.Empty);

                                                ab.Append(@"{\rtf1{\pict\pngblip");
                                                ab.Append(@"\picw" + picw);
                                                ab.Append(@"\pich" + pich);
                                                ab.Append(@"\picwgoal" + picwgoal);
                                                ab.Append(@"\pichgoal" + pichgoal);
                                                ab.Append(@"\hex ");
                                                ab.Append(imagetortf + @"}\v image");
                                                ab.Append(@"}\par}");
                                                richTextBox1.SelectedRtf = ab.ToString();
                                                ab.Clear();
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        hataa = true;
                                    }
                                }
                                if (hataa == true)
                                {
                                    text = tut.Substring(metinbaslangicIndex, tut.Length - metinbaslangicIndex);
                                    try
                                    {
                                        convert = markupConverter.ConvertHtmlToRtf(text);
                                        richTextBox1.SelectedRtf = convert;
                                    }
                                    catch (Exception)
                                    {
                                        try
                                        {
                                            richTextBox1.SelectedText = ConvertHtmlToText(text);
                                        }
                                        catch (Exception)
                                        {
                                            richTextBox1.SelectedText = text;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                richTextBox1.Text = tut;
                            }
                        }
                        else
                        {
                            if (htmlkontrol == true)
                            {
                                try
                                {
                                    richTextBox1.Text = ConvertHtmlToText(tut);
                                }
                                catch (Exception)
                                {
                                    richTextBox1.Text = tut;
                                }
                            }
                            else
                                richTextBox1.Text = tut;
                        }


                        //buttonla okuduğumuz mailde attachment varmı, varsa picturebox rengi değişçek
                        listBox1.Items.Clear();
                        if (gelen_kutusu_dosya.Count > 0)
                        {
                            foreach (var y in gelen_kutusu_dosya)
                            {
                                if (y.alınan_mail_no == gelen_kutusu[dizi].id)
                                {
                                    listBox1.Items.Add(y.attachment_name);
                                    pictureBox3.BackColor = Color.Green;
                                }
                            }
                        }
                        textBox2.Text = gelen_kutusu[dizi].alınan_mail_konu;
                        textBox3.Text = $"{gelen_kutusu[dizi].yollayan_kisi}    --   {gelen_kutusu[dizi].mail_alma_tarhi}";
                    }
                    else if (button_no == 2)
                    {
                        bool htmlkontrol = (giden_kutusu[dizi].gonderilen_mail_icerik != HttpUtility.HtmlEncode(giden_kutusu[dizi].gonderilen_mail_icerik));
                        tut = giden_kutusu[dizi].gonderilen_mail_icerik;
                        if (checkBox1.Checked != true)
                        {
                            if (htmlkontrol == true)
                            {
                                string text;
                                string convert;
                                foreach (var x in giden_kutusu_bodyfile)
                                {
                                    try
                                    {
                                        int kontrol = tut.IndexOf("< img src", metinbaslangicIndex);
                                        if (kontrol == -1)
                                            kontrol = tut.IndexOf("<img src", metinbaslangicIndex);
                                        if (kontrol == -1)
                                            hataa = true;
                                        if (hataa == false)
                                        {
                                            if (x.gonderilen_mail_no == giden_kutusu[dizi].id)
                                            {
                                                int deger1 = tut.IndexOf(">", kontrol) + 1;
                                                text = tut.Substring(metinbaslangicIndex, kontrol - metinbaslangicIndex);
                                                try
                                                {
                                                    convert = markupConverter.ConvertHtmlToRtf(text);
                                                    richTextBox1.SelectedRtf = convert;
                                                }
                                                catch (Exception)
                                                {
                                                    try
                                                    {
                                                        convert = ConvertHtmlToText(text);
                                                        richTextBox1.SelectedText = convert;
                                                    }
                                                    catch (Exception)
                                                    {
                                                        richTextBox1.SelectedText = text;
                                                    }
                                                }
                                                metinbaslangicIndex = deger1;

                                                StringBuilder ab = new StringBuilder();

                                                _graphics = richTextBox1.CreateGraphics();
                                                int picw = (int)Math.Round((x.width / _graphics.DpiX) * 2540);
                                                int pich = (int)Math.Round((x.height / _graphics.DpiY) * 2540);
                                                int picwgoal = (int)Math.Round((x.width / _graphics.DpiX) * 1440);
                                                int pichgoal = (int)Math.Round((x.height / _graphics.DpiY) * 1440);

                                                int rictextwgoal = (int)Math.Round((richTextBox1.Width / _graphics.DpiX) * 1440);
                                                if (picwgoal >= rictextwgoal)
                                                {
                                                    picwgoal = rictextwgoal - 700;
                                                }
                                                string imagetortf = BitConverter.ToString(x.gonderilen_mail_bodyfile, 0).Replace("-", string.Empty);
                                                ab.Append(@"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fnil\fcharset0 Microsoft Sans Serif;}}
\viewkind4\uc1\pard\f0\fs17");
                                                ab.Append(@"{\pict\pngblip");
                                                ab.Append(@"\picw" + picw);
                                                ab.Append(@"\pich" + pich);
                                                ab.Append(@"\picwgoal" + picwgoal);
                                                ab.Append(@"\pichgoal" + pichgoal);
                                                ab.Append(@"\hex ");
                                                ab.Append(imagetortf + @"}\v image");
                                                ab.Append(@"}\par}");
                                                richTextBox1.SelectedRtf = ab.ToString();
                                                ab.Clear();
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        hataa = true;
                                    }
                                }

                                text = tut.Substring(metinbaslangicIndex, tut.Length - metinbaslangicIndex);
                                try
                                {
                                    convert = markupConverter.ConvertHtmlToRtf(text);
                                    richTextBox1.SelectedRtf = convert;
                                }
                                catch (Exception)
                                {
                                    try
                                    {
                                        richTextBox1.SelectedText = ConvertHtmlToText(text);
                                    }
                                    catch (Exception)
                                    {
                                        richTextBox1.SelectedText = text;
                                    }
                                }

                            }
                            else
                            {
                                richTextBox1.Text = tut;
                            }
                        }
                        else
                        {
                            if (htmlkontrol == true)
                            {
                                try
                                {
                                    richTextBox1.Text = ConvertHtmlToText(tut);
                                }
                                catch (Exception)
                                {
                                    richTextBox1.Text = tut;
                                }
                            }
                            else
                                richTextBox1.Text = tut;
                        }

                        //buttonla okuduğumuz mailde attachment varmı, varsa picturebox rengi değişçek
                        listBox1.Items.Clear();
                        if (giden_kutusu_dosya.Count > 0)
                        {
                            foreach (var y in giden_kutusu_dosya)
                            {
                                if (y.gonderilen_mail_no == giden_kutusu[dizi].id)
                                {
                                    pictureBox3.BackColor = Color.Green;
                                    listBox1.Items.Add(y.attachment_name);
                                }
                            }
                        }
                        textBox2.Text = giden_kutusu[dizi].gonderilen_mail_konu;
                        textBox3.Text = $"{login_user.Instance.Eposta}    --   {giden_kutusu[dizi].mail_yollama_tarhi}";
                    }
                    else if (button_no == 3)
                    {
                        bool htmlkontrol = (trash_kutusu[dizi].alınan_mail_icerik != HttpUtility.HtmlEncode(trash_kutusu[dizi].alınan_mail_icerik));
                        tut = trash_kutusu[dizi].alınan_mail_icerik;
                        if (checkBox1.Checked != true)
                        {
                            if (htmlkontrol == true)
                            {
                                string text;
                                string convert;
                                foreach (var x in trash_kutusu_bodyfile)
                                {
                                    try
                                    {
                                        int kontrol = tut.IndexOf("< img src", metinbaslangicIndex);
                                        if (kontrol == -1)
                                            kontrol = tut.IndexOf("<img src", metinbaslangicIndex);
                                        if (kontrol == -1)
                                            hataa = true;
                                        if (hataa == false)
                                        {
                                            if (x.alınan_mail_no == trash_kutusu[dizi].id)
                                            {
                                                int deger1 = tut.IndexOf(">", kontrol) + 1;
                                                text = tut.Substring(metinbaslangicIndex, kontrol - metinbaslangicIndex);
                                                try
                                                {
                                                    convert = markupConverter.ConvertHtmlToRtf(text);
                                                    richTextBox1.SelectedRtf = convert;
                                                }
                                                catch (Exception)
                                                {
                                                    try
                                                    {
                                                        convert = ConvertHtmlToText(text);
                                                        richTextBox1.SelectedText = convert;
                                                    }
                                                    catch (Exception)
                                                    {
                                                        richTextBox1.SelectedText = text;
                                                    }
                                                }
                                                metinbaslangicIndex = deger1;

                                                StringBuilder ab = new StringBuilder();

                                                _graphics = richTextBox1.CreateGraphics();
                                                int picw = (int)Math.Round((x.width / _graphics.DpiX) * 2540);
                                                int pich = (int)Math.Round((x.height / _graphics.DpiY) * 2540);
                                                int picwgoal = (int)Math.Round((x.width / _graphics.DpiX) * 1440);
                                                int pichgoal = (int)Math.Round((x.height / _graphics.DpiY) * 1440);

                                                int rictextwgoal = (int)Math.Round((richTextBox1.Width / _graphics.DpiX) * 1440);
                                                if (picwgoal >= rictextwgoal)
                                                {
                                                    picwgoal = rictextwgoal - 700;
                                                }
                                                string imagetortf = BitConverter.ToString(x.alınan_mail_bodyfile, 0).Replace("-", string.Empty);

                                                ab.Append(@"{\rtf1{\pict\pngblip");
                                                ab.Append(@"\picw" + picw);
                                                ab.Append(@"\pich" + pich);
                                                ab.Append(@"\picwgoal" + picwgoal);
                                                ab.Append(@"\pichgoal" + pichgoal);
                                                ab.Append(@"\hex ");
                                                ab.Append(imagetortf + @"}\v image");
                                                ab.Append(@"}\par}");
                                                richTextBox1.SelectedRtf = ab.ToString();
                                                ab.Clear();
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        hataa = true;
                                    }
                                }
                                if (hataa == true)
                                {
                                    text = tut.Substring(metinbaslangicIndex, tut.Length - metinbaslangicIndex);
                                    try
                                    {
                                        convert = markupConverter.ConvertHtmlToRtf(text);
                                        richTextBox1.SelectedRtf = convert;
                                    }
                                    catch (Exception)
                                    {
                                        try
                                        {
                                            richTextBox1.SelectedText = ConvertHtmlToText(text);
                                        }
                                        catch (Exception)
                                        {
                                            richTextBox1.SelectedText = text;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                richTextBox1.Text = tut;
                            }
                        }
                        else
                        {
                            if (htmlkontrol == true)
                            {
                                try
                                {
                                    richTextBox1.Text = ConvertHtmlToText(tut);
                                }
                                catch (Exception)
                                {
                                    richTextBox1.Text = tut;
                                }
                            }
                            else
                                richTextBox1.Text = tut;
                        }


                        //buttonla okuduğumuz mailde attachment varmı, varsa picturebox rengi değişçek
                        listBox1.Items.Clear();
                        if (trash_kutusu_dosya.Count > 0)
                        {
                            foreach (var y in trash_kutusu_dosya)
                            {
                                if (y.alınan_mail_no == trash_kutusu[dizi].id)
                                {
                                    pictureBox3.BackColor = Color.Green;
                                    listBox1.Items.Add(y.attachment_name);
                                }
                            }
                        }
                        textBox2.Text = trash_kutusu[dizi].alınan_mail_konu;
                        textBox3.Text = $"{trash_kutusu[dizi].yollayan_kisi}    --   {trash_kutusu[dizi].mail_alma_tarhi}";
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        int toplam_mesaj_giden;
        int toplam_mesaj_gelen;
        int toplam_mesaj_trash;
        private void button3_Click(object sender, EventArgs evnt)
        {
            if (listBox2.Items.Count > 0)
            {
                if (listBox2.SelectedValue != null)
                {
                    if (NetworkInterface.GetIsNetworkAvailable() == true)
                    {
                        if (backgroundWorker1.IsBusy != true)
                        {
                            try
                            {
                                using (var client = new ImapClient())
                                {
                                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                                    client.Connect("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);
                                    client.Authenticate(login_user.Instance.Eposta, login_user.Instance.sifre);

                                    if (button_no == 1)
                                    {
                                        var inbox = client.Inbox;
                                        inbox.Open(FolderAccess.ReadWrite);
                                        IList<UniqueId> uids = inbox.Search(SearchQuery.All);
                                        var item = (mail_get_user)listBox2.SelectedValue;
                                        for (int i = 0; i < gelen_kutusu.Count; i++)
                                        {
                                            if (item.id == gelen_kutusu[i].id)
                                            {
                                                int u = gelen_kutusu.Count - 1 - i;
                                                var matchFolder = client.GetFolder(SpecialFolder.Trash);
                                                if (matchFolder != null)
                                                    inbox.MoveTo(uids[u], matchFolder);
                                                inbox.Expunge();
                                                inbox.Close();
                                                listBox2.DataSource = null;
                                                inboxService.CopeTasi(item.id, gelen_kutusu, gelen_kutusu_dosya, gelen_kutusu_bodyfile);        //silmek yerine databasede silinenlere ekle dicez
                                                gelen_kutusu.Remove(item);
                                                listBox2.DataSource = gelen_kutusu;
                                                listBox2.DisplayMember = "tam_deger";
                                                label2.Text = $"Total: {gelen_kutusu.Count}";
                                                client.Disconnect(true);
                                            }
                                        }
                                    }

                                    else if (button_no == 2)
                                    {
                                        var inbox = client.GetFolder(SpecialFolder.Sent);
                                        inbox.Open(FolderAccess.ReadWrite);
                                        IList<UniqueId> uids = inbox.Search(SearchQuery.All);
                                        var item = (mail_send_user)listBox2.SelectedValue;
                                        for (int i = 0; i < giden_kutusu.Count - 1; i++)
                                        {
                                            if (item.id == giden_kutusu[i].id)
                                            {
                                                int u = giden_kutusu.Count - 1 - i;
                                                var matchFolder = client.GetFolder(SpecialFolder.Trash);
                                                if (matchFolder != null)
                                                    inbox.MoveTo(uids[u], matchFolder);
                                                inbox.Expunge();
                                                inbox.Close();
                                                listBox2.DataSource = null;
                                                sentService.CopeTasi(item.id, giden_kutusu, giden_kutusu_dosya, giden_kutusu_bodyfile);
                                                giden_kutusu.Remove(item);
                                                listBox2.DataSource = giden_kutusu;
                                                listBox2.DisplayMember = "tam_deger";
                                                label2.Text = $"Total: {giden_kutusu.Count}";
                                                client.Disconnect(true);
                                            }
                                        }
                                    }

                                    else if (button_no == 3)
                                    {
                                        var inbox = client.GetFolder(SpecialFolder.Trash);
                                        inbox.Open(FolderAccess.ReadWrite);
                                        IList<UniqueId> uids = inbox.Search(SearchQuery.All);
                                        var item = (trash_get_user)listBox2.SelectedValue;
                                        for (int i = 0; i < trash_kutusu.Count - 1; i++)
                                        {
                                            if (item.id == trash_kutusu[i].id)
                                            {
                                                int u = trash_kutusu.Count - 1 - i;
                                                inbox.AddFlags(uids[u], MessageFlags.Deleted, true);
                                                inbox.Expunge();
                                                inbox.Close();
                                                listBox2.DataSource = null;
                                                trashService.KaliciSil(item.id);
                                                trash_kutusu.Remove(item);
                                                listBox2.DataSource = trash_kutusu;
                                                listBox2.DisplayMember = "tam_deger";
                                                label2.Text = $"Total: {trash_kutusu.Count}";
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception) { }
                        }
                    }
                    else
                    {
                        if (button_no == 1)
                        {
                            var item = (mail_get_user)listBox2.SelectedValue;
                            listBox2.DataSource = null;
                            inboxService.CopeTasi(item.id, gelen_kutusu, gelen_kutusu_dosya, gelen_kutusu_bodyfile);
                            gelen_kutusu.Remove(item);
                            listBox2.DataSource = gelen_kutusu;
                            listBox2.DisplayMember = "tam_deger";
                            label2.Text = $"Total: {gelen_kutusu.Count}";
                        }
                        else if (button_no == 2)
                        {
                            var item = (mail_send_user)listBox2.SelectedValue;
                            listBox2.DataSource = null;
                            sentService.CopeTasi(item.id, giden_kutusu, giden_kutusu_dosya, giden_kutusu_bodyfile);
                            giden_kutusu.Remove(item);
                            listBox2.DataSource = giden_kutusu;
                            listBox2.DisplayMember = "tam_deger";
                            label2.Text = $"Total: {giden_kutusu.Count}";
                        }
                        else if (button_no == 3)
                        {
                            var item = (trash_get_user)listBox2.SelectedValue;
                            listBox2.DataSource = null;
                            trashService.KaliciSil(item.id);
                            trash_kutusu.Remove(item);
                            listBox2.DataSource = trash_kutusu;
                            listBox2.DisplayMember = "tam_deger";
                            label2.Text = $"Total: {trash_kutusu.Count}";

                        }
                    }
                    trash_kutusu.Clear();
                    trash_kutusu_bodyfile.Clear();
                    trash_kutusu_dosya.Clear();

                    trash_kutusu = trashService.GetMailler();
                    trash_kutusu_bodyfile = trashService.GetBodyfiles();
                    trash_kutusu_dosya = trashService.GetEklentiler();
                }
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Çıkış Yapmak İstediğinize Emin misiniz?", "Çıkış", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                cıkıs = true;   //diablog içinde çıkış yapınca hata veriyor(sanırsam dialogu kapatmaya çalışıyor.)
            }
            if (cıkıs == true)
                Environment.Exit(1);
        }
        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }



        ///////////////////////////////////////////////////////////////////////////////
        List<mail_tut> mail_tut = new List<mail_tut>();
        List<mail_bodyfile_tut> mail_bodyfile_tut = new List<mail_bodyfile_tut>();
        List<mail_attachment_tut> mail_attachment_tut = new List<mail_attachment_tut>();

        int toplam_mesaj;

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs evnt)
        {
            button3.BorderColor = Color.Red;
            button10.BorderColor = Color.Red;
            using (var client = new ImapClient())
            {
                backgroundWorker1.ReportProgress(0);
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);

                client.Authenticate(login_user.Instance.Eposta, login_user.Instance.sifre);
                IMailFolder inbox = null;
                backgroundWorker1.ReportProgress(10);
                for (int i = 1; i < 4; i++)
                {
                    //inbox.close() gereksiz - sunucu otomatik olarak eski klasörü kapatıyor. https://stackoverflow.com/questions/29490638/mailkit-imailfolder-close-throws-exception  - jstedfast 12/06/2021 18:15
                    if (i == 1)
                    {
                        inbox = client.Inbox;
                        get_mails(inbox, client);
                        inboxService.Senkronize(toplam_mesaj, mail_tut, mail_bodyfile_tut, mail_attachment_tut);
                        gelen_kutusu.Clear();
                        gelen_kutusu_bodyfile.Clear();
                        gelen_kutusu_dosya.Clear();
                        gelen_kutusu = inboxService.GetMailler();
                        backgroundWorker1.ReportProgress(20);
                        gelen_kutusu_bodyfile = inboxService.GetBodyfiles();
                        backgroundWorker1.ReportProgress(30);
                        gelen_kutusu_dosya = inboxService.GetEklentiler();
                        backgroundWorker1.ReportProgress(40);
                    }
                    if (i == 2)
                    {
                        inbox = client.GetFolder(SpecialFolder.Sent);
                        get_mails(inbox, client);
                        sentService.Senkronize(toplam_mesaj, mail_tut, mail_bodyfile_tut, mail_attachment_tut);
                        giden_kutusu.Clear();
                        giden_kutusu_bodyfile.Clear();
                        giden_kutusu_dosya.Clear();
                        giden_kutusu = sentService.GetMailler();
                        backgroundWorker1.ReportProgress(50);
                        giden_kutusu_bodyfile = sentService.GetBodyfiles();
                        backgroundWorker1.ReportProgress(60);
                        giden_kutusu_dosya = sentService.GetEklentiler();
                        backgroundWorker1.ReportProgress(70);
                    }
                    if (i == 3)
                    {
                        inbox = client.GetFolder(SpecialFolder.Trash);
                        get_mails(inbox, client);
                        trashService.Senkronize(toplam_mesaj, mail_tut, mail_bodyfile_tut, mail_attachment_tut);
                        trash_kutusu.Clear();
                        trash_kutusu_bodyfile.Clear();
                        trash_kutusu_dosya.Clear();
                        trash_kutusu = trashService.GetMailler();
                        backgroundWorker1.ReportProgress(80);
                        trash_kutusu_bodyfile = trashService.GetBodyfiles();
                        backgroundWorker1.ReportProgress(90);
                        trash_kutusu_dosya = trashService.GetEklentiler();
                        backgroundWorker1.ReportProgress(100);
                    }
                }
                client.Disconnect(true);
            }
        }
        public void get_mails(IMailFolder folder, ImapClient client)
        {
            try
            {
                mail_tut.Clear();
                mail_bodyfile_tut.Clear();
                mail_attachment_tut.Clear();
                int id = 1;
                folder.Open(FolderAccess.ReadOnly);
                toplam_mesaj = folder.Count;
                if (folder == client.GetFolder(SpecialFolder.Sent))
                    toplam_mesaj_giden = toplam_mesaj;
                if (folder == client.Inbox)
                    toplam_mesaj_gelen = toplam_mesaj;
                if (folder == client.GetFolder(SpecialFolder.Trash))
                    toplam_mesaj_trash = toplam_mesaj;
                string text;
                IList<UniqueId> uids = folder.Search(SearchQuery.All);
                foreach (UniqueId uid in uids)
                {
                    MimeMessage message = folder.GetMessage(uid);
                    if (message.HtmlBody != null)
                    {
                        text = message.HtmlBody;
                        string tut = text.Replace(" ", "");  //değer alacağımızdan boşlukların bir önemi yok
                        foreach (MimePart att in message.BodyParts)
                        {
                            if (att.ContentId != null && att.Content != null && att.ContentType.MediaType == "image" && (text.IndexOf("cid:" + att.ContentId) > -1))
                            {
                                byte[] b;
                                using (var mem = new MemoryStream())
                                {
                                    att.Content.DecodeTo(mem);
                                    b = mem.ToArray();
                                }
                                int resim_width = 0, resim_height = 0;
                                bool hata_tut = false;
                                int deger1 = -1;
                                try
                                {
                                    deger1 = tut.IndexOf(@"<imgsrc=""cid:");
                                    int deger3 = tut.IndexOf("width=", deger1) + 6;
                                    int deger4 = tut.IndexOf("height=", deger3);
                                    resim_width = Convert.ToInt32(tut.Substring(deger3, deger4 - deger3).Replace(@"""", ""));

                                    int deger5 = tut.IndexOf(@"height=", deger1) + 7;
                                    int deger6 = tut.IndexOf(">", deger5);
                                    resim_height = Convert.ToInt32(tut.Substring(deger5, deger6 - deger5).Replace(@"""", ""));
                                }
                                catch (Exception)
                                {
                                    hata_tut = true;
                                }
                                if (deger1 != -1)
                                {
                                    if (hata_tut == false)
                                    {
                                        mail_bodyfile_tut.Add(new mail_bodyfile_tut
                                        {
                                            id = id,
                                            alınan_mail_bodyfile = b,
                                            width = resim_width,
                                            height = resim_height
                                        });
                                    }
                                    int deger7 = tut.IndexOf(@"height=", deger1);
                                    int deger8 = tut.IndexOf(@">", deger7) + 1;
                                    string html_imagecode = tut.Substring(deger1, deger8 - deger1);
                                    tut = tut.Replace(html_imagecode, string.Empty);
                                }
                                else
                                    hataa = true;
                            }
                        }
                    }

                    else if (message.TextBody != null)
                        text = message.TextBody;
                    else
                        text = string.Empty;
                    mail_tut.Add(new mail_tut
                    {
                        id = id,
                        baslik = message.Subject,
                        icerik = text,
                        tarih = (DateTimeOffset)message.Date,
                        yollayan = message.From.ToString(),
                    });
                    if (message.Attachments != null)
                    {
                        foreach (var attachment in message.Attachments)
                        {
                            using (var stream = new MemoryStream())
                            {
                                var part = (MimePart)attachment;

                                part.Content.DecodeTo(stream);

                                byte[] byt = stream.ToArray();
                                mail_attachment_tut.Add(new mail_attachment_tut
                                {
                                    alınan_mail_attachment = byt,
                                    id = id,
                                    attachment_name = part.FileName
                                });
                            }

                        }
                    }
                    id++;
                }
            }
            catch (Exception) { }

        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label1.Text = "Mailler Güncellendi !!";
            if (button_no == 1)
            {
                label2.Text = $"Total: {toplam_mesaj_gelen}";
                listBox2.DataSource = null;
                listBox2.DataSource = gelen_kutusu;
                listBox2.DisplayMember = "tam_deger";
            }
            else if (button_no == 2)
            {
                label2.Text = $"Total: {toplam_mesaj_giden}";
                listBox2.DataSource = null;
                listBox2.DataSource = giden_kutusu;
                listBox2.DisplayMember = "tam_deger";
            }
            else if (button_no == 3)
            {
                label2.Text = $"Total: {toplam_mesaj_trash}";
                listBox2.DataSource = null;
                listBox2.DataSource = trash_kutusu;
                listBox2.DisplayMember = "tam_deger";
            }
            basıldımı = false;
            button6.BorderColor = Color.Black;
            button3.BorderColor = Color.Black;
            button10.BorderColor = Color.Black;
        }




        bool okundumu = false;
        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                if (okundumu == true)
                {
                    if (button_no == 1)
                    {
                        if (listBox2.SelectedItems.Count > 0)
                        {
                            foreach (var x in gelen_kutusu_dosya)
                            {
                                if (gelen_kutusu[dizi].id == x.alınan_mail_no)
                                {
                                    SaveFileDialog file = new SaveFileDialog();
                                    file.FileName = x.attachment_name;
                                    if (file.FileName != "" && file.ShowDialog() == DialogResult.OK)
                                    {
                                        string dosya_byte = BitConverter.ToString(x.alınan_mail_dosyalar, 0).Replace("-", string.Empty);
                                        File.WriteAllBytes(file.FileName, x.alınan_mail_dosyalar);
                                    }
                                }
                            }
                        }
                    }
                    else if (button_no == 2)
                    {
                        if (listBox2.SelectedItems.Count > 0)
                        {
                            foreach (var x in giden_kutusu_dosya)
                            {
                                if (giden_kutusu[dizi].id == x.gonderilen_mail_no)
                                {
                                    SaveFileDialog file = new SaveFileDialog();
                                    file.FileName = x.attachment_name;

                                    if (file.FileName != "" && file.ShowDialog() == DialogResult.OK)
                                    {
                                        string dosya_byte = BitConverter.ToString(x.gonderilen_mail_dosyalar, 0).Replace("-", string.Empty);
                                        File.WriteAllBytes(file.FileName, x.gonderilen_mail_dosyalar);
                                    }
                                }
                            }
                        }

                    }
                    else if (button_no == 3)
                    {
                        if (listBox2.SelectedItems.Count > 0)
                        {
                            foreach (var x in trash_kutusu_dosya)
                            {
                                if (trash_kutusu[dizi].id == x.alınan_mail_no)
                                {
                                    SaveFileDialog file = new SaveFileDialog();
                                    file.FileName = x.attachment_name;
                                    if (file.FileName != "" && file.ShowDialog() == DialogResult.OK)
                                    {
                                        string dosya_byte = BitConverter.ToString(x.alınan_mail_dosyalar, 0).Replace("-", string.Empty);
                                        File.WriteAllBytes(file.FileName, x.alınan_mail_dosyalar);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        bool basıldımı = false;

        private void button6_Click(object sender, EventArgs e)
        {
            if (basıldımı == false)
            {
                if (NetworkInterface.GetIsNetworkAvailable() == true)
                {
                    if (backgroundWorker1.IsBusy == false)
                    {
                        basıldımı = true;
                        label1.Text = "Mailler güncelleniyor...";
                        button6.BorderColor = Color.DarkGreen;
                        backgroundWorker1.RunWorkerAsync();
                    }
                }
                else
                {
                    label1.Text = "Mailler güncelleniyor  -  0%";
                    gelen_kutusu.Clear();
                    gelen_kutusu_bodyfile.Clear();
                    gelen_kutusu_dosya.Clear();
                    giden_kutusu.Clear();
                    giden_kutusu_bodyfile.Clear();
                    giden_kutusu_dosya.Clear();
                    trash_kutusu.Clear();
                    trash_kutusu_bodyfile.Clear();
                    trash_kutusu_dosya.Clear();
                    button6.BorderColor = Color.DarkGreen;
                    gelen_kutusu = inboxService.GetMailler();
                    gelen_kutusu_bodyfile = inboxService.GetBodyfiles();
                    gelen_kutusu_dosya = inboxService.GetEklentiler();
                    label1.Text = "Mailler güncelleniyor  -   35%";
                    giden_kutusu = sentService.GetMailler();
                    giden_kutusu_bodyfile = sentService.GetBodyfiles();
                    giden_kutusu_dosya = sentService.GetEklentiler();
                    label1.Text = "Mailler güncelleniyor  -   75%";
                    trash_kutusu = trashService.GetMailler();
                    trash_kutusu_bodyfile = trashService.GetBodyfiles();
                    trash_kutusu_dosya = trashService.GetEklentiler();
                    label1.Text = "Mailler güncelleniyor  -   100%";
                    button6.BorderColor = Color.Black;
                    label1.Text = "Mailler Güncellendi !!";
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            button_no = 3;
            label3.Text = "Çöp Kutusu:";
            label2.Text = $"Total: {trash_kutusu.Count}";
            listBox2.DataSource = null;
            listBox2.DataSource = trash_kutusu;
            listBox2.DisplayMember = "tam_deger";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            button_no = 2;
            label3.Text = "Giden Kutusu:";
            label2.Text = $"Total: {giden_kutusu.Count}";
            listBox2.DataSource = null;
            listBox2.DataSource = giden_kutusu;
            listBox2.DisplayMember = "tam_deger";
        }

        private void button10_Click(object sender, EventArgs evnt)
        {
            if (listBox2.Items.Count > 0)
            {
                if (listBox2.SelectedValue != null)
                {
                    try
                    {
                        if (NetworkInterface.GetIsNetworkAvailable() == true)
                        {
                            if (backgroundWorker1.IsBusy != true)
                            {
                                if (button_no == 3)
                                {
                                    using (var client = new ImapClient())
                                    {
                                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                                        client.Connect("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);
                                        client.Authenticate(login_user.Instance.Eposta, login_user.Instance.sifre);
                                        var inbox = client.GetFolder(SpecialFolder.Trash);
                                        inbox.Open(FolderAccess.ReadWrite);
                                        IList<UniqueId> uids = inbox.Search(SearchQuery.All);
                                        var item = (trash_get_user)listBox2.SelectedValue;
                                        for (int i = 0; i < trash_kutusu.Count; i++)
                                        {
                                            if (item.id == trash_kutusu[i].id)
                                            {
                                                int u = trash_kutusu.Count - 1 - i;
                                                if (item.yollayan_kisi == login_user.Instance.Eposta)
                                                {
                                                    var matchFolder = client.GetFolder(SpecialFolder.Sent);
                                                    if (matchFolder != null)
                                                        inbox.MoveTo(uids[u], matchFolder);
                                                    sentService.GeriYukle(item.id, trash_kutusu, trash_kutusu_dosya, trash_kutusu_bodyfile);
                                                }
                                                else
                                                {
                                                    var matchFolder = client.Inbox;
                                                    if (matchFolder != null)
                                                        inbox.MoveTo(uids[u], matchFolder);
                                                    inboxService.GeriYukle(item.id, trash_kutusu, trash_kutusu_dosya, trash_kutusu_bodyfile);
                                                }
                                                inbox.Expunge();
                                                inbox.Close();
                                                listBox2.DataSource = null;
                                                trashService.KaliciSil(item.id);
                                                trash_kutusu.Remove(item);
                                                listBox2.DataSource = trash_kutusu;
                                                listBox2.DisplayMember = "tam_deger";
                                                label2.Text = $"Total: {trash_kutusu.Count}";
                                            }

                                        }
                                    }
                                }
                            }

                        }
                        else
                        {
                            if (button_no == 3)
                            {
                                var item = (trash_get_user)listBox2.SelectedValue;
                                if (item.yollayan_kisi == login_user.Instance.Eposta)
                                {
                                    sentService.GeriYukle(item.id, trash_kutusu, trash_kutusu_dosya, trash_kutusu_bodyfile);
                                }
                                else
                                {
                                    inboxService.GeriYukle(item.id, trash_kutusu, trash_kutusu_dosya, trash_kutusu_bodyfile);
                                }
                                listBox2.DataSource = null;
                                trashService.KaliciSil(item.id);
                                trash_kutusu.Remove(item);
                                listBox2.DataSource = trash_kutusu;
                                listBox2.DisplayMember = "tam_deger";
                                label2.Text = $"Total: {trash_kutusu.Count}";
                            }
                        }
                    }
                    catch (Exception) { }
                    gelen_kutusu.Clear();
                    gelen_kutusu_bodyfile.Clear();
                    gelen_kutusu_dosya.Clear();
                    giden_kutusu.Clear();
                    giden_kutusu_bodyfile.Clear();
                    giden_kutusu_dosya.Clear();
                    trash_kutusu.Clear();
                    trash_kutusu_bodyfile.Clear();
                    trash_kutusu_dosya.Clear();
                    gelen_kutusu = inboxService.GetMailler();
                    gelen_kutusu_bodyfile = inboxService.GetBodyfiles();
                    gelen_kutusu_dosya = inboxService.GetEklentiler();
                    giden_kutusu = sentService.GetMailler();
                    giden_kutusu_bodyfile = sentService.GetBodyfiles();
                    giden_kutusu_dosya = sentService.GetEklentiler();
                    trash_kutusu = trashService.GetMailler();
                    trash_kutusu_bodyfile = trashService.GetBodyfiles();
                    trash_kutusu_dosya = trashService.GetEklentiler();
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            label1.Text = ("Mailler güncelleniyor  -  " + e.ProgressPercentage.ToString() + "%");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button_no = 1;
            label3.Text = "Gelen Kutusu:";
            label2.Text = $"Total: {gelen_kutusu.Count}";
            listBox2.DataSource = null;
            listBox2.DataSource = gelen_kutusu;
            listBox2.DisplayMember = "tam_deger";
        }

    }
}