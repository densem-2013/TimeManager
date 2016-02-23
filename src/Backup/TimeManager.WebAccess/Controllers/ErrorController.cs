namespace Infocom.TimeManager.WebAccess.Controllers
{
    using System.Web.Mvc;
    using System;


    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using System.Web;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Net;
    using System.Threading;
    using System.Web.Security;
    using System.Text;


    using Infocom.TimeManager.WebAccess.Extensions;

    public partial class ErrorController : TimeManagerBaseController
    {
        public virtual ActionResult Index(Exception error)
        {
            var m = error.Message; 

            if (error.InnerException != null) m += " | " + error.InnerException.Message;
            var p = Request.ServerVariables.AllKeys.ToList().ToString();
      
            string mailTemplate = @"<p><b>Error:</b> {0}</p>
                                    <p><b>StackTrace:</b>{1}</p>
                                    <p><b>Адрес сервера:</b>{2}</p>
                                    <p><b>Пользователь:</b>{3}</p>
                                    <p><b>Ссылка входа:</b>{4}</p>
                                    <p><b>IP-адрес пользователя:</b>{5}</p>  
                                    <p><b>Имя компьютера:</b>{6}</p> 
                                                                ";             
                                                                                                                      
                                                          
                string message = string.Format(mailTemplate, m, error.StackTrace, 
                Request.ServerVariables["LOCAL_ADDR"].ToString(),
                Request.ServerVariables["LOGON_USER"].ToString(),
                Request.Url.AbsoluteUri.ToString(),
                Request.UserHostAddress.ToString(),
                Request.UserHostName.ToString()
                             
                );
            var subject = string.Format("ERROR: {0}.", error.Message.Substring(0,100));
            var subscribers = new List<string>();
            subscribers.Add("timemanager@dit.infocom-ltd.com");
            this.WatchdogMailMessageHtml(subscribers, message, subject);

            return View("Index");
            
            //if (Request.IsAjaxRequest())
            //{
            //    if (error is TimeManagerException)
            //        return View("Expected");
            //    return View("UnExpected");
            //}

            //if (error is TimeManagerException)
            //    return View("Expected", new ErrorDisplay { Message = error.Message });
            //return View("Error", new ErrorDisplay { Message = error.Message });
        }

        private void WatchdogMailMessageHtml(List<string> messageTo, string body, string subject) //html-шаблон для отправки писем по открытию-закрытию заявок и проектов
        {
            //var watchdogSettings = new WatchdogSettings();
            var mailMessage = new MailMessage();
            try
            {
                var smtpServerName = Properties.Settings.Default.SMTPServerName;
                using (var smtpClient = new SmtpClient(smtpServerName))
                {
                    smtpClient.Port = 25;
                    mailMessage.From = new MailAddress("NoReply@dit.infocom-ltd.com");
                    foreach (var address in messageTo.Cast<string>().Where(address => !string.IsNullOrEmpty(address)))
                    {
                        mailMessage.To.Add(new MailAddress(address));
                    }
                    mailMessage.Subject = "[Time Manager]" + subject;

                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body = body;
                    smtpClient.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
              
            }
        }




        public virtual ActionResult ErrorMessage(string message)
        {
            ViewBag.Message = message;
            return View("Index", new Exception (message));
        }

        public virtual ActionResult HttpError404(Exception error)
        {
            return View();
        }

        public virtual ActionResult HttpError505(Exception error)
        {
            return View();
        }
    }
}