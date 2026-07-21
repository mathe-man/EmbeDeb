# Embedeb communication protocol
The communication protocol is based on a simple message format that consists of multiple "message"

# Definitions
- **Communication**: The whole string sent by the sender to the receiver.
- **Message**: A part of the communication that contains a type and maybe other information. Messages are separated by a separator (e.g., `|`).
- **Message Type**: The first part of a message, it's separated from the message information by a Key-Value separator (e.g., `=`).
- **Message's time information**: It is placed automaticly after the type of the message and provide information about the time this message were raised/created.
- **Embedeb Protocol Magic bytes**: Two bytes that identifies the communication as being part of the Embedeb protocol. This is used to ensure that the receiver can recognize and properly handle the communication.
- **Sender's Identifier**: A unique identifier for the sender of the communication. This is used to identify who sent the communication and can be used for routing or handling purposes.

# Header
The first part of a communication is the header, composed of the magic bytes, directly followed by the length of the communication (represented by an 8 bytes unsigned integer, the `size_t` type in C++), then the sender's identifier ending by a message separator.
This should look like this:
```Embedeb Communication
MagicNumberLengthSenderIdentifier|Message1|Message2|...
```

A communication must start with the Embedeb protocol magic bytes and can end with a message separator or not.

# Message Format
A message consists of a message type, a time information and other information, separated by a separator.
>The way time is represented can be changed as you wich, but for an easy use u can directly use methods like `millis()`on your embedded device

It can look like this:
```Embedeb Message
Type,Time=OtherInformation
```
## Message Types
But a message's type part can contain multiple types ! That don't necessarily have to be separated by a separator, but can be separated by a different character (e.g., `,`).
The last element of the Type side of a message will alway be the time information preceded by a comma (`,`)
```Embedeb Message
Type1,Type2,Type3,Time=OtherInformation
```

```Embedeb Message
Type1Type2-Type3,Time=OtherInformation
```
In both of the above examples, the message has three types: `Type1`, `Type2`, and `Type3`. And they'll be properly interpreted

## Message Information
The information part is all the content following the separator in a message. It can contain any kind of information, and its format is not strictly defined by the protocol. It can be a simple string, a JSON object, or any other format that the sender and receiver agree upon.

# Reserved characters
The protocol uses specific characters as separators, and these characters should not be used in the message types or informations unless they are properly escaped or handled by the sender and receiver. The reserved characters include:
- `|`: Used as a message separator.
- `=`: Used as a key-value separator in messages.
- `,`: Used as a separator in messages.

> Note that those can be changed by the sender and receiver as long as they agree on the new characters to use.

# Example Communication
Here are several examples of communications that follow the Embedeb protocol:

> Note that those are directly represented in a readable way, in fact the protocol use raw bytes for numbers and the magic bytes

```Embedeb Communication
EBDB44MyArduino|Ping=OutOfMemory|Memory=8978
```

```Embedeb Communication
EBDB47SensorNode1|Temperature=25.5|Humidity=60|
```
> Note that the last message ends with a separator, which is allowed by the protocol.

```Embedeb Communication
EBDB56ControllerSV|Stick1=0.89x0.65y|Button=A*B*Super*L2
```
>!The stick values are represented as `x` and `y` coordinates, and the button values are separated by `*`.

```Embedeb Communication
EBDB50DeviceX|Status=OK|Battery=85%|Location=Room1
```

```Embedeb Communication
EBDB53DeviceY|Error=SensorFailure|Code=1234|Retry=true
```

```Embedeb Communication
EBDB53DeviceZ|Data={"temperature":22.5,"humidity":55}
```
> In this example, the information part of the last message is a JSON object containing temperature and humidity data.

```Embedeb Communication
EBDB48EDevice|Debug,Sensor=Value1,Value2,Value3|
```
> In this example, the message has multiple types (`Debug` and `Sensor`) separated by a comma, and the information part contains multiple values also separated by a comma. The message ends with a separator, which is allowed by the protocol.