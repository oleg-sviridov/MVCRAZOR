using MvcRazor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;

namespace MvcRazor.Controllers
{
    public class ProductController : Controller
    {

        //
        // GET: /Product/

        public ActionResult Save_File()
        {
            // this action will create text file 'your_file_name.txt' with data from
            // string variable 'string_with_your_data', which will be downloaded by
            // your browser
            //todo: add some data from your database into that string:
            //var string_with_your_data = "jyujyj";

            //var byteArray = Encoding.ASCII.GetBytes(string_with_your_data);
            //var stream = new MemoryStream(byteArray);


            string path = HttpRuntime.AppDomainAppPath + @"/sample1.txt";
            //todo: post with the check if file exists
            if (!System.IO.File.Exists(path)) { }

            string content;
            using (StreamReader sr = System.IO.File.OpenText(path))
            {
                 content = sr.ReadToEnd();
            }
            return File(Encoding.UTF8.GetBytes(content), "text/plain", "your_file_name.txt");
        }

        [HttpPost]
        public ActionResult Generate(string amount)
        {

            int Min = 1;
            int Max = 10;
            int Size = 100;

            if (!String.IsNullOrEmpty(amount))
            {
                Int32.TryParse(amount, out Size);
            }

            Random RandInt = new Random();
            
            string path = HttpRuntime.AppDomainAppPath + @"/sample1.txt";

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            using (StreamWriter sw = System.IO.File.AppendText(path))
            {
                for (int i = 0; i < Size; i++)
                {
                    sw.WriteLine(RandInt.Next(Min, Max));
                }
            }

            ViewBag.Message = string.Format("Файл с {0} случайных чисел сгенерирован успешно.", Size);
            return View();
        }

        public ActionResult Index()
        {
            List<Product> products = new List<Product>();

            /*
            products.Add(new Product()
            {
                ProductId = 1,
                Name = "Шариковая ручка",
                Description = "Синяя шариковая ручка с колпачком и прозрачным корпусом.",
                Price = 3m,
                Category = "Канцтовары"
            });

            products.Add(new Product()
            {
                ProductId = 2,
                Name = "Бумага A4",
                Description = "Стандартная бумага для цветной и чёрно-белой печати.",
                Price = 15m,
                Category = "Канцтовары"
            });

            products.Add(new Product()
            {
                ProductId = 2,
                Name = "Мобильный телефон",
                Description = "Мобильный телефон с фотокамерой.",
                Price = 250m,
                Category = "Техника"
            });
            */



            /*int Min = 1;
            int Max = 1000000;
            Random RandInt = new Random();
            List<Int32> RandArray = new List<Int32>();
            for (int i = 0; i < 10; i++)
            {
                RandArray.Add(RandInt.Next(Min, Max));
            }*/
            
            /*int[] RandArray = Enumerable
            .Repeat(0, 2)
            .Select(i => RandInt.Next(Min, Max))
            .ToArray();*/

            // Возвращаем представление из директории Views/Products/Index.cshtml
            // Параметр передающийся в метод View() является моделью, которая будет доступна только на чтение в представлении Index
            return View();
        }

    }
}
