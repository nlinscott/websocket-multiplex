### websocket-multiplex

A server side solution to creating multiplexed web socket channels. 


## solution

This project removes the need for clients to be equipped with identifiers to identify message types. 

Channels in this project are defined arbitrarily based on the order `IChannel` implementations are introduced. 

For example, if you need a Chat service you would implement `IChannel` as such. Your server side implementation would then add this as the `first` implementation.

At runtime, the library uses this order to assign short, arbitrary identifiers to the channel that are included at the beginning of all messages. This can be overridden with a custom implementation but by default uses each two letter combination of the alphabet (26 ^ 2 unique combinations).

Clients will always receive a JSON array of all identifiers upon connecting. This can be cached and referenced later. You can then associate client side functionality with an array index rather than having to know exactly what a channel is called or have to worry about passing around these unique identifiers.
