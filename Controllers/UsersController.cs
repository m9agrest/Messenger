using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using SimpleMessenger.Data;
using SimpleMessenger.Models;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace SimpleMessenger.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public UsersController(AppDbContext context, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
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

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var user = getUser();
            if(user == null || user.Id >= Program.Admin)
            {
                return NotFound();
            }
              return _context.Users != null ? 
                          View(await _context.Users.ToListAsync()) :
                          Problem("Entity set 'AppDbContext.Users'  is null.");
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user = getUser();
            if (user == null || user.Id >= Program.Admin)
            {
                return NotFound();
            }
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        public FileResult GetReport()
        {
            var user = getUser();
            if (user == null || user.Id >= Program.Admin)
            {
                return File(new byte[] { }, "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet");
            }
            // Путь к файлу с шаблоном
            string path = "/Reports/Users.xlsx";
            //Путь к файлу с результатом
            string result = "/Reports/UsersReport.xlsx";
            FileInfo fi = new FileInfo(_webHostEnvironment.WebRootPath + path);
            FileInfo fr = new FileInfo(_webHostEnvironment.WebRootPath + result);
            //будем использовть библитотеку не для коммерческого использования
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //открываем файл с шаблоном
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                //устанавливаем поля документа
                excelPackage.Workbook.Properties.Author = "SimpleMessenger";
                excelPackage.Workbook.Properties.Title = "Пользователи";
                excelPackage.Workbook.Properties.Subject = "Список всех пользователей проекта";
                excelPackage.Workbook.Properties.Created = DateTime.Now;
                //плучаем лист по имени.
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Users"];
                //получаем списко пользователей и в цикле заполняем лист данными
                int startLine = 2;
                List<User> Items = _context.Users.ToList();
                foreach (User item in Items)
                {
                    if (item != null)
                    {
                        worksheet.Cells[startLine, 1].Value = item.Id;
                        worksheet.Cells[startLine, 2].Value = item.Name;
                        worksheet.Cells[startLine, 3].Value = item.Email;
                        startLine++;
                    }
                }
                //созраняем в новое место
                excelPackage.SaveAs(fr);
            }
            // Тип файла - content-type
            string file_type = "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet";
            // Имя файла - необязательно
            string file_name = "UsersReport.xlsx";
            return File(result, file_type, file_name);
        }
    }
}
