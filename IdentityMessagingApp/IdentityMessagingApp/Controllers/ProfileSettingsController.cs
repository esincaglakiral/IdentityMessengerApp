using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using EntityLayer.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMessagingApp.Controllers
{
    public class ProfileSettingsController : Controller
    {
        private readonly UserManager<WriterUser> _userManager;

        public ProfileSettingsController(UserManager<WriterUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var values = await _userManager.FindByNameAsync(User.Identity.Name);
            Context c = new Context();
            ViewBag.IncomingMessages = c.WriterUserMessages.Where(x => x.Receiver == values.Email && x.Status == false && x.Trash == false).Count();
            ViewBag.SentMessages = c.WriterUserMessages.Where(x => x.Sender == values.Email && x.Trash == false).Count();
            ViewBag.MessagesInTrash = c.WriterUserMessages.Where(x => x.Trash == true).Count();
            UserEditViewModel model = new UserEditViewModel();
            model.Name = values.Name;
            model.Surname = values.Surname;
            model.PictureURL = values.ImageUrl;
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Index(UserEditViewModel p)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (p.Picture != null)
            {
                var resource = Directory.GetCurrentDirectory();
                var extension = Path.GetExtension(p.Picture.FileName);
                var imagename = ($"{Guid.NewGuid()}{extension}");
                var savelocation = ($"{resource}/wwwroot/userimage/{imagename}");
                var stream = new FileStream(savelocation, FileMode.Create);
                await p.Picture.CopyToAsync(stream);
                user.ImageUrl = imagename;
            }
            user.Name = p.Name;
            user.Surname = p.Surname;
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, p.Password);  //şifre güncelleme ve hashleme
            var result = await _userManager.UpdateAsync(user); //identity 'e bağlı user tablosunda ad soyad ve şifre günceller
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
    }
}
