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

// Choosing the board name to use
#ifndef EMBEDDEB_BOARD_NAME
#define EMBEDDEB_BOARD_NAME "UndefinedName"

#if defined(__GNUC__) || defined(__clang__)
#warning "EmbedDeb: EMBEDDEB_BOARD_NAME not defined. Using default value."
#elif defined(_MSC_VER)
#pragma message("WARNING: EmbedDeb: EMBEDDEB_BOARD_NAME not defined. Using default value.")
#endif

#endif


#define MessageSeparator "|"            // Separator between messages
#define TypeContentSeparator "="        // Key-Value separator between the type and the content of a message


#define MessagesBufferSize 512          // Size of the buffer to hold the messages before flushing, can be changed depending of the needs


 // Those types can be changed depending of the needs
typedef uint32_t UnsignedInt;           // Type for unsigned integers, used for sizes and counts
typedef uint32_t TimeType;              // Type for time values, used for timestamps and durations

#pragma endregion

class Buffer {
public:
    Buffer(size_t size) : m_size(size)
    {
        m_buffer = new std::byte[size];
        m_cursor = 0;
    }
    ~Buffer() {
        delete m_buffer;
    }

    [[nodiscard]]
    bool inline FitInBuffer(uint16_t size) const {
        return size <= m_size - m_cursor;
    }

    void inline ClearBuffer() {
        m_cursor = 0;
    }

    [[nodiscard]]
    size_t Length() const {
        return m_cursor;
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

    bool inline Append(const Buffer &buffer) {
        if (!FitInBuffer(buffer.Length()))
            return false;

        memcpy(m_buffer + m_cursor, buffer.m_buffer, buffer.Length());

        m_cursor += buffer.Length();

        return true;
    }

    void inline CopyTo(void *dest, const size_t size) const {
        memcpy(dest, m_buffer, size);
    }

friend Buffer; // Allow access to private members of between multiple buffer (Useful to append a buffer in another)
private:

    size_t m_size;

    std::byte *m_buffer;
    size_t m_cursor;

    template<typename T>
    static const std::byte* ToBytes(const T& object){
        return reinterpret_cast<const std::byte*>(&object);
    }
};



class Message {
public:
    Message(const char* type, const char* content)
    {
        // Size of the message:
        // Type + Content + Type-Content separator and Messages separator
        size_t size =
            strlen(type) + strlen(content) + strlen(TypeContentSeparator) + strlen(MessageSeparator) ;

        m_buffer = new Buffer(size);

        m_buffer->Append(type);
        m_buffer->Append(TypeContentSeparator);
        m_buffer->Append(content);
        m_buffer->Append(MessageSeparator);
    }

    [[nodiscard]]
    size_t inline Length() const {
        return m_buffer->Length();
    }

    bool CopyInto(Buffer& dest) const {
        return dest.Append(*m_buffer);
    }

private:
    Buffer* m_buffer;
};

using WriteFunction = void(*)(const char*, size_t);
using TimeFunction = TimeType(*)();

class EmbedDeb {
public:

    static void Init(const WriteFunction writeFunc, const TimeFunction timeFunc) {
        setWriteFunction(writeFunc);
        setTimeFunction(timeFunc);

        m_buffer = new Buffer(MessagesBufferSize);
    }

    static void setWriteFunction(const WriteFunction func) {
        writeFunction = func;
    }
    
    static void setTimeFunction(const TimeFunction func) {
        timeFunction = func;
    }

    static inline bool Flush() {
        return FlushBuffer();
    }

    static inline bool Log(const Message message)
    {
        return LogMessage(message);
    }

    static inline TimeType GetTime() {
        if (!timeFunction)
            return 0; // No time function set, cannot get time
        
        return timeFunction();
    }


private:

    // Function pointer to write messages/communications
    static inline WriteFunction writeFunction;
    static inline TimeFunction timeFunction;

    static inline Buffer* m_buffer;



    static inline bool LogMessage(const Message message) {
        // If it fit then we directly return true
        if (message.CopyInto(*m_buffer)) {
            return true;
        }
        // Otherwise we flush then try again

        FlushBuffer();
        return message.CopyInto(*m_buffer);

    }

    static inline void print(const char* write) {
        writeFunction(write, strlen(write));
    }

    static inline void print(const Buffer& buffer) {
        char *dest = new char[buffer.Length() + 1];

        // Copy raw buffer bytes
        buffer.CopyTo(dest, buffer.Length());

        writeFunction(dest, buffer.Length());
    }

    static inline bool FlushBuffer()
    {
        if (m_buffer->Length() == 0) {
            return false; // Buffer is empty, no need to flush
        }
        // Send the serial communication with the format: MagicNumber|BoardName|message1|message2|...|messageN (Assuming the separator is '|')

        Buffer header(50);
        header.Append(EmbedDeb_MagicNumber);
        // Length of the communication
        header.Append(
            m_buffer->Length()  // Every messages
            + strlen(EmbedDeb_MagicNumber)  // Magic number
            + strlen(MessageSeparator)*2    // Two separator
            + strlen(EMBEDDEB_BOARD_NAME)   // Board name
            + sizeof(size_t)                    // The length counter (the one actually calculated)
        );

        header.Append(MessageSeparator);
        header.Append(EMBEDDEB_BOARD_NAME);
        header.Append(MessageSeparator);

        print(header);
        print(*m_buffer);
        m_buffer->ClearBuffer();
        return true;
    }
};

