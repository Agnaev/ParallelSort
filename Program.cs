using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Parallel
{
    class Program
    {
        public class SortElement : IComparable
        {
            public double SortNumber { get; private set; }
            
            public SortElement(double num)
            {
                this.SortNumber = num;
            }
            public int CompareTo(object el)
            {
                SortElement other = el as SortElement;
                return this.SortNumber.CompareTo(other.SortNumber);
            }
        }

        public static async Task Main(string[] args)
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;

            await ParallelSort();
            
            Console.ReadLine();
        }

        public static async Task<(double, double)> ParallelSort()
        {
            Console.Write("Введите количество элементов: ");
            int.TryParse(Console.ReadLine(), out int count);

            Console.Write("Введите количество потоков: ");
            int.TryParse(Console.ReadLine(), out int threadsCount);
            
            Random rand = new Random(DateTime.Now.Second * DateTime.Now.Minute);
            SortElement[] sortingArray = new SortElement[count];
            for (int i = 0; i < count; i++)
            {
                sortingArray[i] = new SortElement(rand.NextDouble() * 100);
            }
            
            Sort<SortElement> sort = new Sort<SortElement>(sortingArray);

            var result = await Task.Run(() => (sort.RunParallelSort(threadsCount), sort.RunMergeSort()));

            int errors = 0;
            for (int i = 1; i < sort.Array.Length; i++)
            {
                //if(sort.Array[i - 1] > sort.Array[i])
                if(sort.Array[i - 1].CompareTo(sort.Array[i]) == 1)
                {
                    ++errors;
                }
            }
            
            Console.WriteLine((errors == 0 ? "" : "Несостыковочек: " + errors + "\r\n") +
                "parallel time: " + result.Item1 / 1000.0 + "\r\n" + 
                "Sequential time: " + result.Item2);

            return result;
        }
    }
}
