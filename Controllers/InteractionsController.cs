using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using OfficeOpenXml;
using SimpleMessenger.Data;
using SimpleMessenger.Models;
using Interaction = SimpleMessenger.Models.Interaction;

namespace SimpleMessenger.Controllers
{
    public class InteractionsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public InteractionsController(AppDbContext context, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
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




        // GET: Interactions
        public async Task<IActionResult> Index()
        {
            var user = getUser();
            if (user == null || user.Id >= Program.Admin)
            {
                return NotFound();
            }
            var appDbContext = _context.Interactions.Include(i => i.User1).Include(i => i.User2);
            return View(await appDbContext.ToListAsync());
        }



        public FileResult GetReport()
        {
            var user = getUser();
            if (user == null || user.Id >= Program.Admin)
            {
                return File(new byte[] { }, "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet");
            }
            // Путь к файлу с шаблоном
            string path = "/Reports/Interactions.xlsx";
            //Путь к файлу с результатом
            string result = "/Reports/InteractionsReport.xlsx";
            FileInfo fi = new FileInfo(_webHostEnvironment.WebRootPath + path);
            FileInfo fr = new FileInfo(_webHostEnvironment.WebRootPath + result);
            //будем использовть библитотеку не для коммерческого использования
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //открываем файл с шаблоном
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                //устанавливаем поля документа
                excelPackage.Workbook.Properties.Author = "SimpleMessenger";
                excelPackage.Workbook.Properties.Title = "Отношения";
                excelPackage.Workbook.Properties.Subject = "Список всех отношений пользователей проекта";
                excelPackage.Workbook.Properties.Created = DateTime.Now;
                //плучаем лист по имени.
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Interactions"];
                //получаем списко пользователей и в цикле заполняем лист данными
                int startLine = 2;
                List<Interaction> Items = _context.Interactions.ToList();
                foreach (Interaction item in Items)
                {
                    if(item != null)
                    {
                        worksheet.Cells[startLine, 1].Value = item.User1Id;
                        //worksheet.Cells[startLine, 2].Value = item.User1.Name;
                        worksheet.Cells[startLine, 3].Value = item.User2Id;
                        //worksheet.Cells[startLine, 4].Value = item.User2.Name;
                        worksheet.Cells[startLine, 5].Value = item.Type;
                        startLine++;
                    }
                }
                //созраняем в новое место
                excelPackage.SaveAs(fr);
            }
            // Тип файла - content-type
            string file_type = "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet";
            // Имя файла - необязательно
            string file_name = "InteractionsReport.xlsx";
            return File(result, file_type, file_name);
        }
    }
}
