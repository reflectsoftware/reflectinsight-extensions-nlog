using System;
using Newtonsoft.Json.Linq;
using NLog;

namespace ReflectInsight.Extensions.NLog.Sample
{
    class Program
    {
        static Logger log = LogManager.GetLogger("ReflectInsight");

        static void Main(string[] args)
        {        
            while (true)
            {
                Console.WriteLine("Press any key or 'q' to quit...");

                ConsoleKeyInfo k = Console.ReadKey();
                if (k.KeyChar == 'q')
                {
                    break;
                }

                TestTrace();

                Exception ex = new Exception("This is my test exception!");

                log.Info("[Enter] My Info1");

                log.Trace("My Trace");
                log.Trace(ex, "Trace Exception");

                log.Info("My Info");
                log.Info(ex, "Info Exception");

                log.Debug("My Debug");
                log.Debug(ex, "Debug Exception");

                log.Warn("My Warn");
                log.Warn(ex, "Warn Exception");

                log.Error("My Error");
                log.Error(ex, "Error Exception");

                log.Fatal("My Fatal");
                log.Fatal(ex, "Fatal Exception");

                DoDomething();
                
                log.Info("[Exit] My Info1");
            }
        }

        static void DoDomething()
        {
            log.Info("[Enter] DoSomething");
            log.Info("Something happened!");
            log.Info("[Exit] DoSomething");
        }

        static void TestTrace()
        {
             Logger _logger = LogManager.GetCurrentClassLogger();

        dynamic t = JObject.Parse(@"{
  ""price"": 11190.0,
  ""amount"": 0.43422189,
  ""datetime"": ""1517049417"",
  ""id"": 844079167,
  ""order_type"": 0
}");
        _logger.Trace(t);
        }
    }
}
