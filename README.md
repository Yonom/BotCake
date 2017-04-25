# BotCake

BotCake is a lightweight dependency injection framework for BotBits. It was built for console applications in mind but can easily be extended to support WinForms, WPF, etc.

## Why?

I'm sure you've written a bot that looks a lot like this one before:

```csharp
class Program
{
    private static BotBitsClient bot = new BotBitsClient();
    // lots of other variables
    
    static void Main()
    {
        // code that reads console arguments
        // code that initializes some variables
        // code that connects to databases
        // code that loads extensions
        
        EventLoader.Of(bot).LoadStatic<Program>();
        CommandLoader.Of(bot).LoadStatic<Program>();
        
        Login.Of(bot)
            .WithEmail("email", "pass")
            .CreateJoinRoom("roomId");  
        
        // code that listens to the console  
    }
    
    // EventListeners and Commands for handling players, blocks, chats, etc.
    
    [EventListener]
    static void On(JoinCompleteEvent e) 
    {
        Chat.Of(bot).Say("hello world!");
    }
}
```
The Program.cs file might be hundreds or thousands of lines long. There are a lot of things to distract you: your login details, all the variables you don't really care about, etc.

So why didn't you split your code into classes to begin with?

Well, if you try to separate your program into classes, you'll see how tedious and boring the process is:
```csharp
class HelloWorld
{
    private BotBitsClient bot;          
    // other variables
    
    public HelloWorld(BotBitsClient client)
    {
        bot = client;
        EventLoader.Of(bot).Load(this);
        CommandLoader.Of(bot).Load(this);
    }
    
    // EventListeners and Commands come here, without "static" this time please
    
    [EventListener]
    void On(JoinCompleteEvent e) 
    {
        Chat.Of(bot).Say("hello world!");
    }
}
```
and you still don't really care about BotBitsClient or EventLoader/CommandLoader here!

So what if:
- we loaded EventListeners and Commands for you and
- removed the .Of(bot) requirement so you don't need to pass BotBitsClient to HelloWorld?

Your HelloWorld class would look like this:
```csharp
class HelloWorld : BotBase
{
    [EventListener]
    void On(JoinCompleteEvent e) 
    {
        Chat.Say("hello world!");
    }
}
```

Much better!

## Download
Download from NuGet (https://www.nuget.org/packages/BotBits)

## Usage
Step 1: Create a new class and name it MyBot (or anything else) and inherit from BotBase
```csharp
using BotCake;
// ...
class MyBot : BotBase 
{
}
```

Step 2: In ```Program.cs``` add:
```csharp
BotCake.CakeSetup.WithBot(bot => new MyBot()) // don't add a semicolon yet!
```

- if you want to use CommandsManager:
```csharp
.WithCommandsExtension('!')
.ListenToConsole() // optional
```

- if you want to load any other extensions:
```csharp
.Do(bot => Extension.LoadInto(bot))
```

- if you want to change the send timer frequency:
```csharp
.WithSendTimerFrequency(300)
```

finally, login and join the room:
```csharp
.WithEmail("email", "pass")
.CreateJoinRoom("roomId");
```

## Features

BotCake automatically loads all EventListeners (and if CommandExtension is loaded, all Commands) in every BotBase. Your BotBitsClient is passed to each BotBase class in the background. 
You no longer need to use .Of(bot) to access BotBits / Commands Packages. To chat, you simply write:

```csharp
Chat.Say("hi");
```

## Help, I need to use .Of()!
BotCake does not support every class that has a .Of(bot) requirement. For SendMessages, Events, etc. you can use .Of(this) instead.

## Adding Packages from other extensions

BotBase has properties for every Package in BotBits and BotBits.Commands. You must add support for other Packages yourself.

This is very easy. For example, let's add support for PermissionManager from BotBits.Permissions.
First, create a new class and call it BotBaseEx.
Then, add this code:
```csharp
class BotBaseEx : BotBase 
{
    public PermissionManager PermissionManager => PermissionManager.Of(this);
}
```
Lastly, update your classes to use BotBaseEx instead of BotBase.

