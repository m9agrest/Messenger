using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using SimpleMessenger.Data;
using SimpleMessenger.Models;

namespace SimpleMessenger.Controllers
{
    public class MessagesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MessagesController(AppDbContext context, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
        }
        private User getUser()
        {
            if (_httpContextAccessor.HttpContext.Session.GetInt32("UserId") != null)
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == _httpContextAccessor.HttpContext.Session.GetInt32("UserId"));
                if (user != null)
                {
                    return user;
                }
                _httpContextAccessor.HttpContext.Session.Remove("UserId");
            }
            return null;
        }

        // GET: Messages
        public async Task<IActionResult> Index()
        {
            var user = getUser();
            if (user == null || user.Id >= Program.Admin)
            {
                return NotFound();
            }
            var appDbContext = _context.Messages.Include(m => m.Recipient).Include(m => m.Sender);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user = getUser();
            if (user == null || user.Id >= Program.Admin)
            {
                return NotFound();
            }
            if (id == null || _context.Messages == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Recipient)
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }
        public FileResult GetReport()
        {
            var user = getUser();
            if (user == null || user.Id >= Program.Admin)
            {
                return File(new byte[] { }, "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet");
            }
            // Путь к файлу с шаблоном
            string path = "/Reports/Messages.xlsx";
            //Путь к файлу с результатом
            string result = "/Reports/MessagesReport.xlsx";
            FileInfo fi = new FileInfo(_webHostEnvironment.WebRootPath + path);
            FileInfo fr = new FileInfo(_webHostEnvironment.WebRootPath + result);
            //будем использовть библитотеку не для коммерческого использования
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //открываем файл с шаблоном
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                //устанавливаем поля документа
                excelPackage.Workbook.Properties.Author = "SimpleMessenger";
                excelPackage.Workbook.Properties.Title = "Сообщения";
                excelPackage.Workbook.Properties.Subject = "Список всех сообщений пользователей проекта";
                excelPackage.Workbook.Properties.Created = DateTime.Now;
                //плучаем лист по имени.
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Messages"];
                //получаем списко пользователей и в цикле заполняем лист данными
                int startLine = 2;
                List<Message> Items = _context.Messages.ToList();
                foreach (Message item in Items)
                {
                    if (item != null)
                    {
                        worksheet.Cells[startLine, 1].Value = item.SenderId;
                        //worksheet.Cells[startLine, 2].Value = item.Sender.Name;
                        worksheet.Cells[startLine, 3].Value = item.Recipient;
                        //worksheet.Cells[startLine, 4].Value = item.Recipient.Name;
                        worksheet.Cells[startLine, 5].Value = item.Date;
                        worksheet.Cells[startLine, 6].Value = item.Text;
                        startLine++;
                    }
                }
                //созраняем в новое место
                excelPackage.SaveAs(fr);
            }
            // Тип файла - content-type
            string file_type = "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet";
            // Имя файла - необязательно
            string file_name = "MessagesReport.xlsx";
            return File(result, file_type, file_name);
        }
    }
}
