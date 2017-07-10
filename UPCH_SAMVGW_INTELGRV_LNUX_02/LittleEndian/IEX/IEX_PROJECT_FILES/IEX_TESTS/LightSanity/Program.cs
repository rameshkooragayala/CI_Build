using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.Tests.Utils;

class Program
{
    static void Main(string[] args)
    {
        var scheduler = new TestsScheduler();
        try
        {
            scheduler.ParseArgs(args);
            scheduler.ExecuteTests();
            scheduler.SendResults();     //Extension method in IEX.Tests.Utils class
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Occurred While Executing The Program: " + ex.Message);
        }
    }
}