using System;
using System.Threading.Tasks;

namespace pifromrandom
{
    class Program
    {
        public delegate void myDelegate();

        public static void Main(string[] args)
        {
            UInt64 MAX;
            UInt64 TOTAL;
            bool useParallel;

            Console.Write("Enter the Max size for the random numbers : ");
            MAX = UInt64.Parse(Console.ReadLine());
            //MAX = 1200;
            Console.Write("Enter the Total amount of random numbers : ");
            TOTAL = UInt64.Parse(Console.ReadLine());
            //TOTAL = 1000000000;
            Console.Write("Do you want to use parallel processing (True/False) : ");
            useParallel = bool.Parse(Console.ReadLine());
            //useParallel = false;

            Console.WriteLine(useParallel);

            UInt64 COUNT;
            if(useParallel)
                COUNT = pifromrandom.Program.COUNTRANDOMDIVISORSPARALLEL(TOTAL, MAX);
            else
				COUNT = pifromrandom.Program.COUNTRANDOMDIVISORS(TOTAL, MAX);

			Console.WriteLine(COUNT);
            double COUNTPERCENT = (double)COUNT / TOTAL;
            Console.WriteLine(COUNTPERCENT);

            double Pi = Math.Sqrt(6 / COUNTPERCENT);

            Console.Write("Pi calculated from random numbers is: " + Pi);
        }



        public static UInt64 GCD(UInt64 num1, UInt64 num2)
        {
            UInt64 Remainder;

            while (num2 != 0)
            {
                Remainder = num1 % num2;
                num1 = num2;
                num2 = Remainder;
            }

            return num1;
        }


        public static UInt64 COUNTRANDOMDIVISORSPARALLEL(UInt64 TOTAL, UInt64 MAX)
        {
            UInt64 COUNT = 0;

            UInt64 degreeOfParallelism = (UInt64)Environment.ProcessorCount;

            Task[] tasks = new Task[degreeOfParallelism];


            for (UInt64 taskNumber = 0; taskNumber < degreeOfParallelism; taskNumber++)
            {
                // capturing taskNumber in lambda wouldn't work correctly
                var taskNumberCopy = (UInt64)taskNumber;

				int seed = DateTime.Now.Millisecond;

				tasks[taskNumber] = Task.Factory.StartNew(() =>
                {
                    var max = TOTAL * (taskNumberCopy + 1) / degreeOfParallelism;

                    // using a copy as it wouldn't work assigning directly 
                    UInt64 CountCopy = 0;

                    Random RNG = new Random(seed);

                    for (UInt64 i = TOTAL * taskNumberCopy / degreeOfParallelism; i < max; i++)
                    {
                        UInt64 A = (UInt64)(RNG.NextDouble() * MAX);
                        UInt64 B = (UInt64)(RNG.NextDouble() * MAX);
                        if (pifromrandom.Program.GCD(A, B) == 1)
                        {
                            CountCopy++;
                        }
                    }

                    COUNT += CountCopy;

                });

            }

            Task.WaitAll(tasks);

            return COUNT;
        }

        public static UInt64 COUNTRANDOMDIVISORS(UInt64 TOTAL, UInt64 MAX)
        {
            UInt64 COUNT = 0;

            Random RNG = new Random();

            for (UInt64 i = 0; i < TOTAL; i++)
            {
                UInt64 A = (UInt64)(RNG.NextDouble() * MAX);
                UInt64 B = (UInt64)(RNG.NextDouble() * MAX);
                if (pifromrandom.Program.GCD(A, B) == 1)
                {
                    COUNT++;
                }
            }

            return COUNT;
        }
    }
}
