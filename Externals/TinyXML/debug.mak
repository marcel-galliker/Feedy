#Generated by VisualGDB (http://visualgdb.com)
#DO NOT EDIT THIS FILE MANUALLY UNLESS YOU ABSOLUTELY NEED TO
#USE VISUALGDB PROJECT PROPERTIES DIALOG INSTEAD

BINARYDIR := ../../_out/Debug

#Toolchain
CC := $(TOOLCHAIN_ROOT)/bin/aarch64-none-linux-gnu-gcc.exe
CXX := $(TOOLCHAIN_ROOT)/bin/aarch64-none-linux-gnu-g++.exe
LD := $(CXX)
AR := $(TOOLCHAIN_ROOT)/bin/aarch64-none-linux-gnu-ar.exe
OBJCOPY := $(TOOLCHAIN_ROOT)/bin/aarch64-none-linux-gnu-objcopy.exe

#Additional flags
PREPROCESSOR_MACROS := NDEBUG=1 linux
INCLUDE_DIRS := 
LIBRARY_DIRS := 
LIBRARY_NAMES := 
ADDITIONAL_LINKER_INPUTS := 
MACOS_FRAMEWORKS := 
LINUX_PACKAGES := 

CFLAGS := -ggdb -ffunction-sections -O0
CXXFLAGS := -ggdb -ffunction-sections -O0
ASFLAGS := 
LDFLAGS := -Wl,-gc-sections
COMMONFLAGS := 
LINKER_SCRIPT := 

START_GROUP := -Wl,--start-group
END_GROUP := -Wl,--end-group

#Additional options detected from testing the toolchain
USE_DEL_TO_CLEAN := 1
CP_NOT_AVAILABLE := 1
IS_LINUX_PROJECT := 1