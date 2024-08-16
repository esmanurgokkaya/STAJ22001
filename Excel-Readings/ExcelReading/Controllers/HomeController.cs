using ExcelDataReader;
using ExcelReading.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ExcelReading.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }



        public IActionResult Privacy()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Excel()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ExcelAsync(IFormFile file)
        {

            if (file != null && file.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                var filePath = Path.Combine(uploads, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                var dataList = GetDataList(filePath);
                var data = CountData(dataList);
                return View(data);
            }
            return View();
        }

        public List<String> GetDataList(string filePath)
        {
            List<string> data = new List<string>();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.OpenRead(filePath))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        for (int column = 0; column < reader.FieldCount; column++)
                        {
                            var cellValue = reader.GetValue(column);

                            if (cellValue != null)
                            {
                                string cellValueStr = cellValue.ToString().Trim();
                                data.Add(cellValueStr);
                            }
                        }
                    }
                }
            }
            return data.Any() ? data : new List<string>();
        }
        public List<Data> CountData(List<string> data)
        {   
            if (data == null || !data.Any())
            {
                return new List<Data>(); 
            }           
            var dataDictionary = new Dictionary<string, int>();
            foreach (var item in data)
            {
                if (dataDictionary.ContainsKey(item))
                {
                    dataDictionary[item]++;
                }
                else
                {
                    dataDictionary[item] = 1;
                }
            }
            var result = dataDictionary.Select(kvp => new Data
            {
                Name = kvp.Key,
                Count = kvp.Value
            }).ToList();

            return result;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
