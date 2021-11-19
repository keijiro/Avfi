#!/bin/sh

CFLAGS="-O2 -Wall"

LIBS="-framework Foundation -framework AVFoundation"
LIBS+=" -framework CoreMedia -framework CoreVideo -framework QuartzCore"

MAC_ARGS="-shared -rdynamic -fPIC"

IOS_ROOT=`xcrun --sdk iphoneos --show-sdk-path`
IOS_ARGS="--sysroot $IOS_ROOT -isysroot $IOS_ROOT -fembed-bitcode"

rm *.o *.so *.a *.bundle

set -x

gcc -target x86_64-apple-macos10.13 $CFLAGS $MAC_ARGS Plugin.m $LIBS -o x86_64.so
gcc -target  arm64-apple-macos10.13 $CFLAGS $MAC_ARGS Plugin.m $LIBS -o arm64.so

gcc $CFLAGS $IOS_ARGS -c Plugin.m

lipo -create -output VideoWriter.bundle x86_64.so arm64.so

ar -crv libVideoWriter.a Plugin.o

DST="../Assets/VideoWriter/Runtime/Plugins"
cp VideoWriter.bundle $DST
cp libVideoWriter.a $DST
