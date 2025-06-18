cmake_minimum_required(VERSION 3.10)
project(TextEditor VERSION 1.0 LANGUAGES CXX)

# Set the C++ standard
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_AUTOMOC ON)
set(CMAKE_BUILD_TYPE Debug)

# Find the Qt package
find_package(Qt6 REQUIRED COMPONENTS Widgets)

# Add include directories
include_directories(./src)
include_directories(${PROJECT_SOURCE_DIR}/external/json)

# Add source files
file(GLOB_RECURSE SOURCES src/*.cpp)

# Define the executable
add_executable(TextEditor ${SOURCES})

# Link Qt libraries
target_link_libraries(TextEditor Qt6::Widgets)
