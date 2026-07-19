using mail.DTO;
using MailKit.Net.Smtp;
using MailKit.Security;
using MarkupConverter;
using MimeKit;
using MimeKit.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace mail
{
    public partial class SendMail : Form
    {
        public SendMail()
        {
            InitializeComponent();
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

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            fontDialog1.ShowDialog();
            richTextBox1.SelectionFont = fontDialog1.Font;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            richTextBox1.SelectionColor = colorDialog1.Color;
        }

        List<string> attachment_dondur = new List<string>();
        private void Form4_Load(object sender, EventArgs e)
        {
            label3.Text = string.Empty;
            listBox1.HorizontalScrollbar = true;
        }
        private void button5_Click(object sender, EventArgs e)  //attachment ekleme
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Multiselect = true;
            if (file.ShowDialog() == DialogResult.OK)
            {
                foreach (string dosya_Ad in file.FileNames)
                {
                    attachment_dondur.Add(dosya_Ad);
                    listBox1.Items.Add(dosya_Ad);
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)  // attachment temizleme
        {
            attachment_dondur.Clear();
            listBox1.Items.Clear();
        }


        string tut, rtf;
        int metinbaslangicIndex = 0;
        int width, height;
        int startIndex;
        int endIndex;
        private void button2_Click(object sender, EventArgs evnt) //gönderme işlemi
        {
            label3.ForeColor = Color.Maroon;
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                metinbaslangicIndex = 0;
                SmtpClient client = new SmtpClient();
                MimeMessage message = new MimeMessage();
                BodyBuilder builder = new BodyBuilder();
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
                client.Authenticate(login_user.Instance.Eposta, login_user.Instance.sifre);
                bool hata_varmı = false;
                bool loop = true;
                rtf = richTextBox1.Rtf;
                while (loop)
                {
                    try
                    {
                        tut = ExtractImgRtf(rtf);
                    }
                    catch (Exception)
                    {
                        hata_varmı = true;
                    }
                    try
                    {
                        bool duplicate_engelleme = false;
                        IMarkupConverter markupConverter = new MarkupConverter.MarkupConverter();
                        if (hata_varmı == false)
                        {
                            for (int i = 0; i < bfile.Count; i++)
                            {
                                if (bfile[i].bodyfile_string == tut && duplicate_engelleme == false)
                                {
                                    duplicate_engelleme = true;
                                    string body = rtf.Substring(metinbaslangicIndex, startIndex - metinbaslangicIndex);
                                    metinbaslangicIndex = (endIndex + 12);
                                    var bodyfile = builder.LinkedResources.Add(bfile[i].file_name, bfile[i].bodyfile_byte);
                                    bodyfile.ContentId = MimeUtils.GenerateMessageId();
                                    //try catch gelcek ve text html yerine textx döncek if error
                                    string html_body = markupConverter.ConvertRtfToHtml(body);
                                    builder.HtmlBody += html_body;
                                    builder.HtmlBody += string.Format(@"<img src=""cid:{0}"" width={1} height={2} > ", bodyfile.ContentId, width, height);
                                }
                            }
                            duplicate_engelleme = false;
                        }
                        else
                        {
                            string body = rtf.Substring(metinbaslangicIndex, richTextBox1.Rtf.Length - metinbaslangicIndex);
                            string html_body = markupConverter.ConvertRtfToHtml(body);
                            builder.HtmlBody += html_body;
                            loop = false;
                        }
                    }
                    catch (Exception)
                    {
                        label3.Text = "Mail yollanırken hata ile karşılaşıldı.";
                        string body = richTextBox1.Text;
                        builder.TextBody = body;
                        loop = false;
                    }
                }
                bool empty = !attachment_dondur.Any();
                if (empty != true)
                {
                    foreach (string yol in attachment_dondur)
                    {
                        builder.Attachments.Add(@"" + yol);
                    }
                }
                bool mesaj_gittimi = true;
                try
                {
                    message.From.Add(MailboxAddress.Parse(login_user.Instance.Eposta));
                    message.To.Add(MailboxAddress.Parse(textBox1.Text));
                    message.Subject = textBox2.Text;
                    message.Body = builder.ToMessageBody();
                    client.Send(message);
                }
                catch (Exception)
                {
                    label3.Text = "Mail yollanırken hata ile karşılaşıldı.";
                    mesaj_gittimi = false;
                }
                finally
                {
                    startIndex = 0;
                    endIndex = 0;
                    metinbaslangicIndex = 0;
                    bfile.Clear();
                    richTextBox1.Clear();
                }
                if (mesaj_gittimi == true)
                {
                    label3.ForeColor = Color.Green;
                    label3.Text = "Mesaj Gönderildi.";
                }
            }
            else
            {
                label3.Text = "Lütfen Her bir bölümü doldurduğunuza emin olunuz...";
            }

        }
        Graphics _graphics;
        List<mail_file_transfer> bfile = new List<mail_file_transfer>();
        private void button6_Click(object sender, EventArgs e)  //richtext box içine resim ekleme işlemleri
        {
            try
            {
                OpenFileDialog file = new OpenFileDialog();
                file.Multiselect = false;
                file.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.png; *.bmp)|*.jpg; *.jpeg; *.gif; *.png; *.bmp";
                if (file.ShowDialog() == DialogResult.OK)
                {
                    MemoryStream stream = new MemoryStream();
                    Image image = Image.FromFile(file.FileName);
                    image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] bytes = stream.ToArray();
                    string imagetortf = BitConverter.ToString(bytes, 0).Replace("-", string.Empty);
                    StringBuilder ab = new StringBuilder();

                    _graphics = richTextBox1.CreateGraphics();
                    int picw = (int)Math.Round((image.Width / _graphics.DpiX) * 2540);
                    int pich = (int)Math.Round((image.Height / _graphics.DpiY) * 2540);
                    int picwgoal = (int)Math.Round((image.Width / _graphics.DpiX) * 1440);
                    int pichgoal = (int)Math.Round((image.Height / _graphics.DpiY) * 1440);

                    int rictextwgoal = (int)Math.Round((richTextBox1.Width / _graphics.DpiX) * 1440);
                    if (picwgoal >= rictextwgoal)
                    {
                        picwgoal = rictextwgoal - 700;
                    }

                    ab.Append(@"{\rtf1{\pict\pngblip");
                    ab.Append(@"\picw" + picw);
                    ab.Append(@"\pich" + pich);
                    ab.Append(@"\picwgoal" + picwgoal);
                    ab.Append(@"\pichgoal" + pichgoal);
                    ab.Append(@"\hex ");
                    ab.Append(imagetortf + @"}\v image");
                    RichTextBox rt = new RichTextBox();
                    rt.Rtf = ab.ToString();
                    richTextBox1.SelectedRtf = ab.ToString();
                    metinbaslangicIndex = 0;
                    string rtf_sadelesmis_byte = ExtractImgRtf(rt.Rtf);  //rtf içinden byte çekince ilk değerinden farklı değer çıkıyor...(o yüzden içinden çekmek zorundayım)

                    bfile.Add(new mail_file_transfer
                    {
                        file_name = file.FileName,
                        bodyfile_string = rtf_sadelesmis_byte,
                        bodyfile_byte = bytes
                    });
                    ab.Clear();
                }
            }
            catch (Exception)
            {
            }
        }


        string ExtractImgRtf(string s)
        {
            startIndex = s.IndexOf("{\\pict", metinbaslangicIndex);

            int widthstartIndex = s.IndexOf("\\picw", startIndex) + 5;
            int widthendIndex = s.IndexOf("\\", widthstartIndex);
            width = (int)Math.Round((Convert.ToInt32(s.Substring(widthstartIndex, widthendIndex - widthstartIndex)) / 1440) * _graphics.DpiX);
            int heightstartIndex = s.IndexOf("\\pich", startIndex) + 5;
            int heightendIndex = s.IndexOf("\\", heightstartIndex);
            height = (int)Math.Round((Convert.ToInt32(s.Substring(heightstartIndex, heightendIndex - heightstartIndex)) / 1440) * _graphics.DpiY);

            int nextIndex1 = s.IndexOf("\\pichgoal", startIndex);
            int nextIndex2 = s.IndexOf(" ", nextIndex1);
            endIndex = s.IndexOf(@"}\v image\v0", nextIndex2);
            return s.Substring(nextIndex2, endIndex - nextIndex2);
        }
    }
}