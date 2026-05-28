# EmbeDeb

EmbeDeb is an embedded debugging and telemetry framework designed to simplify communication between embedded devices and desktop applications.

It provides:

* a lightweight embedded-side protocol,
* a desktop interpreter written in C#,
* extensible communication providers,
* and a future desktop visualization/debugging application.

## Features

* Lightweight embedded protocol
* Cross-layer communication architecture
* Extensible message system
* Serial communication support
* Debug communication providers
* Desktop-side interpreter
* WPF desktop application integration *(work in progress)*

---

## Architecture

```text
Embedded Device
       │
       │ EmbeDeb Protocol
       ▼
Communication Provider
       ▼
EmbeDeb Interpreter
       ▼
Desktop Application / Debug Tools
```

---

## Repository Structure

```text
Header/
    Embedded-side C header

EmbeDebInterpreter/
    C# protocol interpreter
    Message dispatching system
    Communication providers

EmbedebDesktop/
    WPF desktop application (experimental)

Docs/
    Protocol documentation
```

## Embedeb Communication Protocol

The protocol documentation is available here:
[Docs/Protocol](Docs/Protocol.md)

The protocol is designed to be:

* lightweight,
* extensible,
* easy to parse,
* and suitable for embedded environments.

---

## Communication Providers

Current providers include:

* Serial communication: for embededded systems
* Console communication: to directly write in a console
* Debug communication: to send communication directly in your C# code

Additional providers can easily be implemented through the communication abstraction layer.

---

## Desktop Application

The [app integration](https://github.com/mathe-man/EmbeDeb/tree/DesktopAppIntegration) branch contains the early integration work for the desktop application.

The goal of the desktop application is to provide:

* real-time telemetry visualization,
* debugging tools,
* message inspection,
* and device interaction utilities.

---

## Technologies

* C#
* .NET
* WPF
* C/C++ 17

!> The C# code base follow standard C#/.NET naming conventions by Microsoft
---

## Status

Project currently in active development.

Some components are experimental and may change significantly.

---

## Goals

* Create a lightweight embedded debugging ecosystem
* Simplify telemetry collection
* Provide reusable tooling for embedded development
* Offer extensible desktop-side integrations

---

## License

This project is licensed under the Apache 2.0 License.
