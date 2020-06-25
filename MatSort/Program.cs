using System;
using System.Threading;

namespace MergeSortThreading
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread t = Thread.CurrentThread; //Инициализация потока 

            t.Name = "Главный поток";
            Console.WriteLine($"Имя потока: {t.Name}");
            Console.WriteLine($"Включение потока: {t.IsAlive}");
            Console.WriteLine($"Приоритет потока: {t.Priority}");
            Console.WriteLine($"Статус потока: {t.ThreadState}");

            int[] arr = new int[65];

            Random rnd = new Random();

            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = rnd.Next(100);
            }

            for (int i = 0; i < arr.Length; i++)
            {
                Console.Write($"{arr[i]}, ");
            }
            Console.WriteLine();

            int n;

            while (true)
            {
                Console.Write("Введите количество потоков(1,2,4,8) = ");
                int.TryParse(Console.ReadLine(), out n);
                if (n == 1 || n == 2 || n == 4 || n == 8) break;
                else Console.WriteLine("Некорректный ввод");
            }


            MultithreadedSort(arr, n);


            for (int i = 0; i < arr.Length; i++)
            {
                Console.Write($"{arr[i]}, ");
            }
            Console.WriteLine();

            Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
            Console.ReadKey();
        }

        public static void MultithreadedSort(int[] arr, int threads)
        {
            int[][] arrs = new int[threads][];

            Thread[] sorting = new Thread[threads]; //Создание массива потоков

            for (int i = 0; i < threads; i++) //Пока не закончились потоки
            {
                arrs[i] = new int[arr.Length / threads]; //Создание массива с размерностью 

                Array.Copy(arr, i * (arr.Length / threads), arrs[i], 0, arr.Length / threads);

                sorting[i] = new Thread(new ParameterizedThreadStart(sort)); //Инициализация потока для сортировки

                sorting[i].Start(arrs[i]); //Запуск потока 
            }

            for (int i = 0; i < threads; i++) //Пока не закончились потоки

                sorting[i].Join(); // блокируем предыдующие 

            bool norm = false;

            while (arrs.Length != 1)
            {
                int k = 0;
                int[][] tmp = new int[arrs.Length / 2][];

                for (int i = 0; i < arrs.Length; i++) //Пока не закончился массив
                {
                    if (arr.Length % 2 != 0 && !norm)
                    {
                        tmp[k] = new int[arrs[i].Length + arrs[i + 1].Length + 1];
                        tmp[k][tmp[k].Length - 1] = arr[arr.Length - 1];
                        norm = true;
                    }
                    else
                    {
                        tmp[k] = new int[arrs[i].Length + arrs[i + 1].Length];
                    }
                    Array.Copy(arrs[i], 0, tmp[k], 0, arrs[i].Length);
                    try
                    {
                        Array.Copy(arrs[i + 1], 0, tmp[k], arrs[i].Length, arrs[i + 1].Length);
                    }
                    catch (Exception)
                    {

                    }
                    i++;
                    k++;
                }

                Thread[] merging = new Thread[tmp.Length];  //Создание массива парралельных потоков

                for (int j = 0; j < tmp.Length; j++)
                {
                    merging[j] = new Thread(new ParameterizedThreadStart(sort)); //Инициализация парралельного потока для сортировки

                    merging[j].Start(tmp[j]); //Запуск парралельного потока 
                }

                for (int i = 0; i < tmp.Length; i++) //Блокирование потока до завершения
                    merging[i].Join();

                arrs = tmp;
            }

            arrs[0].CopyTo(arr, 0);
        }

        public static void sort(object arr)
        {
            Array.Sort((int[])arr, 0, ((int[])arr).Length);
        }
    }
}