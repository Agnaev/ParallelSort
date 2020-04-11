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

        public static void Main(string[] args)
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;

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
            SortElement[] sortingArray = new SortElement[count];
            for (int i = 0; i < count; i++)
            {
                sortingArray[i] = new SortElement(rand.NextDouble() * 100);
            }
            
            Sort<SortElement> sort = new Sort<SortElement>(sortingArray);

            double parallelTime = sort.RunParallelSort(threadsCount);
            double sequentialTime = sort.RunMergeSort();

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
                "parallel time: " + parallelTime / 1000.0 + "\r\n" + 
                "Sequential time: " + sequentialTime);

            return parallelTime;
        }
    }
}
