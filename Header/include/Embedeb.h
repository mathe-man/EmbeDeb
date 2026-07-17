/*
 * Copyright 2026 Mathieu Mousset
 * Project: EmbeDeb (Embedded Debugger)
 * Repository: https://github.com/mathe-man/EmbeDeb
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */



#pragma once
#include <cstddef>
#include <cstdint>
#include <cstring>
#include <vector>
#pragma region Defines


#define EmbedDeb_MagicNumber "\xEB\xDB" // Magic number to identify EmbedDeb messages: 0xEBDB
#define EmbedDeb_Version "1.0"             // Version of the EmbedDeb protocol, can be used for compatibility checks
#define BoardName "UndefinedName"       // Name of the board


#define MessageSeparator "|"            // Separator between messages
#define TypeContentSeparator "="        // Key-Value separator between the type and the content of a message


#define MessagesBufferSize 512          // Size of the buffer to hold the messages before flushing, can be changed depending of the needs


 // Those types can be changed depending of the needs
typedef uint32_t UnsignedInt;           // Type for unsigned integers, used for sizes and counts
typedef uint32_t TimeType;              // Type for time values, used for timestamps and durations

#pragma endregion

class Buffer {
public:
    Buffer(uint32_t size) : m_size(size)
    {
        m_buffer = new std::byte[size];
        m_cursor = 0;
    }

    [[nodiscard]]
    bool inline FitInBuffer(uint16_t size) const {
        return size <= m_size - m_cursor;
    }

    void inline ClearBuffer() {
        m_cursor = 0;
    }

    template<typename T>
    bool Append(const T& object)
    {
        static_assert(std::is_trivially_copyable_v<T>,
            "Type must be trivially copyable");


        // First check available size
        if (!FitInBuffer(sizeof(object)))
            return false;

        memcpy(m_buffer + m_cursor, &object, sizeof(object));

        m_cursor += sizeof(object);

        return true;
    }

    // char arrays overload
    bool inline Append(const char* str) {
        if (!FitInBuffer(strlen(str)))
            return false;

        memcpy(m_buffer + m_cursor, str, strlen(str));

        m_cursor += strlen(str);

        return true;
    }


private:

    uint32_t m_size;

    std::byte *m_buffer;
    uint16_t m_cursor;

    template<typename T>
    static const std::byte* ToBytes(const T& object){
        return reinterpret_cast<const std::byte*>(&object);
    }
};



#pragma region Core

class Event;

struct EmbedDebMessage {
    const char* type;
    const char* content;

    EmbedDebMessage(const char* type, const char* content) : type(type), content(content) {}

    UnsignedInt inline Length() const {
        return strlen(type) + strlen(content) + strlen(MessageSeparator) + strlen(TypeContentSeparator);
    }
    const char* Build() const {
        char* message = new char[strlen(type) + strlen(content) + strlen(TypeContentSeparator) + strlen(MessageSeparator) + 1]; // +1 for null terminator

        strcpy(message, type);
        strcat(message, TypeContentSeparator);  
        strcat(message, content);
        strcat(message, MessageSeparator);

        return message;
    }
};

using WriteFunction = void(*)(const char*);
using TimeFunction = TimeType(*)();

class EmbedDeb {
public:

    static void Init(WriteFunction writeFunc, TimeFunction timeFunc) {
        setWriteFunction(writeFunc);
        setTimeFunction(timeFunc);
    }

    static void setWriteFunction(WriteFunction func) {
        writeFunction = func;
    }
    
    static void setTimeFunction(TimeFunction func) {
        timeFunction = func;
    }

    static inline bool Flush() {
        return FlushBuffer();
    }

    static inline bool Log(EmbedDebMessage message)
    {
        return LogMessage(message);
    }

    static inline bool print(const char* value) {
        if (!writeFunction)
            return false; // No write function set, cannot print
        
        writeFunction(value);
        return true;
    }

    static inline bool println(const char* value) {
        if (!writeFunction)
            return false; // No write function set, cannot print
        
        writeFunction(value);
        writeFunction("\r\n");  // Finish the line with a newline and carriage return

        return true;
    }

    static inline TimeType GetTime() {
        if (!timeFunction)
            return 0; // No time function set, cannot get time
        
        return timeFunction();
    }


private:

    // Function pointer for writing messages, use Serial.print by default
    static inline WriteFunction writeFunction;
    static inline TimeFunction timeFunction;

    static inline char eventsMessagesBuffer[MessagesBufferSize] = ""; // Buffer to hold the messages before flushing

    static inline bool FitInBuffer(EmbedDebMessage message) {
        return strlen(eventsMessagesBuffer) + message.Length() + 1 < MessagesBufferSize; // +1 for null terminator
    }

    static inline UnsignedInt EmptyBufferSpace() {
        return MessagesBufferSize - 1; // -1 for null terminator
    }

    static inline void ClearBuffer() {
        eventsMessagesBuffer[0] = '\0';
    }

    static inline void AddToBuffer(EmbedDebMessage message) {
        strcat(eventsMessagesBuffer, message.Build());   // The message build already include separator
    }


    // Event class can have access to logging
    friend class Event;

#define MaxLogAttempt 5

    static inline bool LogMessage(EmbedDebMessage message, uint8_t attempt = 0) {
        if (attempt >= MaxLogAttempt) {
            return false;       // Max log attempts reached, give up
        }
        // Check if the message size is acceptable
        if (message.Length() > EmptyBufferSpace())
            return false;

        // Check if the message can fit in the buffer
        if (!FitInBuffer(message)) {
            FlushBuffer();
            return LogMessage(message, attempt++); // Try to log the message again after flushing the buffer
        }

        // All the test passed => Add the message to the buffer and return true
        AddToBuffer(message);
        return true;
    }

    static inline bool FlushBuffer()
    {
        if (strlen(eventsMessagesBuffer) == 0) {
            return false; // Buffer is empty, no need to flush
        }
        // Send the serial communication with the format: MagicNumber|BoardName|message1|message2|...|messageN (Assuming the separator is '|')
        print(EmbedDeb_MagicNumber); print(MessageSeparator);
        print(BoardName); print(MessageSeparator);
        println(eventsMessagesBuffer);
        ClearBuffer();
        return true;
    }
};

#pragma endregion 


#pragma region Messages


class TextMessage {
public:
    char* text;
    TextMessage(char* text) : text(text) {}
    TextMessage(const char* text) {
        this->text = new char[strlen(text) + 1];
        strcpy(this->text, text);
	}

    void push() {
        EmbedDebMessage message("Txt", text);
		EmbedDeb::Log(message);
    }
};


#pragma endregion

