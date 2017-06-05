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
        public int Size = 10000000;
        //задаем путь к файлу
        public string path = HttpRuntime.AppDomainAppPath + @"/sample.txt";

        //Метод считывания содержимого файла в список класса Numbers
        public Numbers[] ReadFileToList(string path)
        {
            //инициализация списка класса Numbers
            Numbers[] NumberList = new Numbers[10000000];
            //создаем объект для считывания файла
            using (var fs = System.IO.File.OpenRead(path))
            //считываем файл
            using (var reader = new StreamReader(fs))
            {
                //пока не достигнем конца файла
                for (int i = 0; (!reader.EndOfStream); i++)
                {
                    //выделяем значения, разделенные запытой
                    var values = reader.ReadLine().Split(',');
                    //инициализируем переменную класса Numbers
                    Numbers numbers = new Numbers();
                    //заносим значения в numbers
                    int Item;
                    int Value;
                    Int32.TryParse(values[0], out Item);
                    numbers.Item = Item;
                    Int32.TryParse(values[1], out Value);
                    numbers.Value = Value;
                    //Добавляем numbers в NumberList
                    NumberList[i] = numbers;
                }
            }

            return NumberList;
        }

        private void TransmitFile(string fullPath, string outFileName)
        {
            System.IO.Stream iStream = null;

            // Буффер для чтения массива из 10 000 байт за один раз d 10K bytes in chunk:
            byte[] buffer = new Byte[10000];

            // Длина файла:
            int length;

            // Количество байт для чтения:
            long dataToRead;

            // Определяем имя файла.
            string filename = System.IO.Path.GetFileName(fullPath);

            try
            {
                // Открываем файл
                iStream = new System.IO.FileStream(fullPath, System.IO.FileMode.Open,
                            System.IO.FileAccess.Read, System.IO.FileShare.Read);


                // Количество байт для чтения:
                dataToRead = iStream.Length;
                //определяем HttpResponse типа application/octet-stream и добавляем заголовок 
                Response.Clear();
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + outFileName);
                Response.AddHeader("Content-Length", iStream.Length.ToString());

                // Считывание байтов файла до конца
                while (dataToRead > 0)
                {
                    // Смотрим, что клиент подключен
                    if (Response.IsClientConnected)
                    {
                        // Считываем данные из iStream в буффер
                        length = iStream.Read(buffer, 0, 10000);

                        // Пишем буффер в выходной поток HttpResponse
                        Response.OutputStream.Write(buffer, 0, length);

                        // Отправляем OutputStream клиенту
                        Response.Flush();

                        // Переопределяем буффер и уменьшаем указатель количества оставшегося размера для чтения файла
                        buffer = new Byte[10000];
                        dataToRead = dataToRead - length;
                    }
                    else
                    {
                        //предотвращаем бесконечный цикл, если клиент отключился
                        dataToRead = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                if (iStream != null)
                {
                    //Закрываем файл.
                    iStream.Close();
                }
                Response.Close();
            }
        }


        //Запрос создания файла 10 000 000 случайных значений
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
                sw.Close();
            }
            // Задаем сообщение в View Index, который будет отображен js скриптом
            TempData["Message"] = string.Format("Файл с {0} случайных чисел сгенерирован успешно.", Size);
            // Переходим на View Index
            return RedirectToAction("Index", "Product");
        }

        //запрос сохранения файла на компьютер пользователя
        public ActionResult Save_File()
        {
            //передаем File в качестве результата, если файл случайных значений существует
            if (!System.IO.File.Exists(path))
            {
                // Задаем сообщение в View Index, который будет отображен js скриптом
                TempData["Message"] = string.Format("Файл не существует. Необходимо сначала сгенерировать файл случайных значений");
                // Переходим на View Index
                return RedirectToAction("Index", "Product");
            }
            else
            {
                //Вызываем метод передачи файла клиенту
                TransmitFile(path, "sample.txt");
                return RedirectToAction("Index", "Product");
            }
        }

        //Запрос ajax из chart.js. действие контроллера 
        //возвращает JsonResult - в виде списка самых частых значений и их позиции в массиве
        [HttpGet]
        public ActionResult Draw()
        {
            //выдаем список в формате JsonResult, если существует файл случайных значений
            if (!System.IO.File.Exists(path))
            {
                // Задаем сообщение в View Index, который будет отображен js скриптом
                TempData["Message"] = string.Format("Файл не существует. Необходимо сначала сгенерировать файл случайных значений");
                // Переходим на View Index
                return RedirectToAction("Index", "Product");
            }
            else
            {
                //инициализируем списки класса Numbers
                Numbers[] RandList = new Numbers[10000000];
                List<Numbers> MostSequenceList = new List<Numbers>();

                //считываем список из файла в список класса Numbers
                RandList = ReadFileToList(path);

                //создаем список только со случайными значениями
                var RandValues = RandList.Select(o => o.Value);

                //выбираем первые 10 самых частых случайных чисел
                var most = RandValues.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).Take(10);

                //выбираем 10 самых частых случайных чисел из исходного списка RandList
                var result = from Rand in RandList
                             join Most in most
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
                return Json(MostSequenceList, JsonRequestBehavior.AllowGet);


            }
        }

        //запрос поиска случайных значений, повторяющихся определенное количество раз
        [HttpPost]
        public ActionResult Index(string amount)
        {
            //выдаем список в формате JsonResult, если существует файл случайных значений
            if (!System.IO.File.Exists(path))
            {
                // Задаем сообщение в View Index, который будет отображен js скриптом
                TempData["Message"] = string.Format("Файл не существует. Необходимо сначала сгенерировать файл случайных значений");
                // Переходим на View Index
                return RedirectToAction("Index", "Product");
            }
            else
            {
                //инициализируем переменную количества повторений
                int Amount = new int();
                if (!String.IsNullOrEmpty(amount))
                {
                    Int32.TryParse(amount, out Amount);
                }
                //инициализируем списки класса Numbers
                Numbers[] RandList = new Numbers[10000000];
                //считываем список из файла в список класса Numbers
                RandList = ReadFileToList(path);
                //создаем список только со случайными значениями
                var RandValues = RandList.Select(o => o.Value);
                //создаем список только со случайными значениями
                var count = RandValues
                            .GroupBy(e => e)
                            .Where(e => e.Count() == Amount)
                            .Select(e => e.First()).ToList();

                // Задаем сообщение в View Index, который будет отображен js скриптом
                ViewBag.Message = string.Format("Найдено {0} случайных чисел, повторяющихся {1} раз", count.Count(), Amount);
                return View(count);
            }
        }

        public ActionResult Index()
        {
            //выводим основной вид Index и сообщение
            ViewBag.Message = TempData["Message"];
            return View();
        }

    }
}
