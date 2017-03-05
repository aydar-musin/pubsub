# PubSub.Client

## Installation
```
Install-Package PubSub.Client -Version 1.0.1
```
### using namepsaces
- PubSub.Client
- GeoPositionStorage.Service


## Connecting
```csharp
GeoPubSubClient client=new GeoPubSubClient(userId,serverUrl, Key);
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
var user=new User(){ Id="userId1", Lat= 5.0, Lng= 5.0};
client.Publish(user);
```

## Subscribe/Unsubscribe
```csharp
client.Subscribe("userId1");
client.UnSubscribe("userId1");
```

## Receive Messages
``` csharp
client2.OnDataReceived=(user)=>{
		Console.WriteLine(string.Format("received: {0} {1} {2}",user.Id, user.Lat, user.Lng));
	};
```


## Example

```csharp
GeoPubSubClient client=new GeoPubSubClient("userId1","http://localhost:46358/signalr","0452f94e38809f822030bad50f");
	client.Start().Wait();
	
	client.Publish(new User(){ Id="userId1", Lat= 5.0, Lng= 5.0});
	
	client.Subscribe("userId2");
	
	client.OnDataReceived=(user)=>{
		Console.WriteLine(string.Format("received: {0} {1} {2}",user.Id, user.Lat, user.Lng));
	};
	
	client.Stop();
```