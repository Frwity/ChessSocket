# Chess
By group 3: Antony GAVELLE & Sami AMARA  
Unity version 2020.3.17f1 (LTS)

## Description
We implemented the online mode based on the template provided. To be more specific, with our implementation:

- Each player can either join or host a lobby
- The host has to give their opponent the IP address displayed on their screen
- The client will have to put the host's IP address into the dedicated field
- Both the host and the client must choose a pseudo
- Once the host and the client are connected, the host chooses when to launch the game
- Turn by turn, each player make their move, which is displayed on the opponent's screen
- A chat where the player can write messages to each others is available
- Until one of the client leaves, the game goes on


## Global architecture
The network logic is in the scripts of the `Assets\Scripts\Network` directory:
- [`ChessSerializer.cs`](Assets\Scripts\Network\ChessSerializer.cs): serialization and deserialization of incoming and outgoing packets. With choose to use the [`BinaryFormatter`](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.serialization.formatters.binary.binaryformatter?view=net-5.0) and [`MemoryStream`](https://docs.microsoft.com/en-us/dotnet/api/system.io.memorystream?view=net-5.0) C# helper classes
- [`Client.cs`](Assets\Scripts\Network\Client.cs): the client-side netcode to connect to, listen to, send to and disconnect from the host
- [`ServerHost.cs`](Assets\Scripts\Network\ServerHost.cs): the server-side netcode to connect with, listen to, send to and disconnect the client
- [`NetworkDataDispatcher.cs`](Assets\Scripts\Network\NetworkDataDispatcher.cs): the common part of the server and host. This is where packets are forwarded to be translated

The `Assets\Scripts\UI` directory has the UI-related scripts. Some of them make used of the scripts in `Assets\Scripts\Network\`.

## Known Bug

- If either player would restart a new game, and so open a new lobby, players can join and play but the board will only reset on the first move (no impact on the gameplay but only in feedback). And in some case the menu might not be properly displayed but ultimately the players can play. 

## Playing
Either launch the build available in the `Build` folder, or clone our repository and launch the project: `ssh://git@git.isartintra.com:2424/2021/GP_2023_CHESSMULTI/Groupe_03.git`

## Video

   https://youtu.be/bqGTLNNdUkM

   Or go in the `Media` folder




