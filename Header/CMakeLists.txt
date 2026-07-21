cmake_minimum_required(VERSION 3.15)
project(EmbedebHeaderProject CXX)

# Use C++17
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Declare the header-only lib
add_library(Embedeb INTERFACE)
target_include_directories(Embedeb INTERFACE ${CMAKE_CURRENT_SOURCE_DIR}/include)

# Unused target to compile, help the clang analysing the header code
set(LOCAL_SRC "${CMAKE_CURRENT_SOURCE_DIR}/local_src")
set(DEFAULT_SRC "${CMAKE_CURRENT_SOURCE_DIR}/src")

# Use your local testing project
if(EXISTS "${LOCAL_SRC}" AND IS_DIRECTORY "${LOCAL_SRC}")
    set(TARGET_DIRECTORY "${LOCAL_SRC}")

# Use a default blank project if u don't have a local project
else ()
    set(TARGET_DIRECTORY "${DEFAULT_SRC}")

endif()


file(GLOB SOURCES
        "${TARGET_DIRECTORY}/*.cpp"
        "${TARGET_DIRECTORY}/*.h"
)

add_executable(Testing ${SOURCES})

target_link_libraries(Testing PRIVATE Embedeb)

