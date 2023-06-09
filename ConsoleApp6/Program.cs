
using System;

internal class Program
{
   static int a = 1;
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        while (true)
        {
            Add();
            Console.WriteLine(a);
        }
    }
   static void Add()
    {
        a++;
    }
}

