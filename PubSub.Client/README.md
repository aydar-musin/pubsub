# PubSub.Client

## Installation
```
Install-Package PubSub.Client -Version 1.0.1
```

### using namepsaces
- PubSub.Client

## Connecting
```csharp
PubSubClient client=new PubSubClient(userId,serverUrl, Key);
```
- in async method
```csharp
await client.Start()
```
- in sync method
```csharp
client.Start().Wait();
```

## Publish
```csharp
client.Publish("put you message here. For example, JSON");
```

## Subscribe/Unsubscribe
```csharp
client.Subscribe("userId1");
client.UnSubscribe("userId1");
```

## Receive Messages
``` csharp
client.OnDataReceived=(msg)=>{
		Console.WriteLine(string.Format("received: {0}",msg));
	};
```


## Example

```csharp
PubSubClient client=new PubSubClient("userId1","http://localhost:46358/signalr","0452f94e38809f822030bad50f");
	client.Start().Wait();
	
	client.Publish("MyMessageBody");
	
	client.Subscribe("userId2");
	
	client.OnDataReceived=(msg)=>{
		Console.WriteLine(string.Format("received: {0}",msg));
	};
	
	client.Stop();
```