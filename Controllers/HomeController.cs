using ExcelDataReader;
using ExcelReading.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace WebApplication6.Controllers
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
                // dosya yolu berlirlenir
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files");
                // dosya yolu yoksa tanýmlanýr
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                // kaydedilecek belge dosya yoluna eklenir
                var filePath = Path.Combine(uploads, file.FileName);
                // yeni yol kullanýlarak belge kaydedilir.
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                //belgeyi okuyacak metot çaðrýlýr
                var dataList = GetDataList(filePath);
                //okunan belgedeki tekrarlananlarý döndürecek metot çaðrýlýr
                var data = CountData(dataList);
                return View(data);
            }

            return View();
        }


        public List<Data> CountData(List<string> data)

        {   // gelen veri boþsa boþ list döndürülür
            if (data == null || !data.Any())
            {
                return new List<Data>(); 
            }
            // dictionary yapýsý oluþturulur
            var dataDictionary = new Dictionary<string, int>();
            //gelen veriler dolaþýlýr ve dictionary içine eklenir tekrar  karþýlaþýnca deðeri artýrýlýr.
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
            // model yapýmýza uyarlamak için liste dönüþüm yapýlýr
            var result = dataDictionary.Select(kvp => new Data
            {
                Name = kvp.Key,
                Count = kvp.Value
            }).ToList();

            return result;
        }
        public List<String> GetDataList(string filePath)
        {
            List<string> data = new List<string>();
            // yüklenen belge yolu metodun parametresinden alýnýp okunur
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
                            {   // baþtan ve sondan boþluk karakterini silerek verilerin tekrarýný daha doðru hesaplar
                                string cellValueStr = cellValue.ToString().Trim();
                                data.Add(cellValueStr);
                            }
                        }
                    }
                }
            }
            return data.Any() ? data : new List<string>();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
