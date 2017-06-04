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
        public ActionResult Index(string amount)
        {

            int Min = 1;
            int Max = 1000000;
            int Size = 1000000;

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
                for (int i = 1; i <= Size; i++)
                {
                    sw.WriteLine(i + "," + RandInt.Next(Min, Max));
                }
            }

            ViewBag.Message = string.Format("Файл с {0} случайных чисел сгенерирован успешно.", Size);
            return View();
        }

        [HttpGet]
        public ActionResult Draw()
        {
            string path = HttpRuntime.AppDomainAppPath + @"/sample1.txt";
            List<Numbers> RandList = new List<Numbers>();
            List<Numbers> CountList = new List<Numbers>();
            List<Numbers> MostList = new List<Numbers>();
            List<Numbers> MostSequenceList = new List<Numbers>();

            //todo: post with the check if file exists
            if (!System.IO.File.Exists(path)) { }

            using (var fs = System.IO.File.OpenRead(path))
            using (var reader = new StreamReader(fs))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    Numbers numbers = new Numbers();

                    Int32.TryParse(values[0], out int Item);
                    numbers.Item = Item;
                    Int32.TryParse(values[1], out int Value);
                    numbers.Value = Value;

                    RandList.Add(numbers);
                }
            }

            var RandValues = RandList.Select(o => o.Value).ToList();
            //количество повторений какого-либо числа
            var groups = RandValues.GroupBy(item => item);
            foreach (var group in groups)
            {
                Numbers numbers = new Numbers();
                numbers.Item = group.Key;
                numbers.Value = group.Count();
                
                CountList.Add(numbers); 

            }
            //первые 10 самые частые
            var most = RandValues.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).Take(10);

            //логика для последующего вывода рейтинга 10 самых частых на графике
            /*int j = 1;

            foreach (var group in most)
            {
                Numbers numbers = new Numbers();
                numbers.Item = j;
                j++;
                numbers.Value = group;

                MostList.Add(numbers);

            }*/


            var query = from Rand in RandList
                        join Most in most.ToList()
                        on Rand.Value equals Most
                        select Rand;

            foreach (var val in query)
            {
                Numbers numbers = new Numbers();
                numbers.Item = val.Item;
                numbers.Value = val.Value;

                MostSequenceList.Add(numbers);

            }

            return Json(MostSequenceList.ToList(), JsonRequestBehavior.AllowGet);
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
