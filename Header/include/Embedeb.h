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


#pragma region Expressions


#define EMBEDEB_MAGIC "\xEB\xDB" // Magic number to identify EmbeDeb messages: 0xEBDB
#define EMBEDEB_VERSION "1.0"             // Version of the EmbeDeb protocol, can be used for compatibility checks

// Choosing the board name to use
#ifndef EMBEDEB_BOARD_NAME
#define EMBEDEB_BOARD_NAME "UndefinedName"

#if defined(__GNUC__) || defined(__clang__)
#warning "EmbeDeb: EMBEDEB_BOARD_NAME not defined. Using default value."
#elif defined(_MSC_VER)
#pragma message("WARNING: EmbeDeb: EMBEDEB_BOARD_NAME not defined. Using default value.")
#endif

#endif


constexpr char MessageSeparator =  *"|";           // Separator between messages
constexpr char TypeContentSeparator = *"=";        // Key-Value separator between the type and the content of a message

constexpr size_t MessagesBufferSize = 128;          // Size of the buffer to hold the messages before flushing, can be changed depending of the needs


 // Those types can be changed depending on the needs
typedef uint32_t TimeType;              // Type for time values, used for timestamps and durations

#pragma endregion

namespace functions {
    using WriteFunction = void(*)(const char*, size_t);
    using TimeFunction = TimeType(*)();

    inline WriteFunction writeFunction = nullptr;
    inline TimeFunction timeFunction = nullptr;

    inline TimeType GetTime()
    {
        if (!timeFunction)
            return 0;

        return timeFunction();
    }

    inline void Write(const char* str, size_t len) {
        if (!writeFunction)
            return;

        writeFunction(str, len);
    }
}

class Buffer {
public:
    explicit Buffer(size_t size) : m_size(size)
    {
        m_buffer = new std::byte[size];
        m_cursor = 0;
    }
    ~Buffer() {
        delete m_buffer;
    }

    [[nodiscard]]
    bool FitInBuffer(uint16_t size) const {
        return size <= m_size - m_cursor;
    }

    void ClearBuffer() {
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
    bool Append(const char* str) {
        if (!FitInBuffer(strlen(str)))
            return false;

        memcpy(m_buffer + m_cursor, str, strlen(str));

        m_cursor += strlen(str);

        return true;
    }

    bool Append(const Buffer &buffer) {
        if (!FitInBuffer(buffer.Length()))
            return false;

        memcpy(m_buffer + m_cursor, buffer.m_buffer, buffer.Length());

        m_cursor += buffer.Length();

        return true;
    }

    void CopyTo(void *dest, const size_t size) const {
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
            strlen(type) + strlen(content) + sizeof(TypeContentSeparator) + sizeof(MessageSeparator)
        + sizeof(TimeType) + sizeof(','); // We may also add information about the timestamp

        m_buffer = new Buffer(size);

        m_buffer->Append(type);

        if (const auto time = functions::GetTime(); time != 0) {
            m_buffer->Append(',');
            m_buffer->Append(time);
        }

        m_buffer->Append(TypeContentSeparator);
        m_buffer->Append(content);
        m_buffer->Append(MessageSeparator);
    }

    [[nodiscard]]
    size_t Length() const {
        return m_buffer->Length();
    }

    bool CopyInto(Buffer& dest) const {
        return dest.Append(*m_buffer);
    }

private:
    Buffer* m_buffer;
};


class EmbeDeb {
public:

    static void Init(const functions::WriteFunction writeFunc, const functions::TimeFunction timeFunc) {
        setWriteFunction(writeFunc);
        setTimeFunction(timeFunc);

        m_buffer = new Buffer(MessagesBufferSize);
    }

    static void setWriteFunction(const functions::WriteFunction func) {
        functions::writeFunction = func;
    }
    
    static void setTimeFunction(const functions::TimeFunction func) {
        functions::timeFunction = func;
    }

    static bool Flush() {
        return FlushBuffer();
    }

    static bool Log(const Message message)
    {
        return LogMessage(message);
    }


private:
    static inline Buffer* m_buffer;


    static bool LogMessage(const Message message) {
        // If it fit then we directly return true
        if (message.CopyInto(*m_buffer)) {
            return true;
        }

        // Otherwise we flush then try again

        FlushBuffer();
        return message.CopyInto(*m_buffer);

    }

    static void print(const char* write) {
        functions::Write(write, strlen(write));
    }

    static void print(const Buffer& buffer) {
        char *dest = new char[buffer.Length() + 1];

        // Copy raw buffer bytes
        buffer.CopyTo(dest, buffer.Length());

        functions::Write(dest, buffer.Length());
    }

    static bool FlushBuffer()
    {
        if (m_buffer->Length() == 0) {
            return false; // Buffer is empty, no need to flush
        }

        // Create a header with the desired size
        Buffer header(
            strlen(EMBEDEB_MAGIC) +
            sizeof(size_t) +
            strlen(EMBEDEB_BOARD_NAME) +
            sizeof(char)
            );


        header.Append(EMBEDEB_MAGIC);
        // Length of the communication
        header.Append(
            m_buffer->Length()  // Every messages
            + strlen(EMBEDEB_MAGIC)  // Magic number
            + sizeof(size_t)                    // The length counter (the one actually calculated)
            + strlen(EMBEDEB_BOARD_NAME)   // Board name
            + sizeof(char)  // Message separator between the header and the content
        );

        header.Append(EMBEDEB_BOARD_NAME);
        header.Append(MessageSeparator);


        print(header);
        print(*m_buffer);
        m_buffer->ClearBuffer();
        return true;
    }
};

