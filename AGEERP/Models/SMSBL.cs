using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AGEERP.Models
{
    public class SMSBL
    {

        AGEEntities db = new AGEEntities();

        public async Task<string> Send(string number, string msg)
        {
            string resp = "";
            try
            {


                HttpClient client = new HttpClient();
                if (number.Substring(0, 2) == "92")
                {
                    number = "0" + number.Substring(2, number.Length - 2);
                }
                client.BaseAddress = new Uri("https://connect.jazzcmt.com/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("sendsms_url_dual_api.html?rom=AFZAL ELECT&To=" + number + "&Message=" + msg);
                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsStringAsync();
                    resp = res.Substring(0, 12);
                }

                return resp;

            }
            catch (Exception)
            {
                return "Error";
            }
        }
        public bool SendPOEmail(string Subject, long POId)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("no-reply@afzalelectronics.com.pk");
                message.To.Add(new MailAddress("pricing@afzalelectronics.com.pk"));
                message.Subject = Subject;
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = "PFA.";
                smtp.Port = 25;
                smtp.Host = "mail.afzalelectronics.com.pk";
                smtp.EnableSsl = false;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("no-reply@afzalelectronics.com.pk", "Zdxpy2ojhup1");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                WebClient client = new WebClient();
                byte[] bytes = client.DownloadData("http://192.168.77.35/ReportServer?%2fAGEReports%2frptPO&POId=" + POId.ToString() + "&rs%3aFormat=PDF");
                MemoryStream ms = new MemoryStream(bytes);
                message.Attachments.Add(new Attachment(ms, "PO.pdf"));
                smtp.Send(message);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool SendEmail(string toEmail, string subject, string msg)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("no-reply@afzalelectronics.com.pk");
                message.To.Add(new MailAddress(toEmail));
                message.Subject = subject;
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = msg;
                smtp.Port = 25;
                smtp.Host = "mail.afzalelectronics.com.pk";
                smtp.EnableSsl = false;
                smtp.UseDefaultCredentials = false;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}