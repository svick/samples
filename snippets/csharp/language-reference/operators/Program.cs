﻿using System;

namespace operators
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("============== % operator examples =============");
            RemainderExamples.Examples();
            Console.WriteLine();

            Console.WriteLine("============== + operator examples =============");
            AdditionExamples.Examples();
            Console.WriteLine();

            Console.WriteLine("============== & operator examples =============");
            AndOperatorExamples.Examples();
            Console.WriteLine();

            Console.WriteLine("============== = operator examples =============");
            AssignmentExamples.Examples();
            Console.WriteLine();
            
            Console.WriteLine("============== ?: operator examples ============");
            ConditionalExamples.Examples();
            Console.WriteLine();

            Console.WriteLine("============== ~ operator examples =============");
            BitwiseComplementExamples.Examples();
            Console.WriteLine();

            Console.WriteLine("============== && and || operator examples =====");
            ConditionalLogicalOperatorsExamples.Examples();
            Console.WriteLine();

            Console.WriteLine("============== -- and ++ operator examples =====");
            DecrementAndIncrementExamples.Examples();
            Console.WriteLine();
        }
    }
}
