Inferables-for-CLR
==================

Dependecy Injection. Its been around for a while and generally requires some sort of configuration. Inspired by rails and other convention based approaches, Inferables creates a depency resolution system that is based on where code is located relative to project namespaces. Lets look at an example.

Lets say your have some injectable classes defined somewhere- a set of loggers for instance:

```c#
namespace LoggerStuff
{
    public interface ILogger
    {
        void WriteMessage(string message);
    }

    // A trace logger
    public class TraceLogger: ILogger
    {
        public void WriteMessage(string message)
        {
            Console.Writeline(message);
        }
    }
    
    // A debug logger
    public class DebugLogger: ILogger
    {
        public void WriteMessage(string message)
        {
            Console.Writeline(message);
        }
    }
}

```

And lets say you want to inject them into a class

```c#
using LoggerStuff;

namespace SnarkyServiceLib
{
    public interface ISnarkyService
    {
        string GetSnarkyMessage();   
    }
    
    public class SnarkyService
    {    
        private ILogger traceLogger;
        private ILogger debugLogger;  
    
        public SnarkyService(ILogger traceLogger, ILogger debugLogger)
        {
            this.traceLogger = traceLogger;
            this.consoleLogger = debugLogger;
        }
        
        public string GetSnarkyMessage()
        {
            traceLogger.Write("Thinking...");
            traceLogger.Write("Still thinking...");
            traceLogger.Write("I've got it...");   
            debugLogger.Write("Not really thinking, just doing static code");
            
            string message = "SnarkyNess!";
        
            debugLogger.Write("Created Message);
            
            return message;
        }
    }
}
```

With Inferables for clr, dependcy injection is managed my relative namespace location. So in the following code:

```c#
var module = ModuleManager.CreateModule();
var snarkyService = module.Get<ISnarkyService>();

var message = snarkyService.GetSnarkyMessage();
```

Inferables creates an implementation for `module.Get<ISnarkyService>()` that looks like this:

```c#
return new SnarkyService(new LoggerStuff.TraceLogger(), new LoggerStuff.DebugLogger());
```

Try it out for yourself!