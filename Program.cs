using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Parallel
{
    class Program
    {
        public class SortElement : IComparable<SortElement>
        {
            public SortElement(int num)
            {
                SortNumber = num;
            }

            public int SortNumber { get; private set; }
            public int CompareTo(SortElement el)//(object obj)
            {
                //var other = el as SortElement ?? new SortElement(this.SortNumber);

                if (SortNumber > el.SortNumber) return -1;
                if (SortNumber == el.SortNumber) return 0;
                return 1;
            }

            public static void Print(SortElement el)
            {
                Console.Write(el.SortNumber + " ");
            }
        }

        static void Main(string[] args)
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            ParallelSort();
            
            Console.ReadLine();
        }

        public static double ParallelSort()
        {
            Console.Write("Введите количество элементов: ");
            int.TryParse(Console.ReadLine(), out int count);

            Console.Write("Введите количество потоков: ");
            int.TryParse(Console.ReadLine(), out int threadsCount);
            
            Random rand = new Random(DateTime.Now.Second * DateTime.Now.Minute);
            double[] sortingArray = new double[count];
            for (int i = 0; i < count; i++)
            {
                sortingArray[i] = rand.NextDouble() * 100;
            }
            
            Sort sort = new Sort(sortingArray);

            double parallelTime = sort.RunParallelSort(threadsCount);
            double sequentialTime = sort.RunMergeSort();

            int errors = 0;
            for (int i = 1; i < sort.Array.Length; i++)
            {
                if(sort.Array[i - 1] > sort.Array[i])
                {
                    ++errors;
                }
            }
            
            Console.WriteLine((errors == 0 ? "" : "Несостыковочек: " + errors + "\r\n") +
                "parallel time: " + parallelTime / 1000.0 + "\r\n" + 
                "Sequential time: " + sequentialTime);

            return parallelTime;
        }
    }
}
