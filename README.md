Inferables for CLR
==================

Dependency Injection, arghh, a configuration eyesore! -even with exceptional lightweight frameworks like NInject and Castle Windsor. Hoping to change a small part of the world (or at least my world for that matter), I started working on a new solution on <a href="https://github.com/nmuhonen/Inferables-For-CLR" title="Github">Github</a>. <i>Inferables For CLR</i> is inspired by convention over configuration software approaches, providing a dependency resolution system that is based on where code is located and named relative to project namespaces, instead of a configuration mechanism. The result is easier management of standard injection patterns just by putting stuff in the right place :)

<h4>An Example:</h4>

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
            Console.Writeline("DEBUG:" + message);
        }
    }
}
```

And lets say you want to inject them into a class:

```c#
using LoggerStuff;

namespace SnarkyServiceLib
{
    public interface ISnarkyService
    {
        string GetSnarkyMessage();   
    }
    
    public class SnarkyService: ISnarkyService
    {    
        private ILogger traceLogger;
        private ILogger debugLogger;  
    
        public SnarkyService(ILogger traceLogger, ILogger debugLogger)
        {
            this.traceLogger = traceLogger;
            this.debugLogger = debugLogger;
        }
        
        public string GetSnarkyMessage()
        {
            traceLogger.Write("Thinking...");
            traceLogger.Write("Still thinking...");
            traceLogger.Write("I've got it...");   
            debugLogger.Write("Not really thinking, just doing static code");
            
            string message = "SnarkyNess!";
        
            debugLogger.Write("Created Message.");
            
            return message;
        }
    }
}
```

With Inferables for clr, dependency injection is managed by relative namespace location, requiring no need for configuration in files or code. Thus, through the powers of the Inferables library:

```c#
using Inferables;
...

var module = ModuleManager.CreateModule("~");
var snarkyService = module.Get<ISnarkyService>();

var message = snarkyService.GetSnarkyMessage();
```

results in creating a <strong>SnarkyServiceLib.SnarkyService</strong> class, injecting <strong>LoggerStuff.TraceLogger</strong> and <strong>LoggerStuff.DebugLogger</strong> as inner members. 

<h4>How it works:</h4>

With <strong>ModuleManager.CreateModule("~")</strong>, Inferables maps implementation classes to be found in the same namespace as their requested base definition. With such, it creates an implementation for 

```c#
module.Get<ISnarkyService>()
``` 


that looks like this:

```c#
return new SnarkyService(new LoggerStuff.TraceLogger(), new LoggerStuff.DebugLogger());
```

Lets break down the steps of this wondrous magic:
<ol>
	<li>Inferables looks under the namespace <strong>SnarkyServiceLib</strong> to find an implementation for <strong>ISnarkyService</strong>- finding <strong>SnarkyService</strong></li>
	<li>Next, it finds an appropriate constructor: <strong>public SnarkyService(ILogger traceLogger, ILogger debugLogger)</strong></li>
	<li>Finally, it finds appropriate injectable matches to ILogger under its relative namespace LoggerStuff and based on the chosen names of the constructor parameters: <strong>TraceLogger</strong> and <strong>DebugLogger</strong></li>
</ol>

<h4>How to override default injection patterns:</h4>

Lets say you want to inject something else besides the standard resolution pattern for testing, This can be done by adding another resolution pattern. For instance, lets you want to resolve a dependency to replace the DebugLogger. Simply put it under a new namespace resolution path:

```c#
namespace LoggerStuff.Mocks
{
    public class MockDebugLogger: ILogger
    {
        public void Write(string message)
        {
             Console.Writeline("Not really logging, just a mock :(");
        }  
    }
}
```

And change your code:

```c#
var module = ModuleManager.CreateModule("~.Mocks,~");
var snarkyService = module.Get<ISnarkyService>();

var message = snarkyService.GetSnarkyMessage();
```

With these changes, Inferables now acts like this:

<ol>
   <Li>First, look for implementation types with locations starting with target type namespace with an additional <strong>.Mocks</strong> suffix.</li>
   <Li>If it can't find it there, then look for implementation types that are found in the same namespace as the target type.</li>
</ol>

With these dependency resolution changes, Inferables creates an implementation for 

```c#
module.Get<ISnarkyService>()
``` 

that looks like this:

```c#
return new SnarkyService(new LoggerStuff.TraceLogger(), new LoggerStuff.Mocks.MockDebugLogger());
```

<h4>Enjoy</h4>

Try it out for yourself: <a href="https://github.com/nmuhonen/Inferables-for-CLR" title="https://github.com/nmuhonen/Inferables-for-CLR">https://github.com/nmuhonen/Inferables-for-CLR</a> and happy coding :)