# Embedeb communication protocol
The communication protocol is based on a simple message format that consists of multiple "message"

# Definitions
- **Communication**: The whole string sent by the sender to the receiver.
- **Message**: A part of the communication that contains a type and maybe other information. Messages are separated by a separator (e.g., `|`).
- **Message Type**: The first part of a message, it's separated from the message information by a Key-Value separator (e.g., `=`).
- **Embedeb Protocol Magic String**: A specific string that identifies the communication as being part of the Embedeb protocol. This is used to ensure that the receiver can recognize and properly handle the communication.
- **Sender's Identifier**: A unique identifier for the sender of the communication. This is used to identify who sent the communication and can be used for routing or handling purposes.

# Communication Format
A communication consists of multiple messages, each separated by a separator.
The first two messages of a communication are reserved for the Embedeb protocol magic string, and the sender's identifier.
This should look like this:
```Embedeb Communication
MagicNumber|SenderIdentifier|Type=OtherInformation|Type=OtherInformation|...
```

A communication must start with the Embedeb protocol magic string and can end with a message separator or not.

# Message Format
A message consists of a message type and optionally other information, separated by a separator.
It can look like this:
```Embedeb Message
Type=OtherInformation
```
## Message Types
But a message's type part can contain multiple types ! That don't necessarily have to be separated by a separator, but can be separated by a different character (e.g., `,`).
```Embedeb Message
Type1,Type2,Type3=OtherInformation
```

```Embedeb Message
Type1Type2-Type3=OtherInformation
```
In both of the above examples, the message has three types: `Type1`, `Type2`, and `Type3`. And they'll be properly interpreted

## Message Information
The information part is all the content following the separator in a message. It can contain any kind of information, and its format is not strictly defined by the protocol. It can be a simple string, a JSON object, or any other format that the sender and receiver agree upon.

# Reserved characters
The protocol uses specific characters as separators, and these characters should not be used in the message types or informations unless they are properly escaped or handled by the sender and receiver. The reserved characters include:
- `|`: Used as a message separator.
- `=`: Used as a key-value separator in messages.
- `,`: Used as a types and values separator in messages (optional).

>!Note that those can be changed by the sender and receiver as long as they agree on the new characters to use.

# Example Communication
Here are several examples of communications that follow the Embedeb protocol:
```Embedeb Communication
EBDB|MyArduino|Ping=OutOfMemory|Memory=8978
```

```Embedeb Communication
EBDB|SensorNode1|Temperature=25.5|Humidity=60|
```
>!Note that the last message ends with a separator, which is allowed by the protocol.

```Embedeb Communication
EBDB|ControllerSV|Stick1=0.89x0.65y|Button=A*B*Super*L2
```
>!The stick values are represented as `x` and `y` coordinates, and the button values are separated by `*`.

```Embedeb Communication
EBDB|DeviceX|Status=OK|Battery=85%|Location=Room1
```

```Embedeb Communication
EBDB|DeviceY|Error=SensorFailure|Code=1234|Retry=true
```

```Embedeb Communication
EBDB|DeviceZ|Data={"temperature":22.5,"humidity":55}
```
>!In this example, the information part of the last message is a JSON object containing temperature and humidity data.

```
EBDB|EDevice|Debug,Sensor=Value1,Value2,Value3|
```
>!In this example, the message has multiple types (`Debug` and `Sensor`) separated by a comma, and the information part contains multiple values also separated by a comma. The message ends with a separator, which is allowed by the protocol.