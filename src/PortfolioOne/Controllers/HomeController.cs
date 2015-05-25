using Microsoft.AspNet.Mvc;
using PortfolioOne.Models;
using System.Net;
using System.Net.Mail;

namespace PortfolioOne.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        //[GoogleRecaptcha] - TODO
        public ActionResult _Contact(ContactModel model)
        {
            if (ModelState.IsValid)
            {
                
                MailAddress fromAddress = new MailAddress(model.Email, model.FirstName + model.LastName);
                MailAddress toAddress = new MailAddress(Global.Configuration.Get("SmtpSettings:ToAddress"), Global.Configuration.Get("SmtpSettings:ToName"));

                int port = 0;
                int.TryParse(Global.Configuration.Get("SmtpSettings:Port"), out port);

                using (SmtpClient client = new SmtpClient() {
                    Host = Global.Configuration.Get("SmtpSettings:Server"),
                    Port = port,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(Global.Configuration.Get("SmtpSettings:SenderAccount"), Global.Configuration.Get("SmtpSettings:SenderPassword"))
                })
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = "Contact from portfolio site!",
                    Body = model.Message
                })
                {
                    client.Send(message);
                }
                return Json(new { Success = true, Message = "Thank you for the message, i will get back with you as soon as possible." });
            }
            return PartialView(model);
        }
    }
}
