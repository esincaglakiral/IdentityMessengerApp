using EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMessagingApp.ViewComponents
{
    public class _DefaultNavbarComponent : ViewComponent
    {
        private readonly UserManager<WriterUser> _userManager;

        public _DefaultNavbarComponent(UserManager<WriterUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.userImage = values.ImageUrl;

            return View();
        }
    }
}
