using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace IdentityMessagingApp.Controllers
{
    public class DashboardMessangerController : Controller
    {
        private readonly UserManager<WriterUser> _userManager;

        public DashboardMessangerController(UserManager<WriterUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var values = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.LoggedInUser = ($"{values.Name} {values.Surname}");

            //Weather APi
            string city = "istanbul";
            string api = "67db534b921c0d3eaffb2aefa96c322e";
            string api2 = "60b618984024156d0a007238e4396f6d";

            try
            {
                string connection = ($"http://api.openweathermap.org/data/2.5/weather?q={city}&mode=xml&lang=tr&units=metric&appid={api}");
                XDocument document = XDocument.Load(connection);
                ViewBag.weather = document.Descendants("temperature").ElementAt(0).Attribute("value").Value;
            }
            catch (Exception)
            {

                string connection = ($"http://api.openweathermap.org/data/2.5/weather?q={city}&mode=xml&lang=tr&units=metric&appid={api2}");
                XDocument document = XDocument.Load(connection);
                ViewBag.weather = document.Descendants("temperature").ElementAt(0).Attribute("value").Value;
            }

            //statistics
            Context c = new Context();
            ViewBag.TotalMessage = c.WriterUserMessages.Where(x => x.Receiver == values.Email).Count();
            ViewBag.TotalUser = c.Users.Count();
            ViewBag.IncomingMessages = c.WriterUserMessages.Where(x => x.Receiver == values.Email && x.Status == false && x.Trash == false).Count();
            ViewBag.SentMessages = c.WriterUserMessages.Where(x => x.Sender == values.Email && x.Trash == false).Count();
            ViewBag.MessagesInTrash = c.WriterUserMessages.Where(x => x.Trash == true).Count();
            return View();
        }
    }
}
