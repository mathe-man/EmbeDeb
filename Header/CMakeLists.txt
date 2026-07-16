cmake_minimum_required(VERSION 3.15)
project(EmbedebHeaderProject CXX)

# Use C++17
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Declare the header-only lib
add_library(Embedeb INTERFACE)
target_include_directories(Embedeb INTERFACE ${CMAKE_CURRENT_SOURCE_DIR}/include)

# Unused target to compile, help the clang analysing the header code
add_executable(embedeb_test src/target_main.cpp)
target_link_libraries(embedeb_test PRIVATE Embedeb)

