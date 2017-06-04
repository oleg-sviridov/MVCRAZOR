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
        //задаем параметры массива случайных чисел - диапазон и размер
        public int Min = 1;
        public int Max = 1000000;
        public int Size = 1000000;
        //задаем путь к файлу
        public string path = HttpRuntime.AppDomainAppPath + @"/sample.txt";

        //Метод считывания содержимого файла в список класса Numbers
        public List<Numbers> ReadFileToList(string path)
        {
            //инициализация списка класса Numbers
            List<Numbers> NumberList = new List<Numbers>();
            //создаем объект для считывания файла
            using (var fs = System.IO.File.OpenRead(path))
            //считываем файл
            using (var reader = new StreamReader(fs))
            {
                //пока не достигнем конца файла
                while (!reader.EndOfStream)
                {
                    //выделяем значения, разделенные запытой
                    var values = reader.ReadLine().Split(',');
                    //инициализируем переменную класса Numbers
                    Numbers numbers = new Numbers();
                    //заносим значения в numbers
                    Int32.TryParse(values[0], out int Item);
                    numbers.Item = Item;
                    Int32.TryParse(values[1], out int Value);
                    numbers.Value = Value;
                    //Добавляем numbers в NumberList
                    NumberList.Add(numbers);
                }
            }

            return NumberList;
        }

        //Запрос создания файла 1 000 000 случайных значений
        [HttpPost]
        public ActionResult Generate()
        {
            // если файл существует, то удаляем
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            // инициализируем случайное число
            Random RandInt = new Random();
            // заполняем файл случайными числами формата CSV (Номер,Значение)
            using (StreamWriter sw = System.IO.File.AppendText(path))
            {
                for (int i = 1; i <= Size; i++)
                {
                    sw.WriteLine(i + "," + RandInt.Next(Min, Max));
                }
            }
            // Задаем сообщение в View Index, который будет отображен js скриптом
            TempData["Message"] = string.Format("Файл с {0} случайных чисел сгенерирован успешно.", Size);
            // Переходим на View Index
            return RedirectToAction("Index", "Product");
        }

        //запрос сохранения файла на компьютер пользователя
        public ActionResult Save_File()
        {
            //передаем File в качестве результата
            return File(path, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(path));
        }

        [HttpGet]
        public ActionResult Draw()
        {
            //инициализируем списки класса Numbers
            List<Numbers> RandList = new List<Numbers>();
            List<Numbers> CountList = new List<Numbers>();
            List<Numbers> MostList = new List<Numbers>();
            List<Numbers> MostSequenceList = new List<Numbers>();

            //считываем список из файла в список класса Numbers
            RandList = ReadFileToList(path);

            //создаем список только со случайными значениями
            var RandValues = RandList.Select(o => o.Value).ToList();

            //логика для последующего вывода рейтинга 10 самых частых случайных чисел на графике (опционально)
            /*
            //рассчитываем количество повторений какого-либо числа
            var groups = RandValues.GroupBy(item => item);
            //приводим получившийся список к списку класса Numbers
            foreach (var group in groups)
            {
                Numbers numbers = new Numbers();
                numbers.Item = group.Key;
                numbers.Value = group.Count();
                
                CountList.Add(numbers); 
            }
            }*/

            //выбираем первые 10 самых частых случайных чисел
            var most = RandValues.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).Take(10);

            //логика для последующего вывода рейтинга 10 самых частых на графике (опционально)
            /*int j = 1;
            //приводим получившийся список к списку класса Numbers
            foreach (var group in most)
            {
                Numbers numbers = new Numbers();
                numbers.Item = j;
                j++;
                numbers.Value = group;

                MostList.Add(numbers);

            }*/

            //выбираем 10 самых частых случайных чисел из исходного списка RandList
            var result = from Rand in RandList
                        join Most in most.ToList()
                        on Rand.Value equals Most
                        select Rand;
            //приводим получившийся список к списку класса Numbers
            foreach (var val in result)
            {
                Numbers numbers = new Numbers();
                numbers.Item = val.Item;
                numbers.Value = val.Value;
                MostSequenceList.Add(numbers);
            }
            //возвращаем JSonResult в JavaScript
            return Json(MostSequenceList.ToList(), JsonRequestBehavior.AllowGet);
        }

        //запрос поиска случайных значений, повторяющихся определенное количество раз
        [HttpPost]
        public ActionResult Index(string amount)
        {
            //задаем путь к файлу
            string path = HttpRuntime.AppDomainAppPath + @"/sample.txt";
            //инициализируем переменную количества повторений
            int Amount = new int();
            if (!String.IsNullOrEmpty(amount))
            {
                Int32.TryParse(amount, out Amount);
            }
            //инициализируем списки класса Numbers
            List<Numbers> RandList = new List<Numbers>();
            //считываем список из файла в список класса Numbers
            RandList = ReadFileToList(path);
            //создаем список только со случайными значениями
            var RandValues = RandList.Select(o => o.Value).ToList();
            //создаем список только со случайными значениями
            var count = RandValues
                        .GroupBy(e => e)
                        .Where(e => e.Count() == Amount)
                        .Select(e => e.First());
            ViewBag.Message = string.Format("Найдено {0} случайных чисел, повторяющихся {1} раз", count.Count(), Amount);
            return View(count.ToList());
        }

        public ActionResult Index()
        {
            ViewBag.Message = TempData["Message"];
            return View();
        }

    }
}
