using OpenPop.Pop3;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading;

namespace EmailSenderDll
{
    public class EmailSender
    {
        public void SenderEmail()
        {
            var emailOrigin = ConfigurationManager.AppSettings["emailOrigin"];
            var emailRecipient = ConfigurationManager.AppSettings["emailRecipient"];
            var message = ConfigurationManager.AppSettings["message"];
            var body = ConfigurationManager.AppSettings["body"];
            var password = ConfigurationManager.AppSettings["password"];
            var host = ConfigurationManager.AppSettings["Host"];

            MailMessage oMailMesagge = new MailMessage("clarityjuandavid@gmail.com", "jxxxx@gmail.com", "To whom it may concern", "This is Juan David, This is my backend software technical test");
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential("clarityjuandavid@gmail.com", "4321abcd");

            try
            {
                int contador = 0;
                bool flag = false;
                do
                {
                    smtpClient.Send(oMailMesagge);
                    int mydelay = 10000;
                    Thread.Sleep(mydelay);
                    Console.WriteLine("Sending....");
                    ConnectToEmail oConnectToEmail = new ConnectToEmail();
                    List<OpenPop.Mime.Message> emails = oConnectToEmail.GetMessages();
                    foreach (var email in emails)
                    {
                        if (email.Headers.Subject == "Delivery Status Notification (Failure)")
                        {
                            contador++;
                            flag = false;
                            if (contador == 3)
                            {
                                Console.WriteLine("Unable to send the email at the third attemp. Please verify the  recipient email address");
                                flag = true;
                            }
                        }
                        if (email.Headers.Subject != "Delivery Status Notification (Failure)")
                        {
                            flag = false;
                        }
                    }
                    if (emails.Count.Equals(0))
                    {
                        Console.WriteLine("Successfully sent");
                        flag = true;
                    }

                } while (!flag);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
    public class ConnectToEmail
    {
        public string emailOrigin = ConfigurationManager.AppSettings["emailOrigin"];
        public string password = ConfigurationManager.AppSettings["password"];
        public int port = 995;
        public bool useSSL = true;
        public string Hostname = "pop.gmail.com";

        public List<OpenPop.Mime.Message> GetMessages()
        {
            using (Pop3Client oClient = new Pop3Client())
            {
                oClient.Connect(Hostname, port, useSSL);
                oClient.Authenticate("clarityjuandavid@gmail.com", "4321abcd");
                int messageCount = oClient.GetMessageCount();
                List<OpenPop.Mime.Message> lstMessages = new List<OpenPop.Mime.Message>(messageCount);
                for (int i = messageCount; i > 0; i--)
                {
                    lstMessages.Add(oClient.GetMessage(i));
                }
                return lstMessages;
            }
        }
    }
}