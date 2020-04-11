using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;
using System.Diagnostics;

namespace Parallel
{
    public class Sort
    {
        public double[] SourceArray { get; private set; }
        public  double[] Array { get; private set; }
        private List<Thread> Threads { get; set; } = new List<Thread>();
        public Stopwatch ParallelTime { get; private set; } = new Stopwatch();
        public Stopwatch SequentialTime { get; private set; } = new Stopwatch();

        private void MergeInPlace(double[] a, int left, int middle, int high)
        {
            int right = middle + 1;
            while (left <= middle && right <= high)
            {
                if (a[left] <= a[right])
                {
                    left++;
                }
                else
                {
                    double temp = a[right];
                    System.Array.Copy(a, left, a, left + 1, right - left);
                    a[left] = temp;
                    left++;
                    right++;
                    middle++;
                }
            }
        }
        
        private void CreateThread(int left, int right)
        {
            this.Threads.Add(new Thread(() => MergeSort(this.Array, left, right)));
        }

        private void MergeSort(double[] a, int left, int right)
        {
            if (right > left)
            {
                int middle = (left + right) / 2;
                this.MergeSort(a, left, middle);
                this.MergeSort(a, middle + 1, right);
                this.MergeInPlace(a, left, middle, right);
            }
        }

        public double RunMergeSort()
        {
            this.Array = this.SourceArray.Clone() as double[];
            SequentialTime.Start();
            MergeSort(Array, 0, Array.Length - 1);
            SequentialTime.Stop();
            return SequentialTime.ElapsedMilliseconds / 1e3;
        }

        public double RunParallelSort(int threadsCount)
        {
            int end, begin = 0;
            for (int i = 0; i < threadsCount; i++)
            {
                end = (i + 1) * this.Array.Length / threadsCount + (i == threadsCount - 1 ? -1 : 0);
                this.CreateThread(begin, end);
                begin = end + 1;
            }

            ParallelTime.Start();

            this.Threads.ForEach(x => x.Start());
            this.Threads.ForEach(x => x.Join());

            this.MergeSort(this.Array, 0, this.Array.Length - 1);
            ParallelTime.Stop();
            return ParallelTime.ElapsedMilliseconds / 1e3;
        }

        public Sort(double[] Array)
        {
            this.Array = Array;
            this.SourceArray = this.Array.Clone() as double[];
        }
    }
}
