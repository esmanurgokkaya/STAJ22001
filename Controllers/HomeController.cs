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
                // dosya yolu yoksa tan�mlan�r
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                // kaydedilecek belge dosya yoluna eklenir
                var filePath = Path.Combine(uploads, file.FileName);
                // yeni yol kullan�larak belge kaydedilir.
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                //belgeyi okuyacak metot �a�r�l�r
                var dataList = GetDataList(filePath);
                //okunan belgedeki tekrarlananlar� d�nd�recek metot �a�r�l�r
                var data = CountData(dataList);
                return View(data);
            }

            return View();
        }


        public List<Data> CountData(List<string> data)

        {   // gelen veri bo�sa bo� list d�nd�r�l�r
            if (data == null || !data.Any())
            {
                return new List<Data>(); 
            }
            // dictionary yap�s� olu�turulur
            var dataDictionary = new Dictionary<string, int>();
            //gelen veriler dola��l�r ve dictionary i�ine eklenir tekrar  kar��la��nca de�eri art�r�l�r.
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
            // model yap�m�za uyarlamak i�in liste d�n���m yap�l�r
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
            // y�klenen belge yolu metodun parametresinden al�n�p okunur
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
                            {   // ba�tan ve sondan bo�luk karakterini silerek verilerin tekrar�n� daha do�ru hesaplar
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
