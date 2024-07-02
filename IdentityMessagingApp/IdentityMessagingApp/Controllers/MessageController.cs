using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace IdentityMessagingApp.Controllers
{
    public class MessageController : Controller
    {
        WriterUserMessageManager _userMessage = new WriterUserMessageManager(new EFWriterUserMessageDal());

        private readonly UserManager<WriterUser> _userManager;

        public MessageController(UserManager<WriterUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

		public async Task<IActionResult> ReceiverMessage(string p, int page = 1)
		{
			var values = await _userManager.FindByNameAsync(User.Identity.Name);
			p = values.Email;

			ViewBag.Giden = _userMessage.GetListSenderMessage(p).Where(x => x.Trash == false).Count();
			ViewBag.Gelen = _userMessage.GetListReceiverMessage(p).Where(x => x.Status == false && x.Trash == false).Count();
			ViewBag.Cop = _userMessage.TGetList().Where(x => x.Trash == true).Count();

			var messagelist = _userMessage.GetListReceiverMessage(p).Where(x => x.Trash == false).OrderBy(x => x.Status).OrderByDescending(x => x.Date).ToPagedList(page, 8);
			return View(messagelist);
		}

		public IActionResult SenderMessageRead(int id)
		{
			var values = _userMessage.TGetByID(id);
			values.Status = values.Status == true ? false : true;
			_userMessage.TUpdate(values);
			return Redirect("~/Message/ReceiverMessage/");
		}



		public async Task<IActionResult> SenderMessage(string p, int page = 1)
		{
			var values = await _userManager.FindByNameAsync(User.Identity.Name);
			p = values.Email;

			var messagelist = _userMessage.GetListSenderMessage(p).Where(x => x.Trash == false).OrderByDescending(x => x.Date).ToPagedList(page, 8);

			ViewBag.Gelen = _userMessage.GetListReceiverMessage(p).Where(x => x.Status == false && x.Trash == false).Count();
			ViewBag.Giden = _userMessage.GetListSenderMessage(p).Where(x => x.Trash == false).Count();
			ViewBag.Cop = _userMessage.TGetList().Where(x => x.Trash == true).Count();

			return View(messagelist);
		}


		[HttpGet]
		public async Task<IActionResult> SenderMessageDetails(int id)
		{
			var values = await _userManager.FindByNameAsync(User.Identity.Name);
			Context context = new Context();
			ViewBag.Gelen = context.WriterUserMessages.Where(x => x.Receiver == values.Email && x.Status == false && x.Trash == false).Count();
			ViewBag.Giden = context.WriterUserMessages.Where(x => x.Sender == values.Email && x.Trash == false).Count();
			ViewBag.Cop = context.WriterUserMessages.Where(x => x.Trash == true).Count();

			WriterUserMessage message = _userMessage.TGetByID(id);
			return View(message);
		}


		[HttpGet]
		public async Task<IActionResult> ReceiverMessageDetails(int id)
		{

			var values = await _userManager.FindByNameAsync(User.Identity.Name);
			Context context = new Context();
			ViewBag.Gelen = context.WriterUserMessages.Where(x => x.Receiver == values.Email && x.Status == false && x.Trash == false).Count();
			ViewBag.Giden = context.WriterUserMessages.Where(x => x.Sender == values.Email && x.Trash == false).Count();
			ViewBag.Cop = context.WriterUserMessages.Where(x => x.Trash == true).Count();

			WriterUserMessage message = _userMessage.TGetByID(id);
			return View(message);
		}


		[HttpGet]
		public async Task<IActionResult> SendMessage()
		{
			var values = await _userManager.FindByNameAsync(User.Identity.Name);
			Context context = new Context();
			ViewBag.Gelen = context.WriterUserMessages.Where(x => x.Receiver == values.Email && x.Status == false && x.Trash == false).Count();
			ViewBag.Giden = context.WriterUserMessages.Where(x => x.Sender == values.Email && x.Trash == false).Count();
			ViewBag.Cop = context.WriterUserMessages.Where(x => x.Trash == true).Count();

			List<SelectListItem> list = (from x in _userManager.Users
										 select new SelectListItem
										 {
											 Text = x.Email,
											 Value = x.Email.ToString()
										 }).ToList();
			ViewBag.mail = list;

			return View();
		}


		[HttpPost]
		public async Task<IActionResult> SendMessage(WriterUserMessage p)
		{

			var values = await _userManager.FindByNameAsync(User.Identity.Name);
			string mail = values.Email;

			string name = ($"{values.Name} {values.Surname}");
			p.Date = Convert.ToDateTime(DateTime.Now);
			p.Sender = mail;
			p.SenderName = name;
			p.Status = false;
			Context context = new Context();
			var usernamesurname = context.Users.Where(x => x.Email == p.Receiver).Select(y => y.Name + " " + y.Surname).FirstOrDefault();
			if (usernamesurname == null)
			{
				ModelState.AddModelError("", "Sistemde Kayıtlı Mail Adresi Bulunamadı");
			}
			else
			{
				p.ReceiverName = usernamesurname;
				_userMessage.TAdd(p);
				return Redirect("~/Message/SenderMessage/");
			}
			return View();
		}


		[HttpGet]
		public async Task<IActionResult> RubbishMessage(int page = 1)
		{

			var valuese = await _userManager.FindByNameAsync(User.Identity.Name);
			Context context = new Context();
			ViewBag.Gelen = context.WriterUserMessages.Where(x => x.Receiver == valuese.Email && x.Status == false && x.Trash == false).Count();
			ViewBag.Giden = context.WriterUserMessages.Where(x => x.Sender == valuese.Email && x.Trash == false).Count();
			ViewBag.Cop = _userMessage.TGetList().Where(x => x.Trash == true).Count();

			var values = _userMessage.TGetList().Where(x => x.Trash == true).ToPagedList(page, 8);


			return View(values);
		}



		public async Task<IActionResult> RubbishMessager(int id)
		{
			var valuese = await _userManager.FindByNameAsync(User.Identity.Name);

			var values = _userMessage.TGetByID(id);
			values.Trash = values.Trash == true ? false : true;

			_userMessage.TUpdate(values);
			if (values.Sender == valuese.Email)
			{
				return Redirect("~/Message/SenderMessage/");
			}
			else
			{
				return Redirect("~/Message/ReceiverMessage/");
			}
		}

	}
}
