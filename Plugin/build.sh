#!/bin/sh

ARGS="-O2 -Wall -shared -rdynamic -fPIC"
ARGS+=" -framework Foundation -framework AVFoundation -framework CoreMedia -framework CoreVideo -framework QuartzCore"

set -x

rm *.so *.bundle

gcc -target x86_64-apple-macos10.13 $ARGS Plugin.m -o x86_64.so
gcc -target  arm64-apple-macos10.13 $ARGS Plugin.m -o arm64.so

lipo -create -output VideoWriter.bundle x86_64.so arm64.so

cp VideoWriter.bundle ../Assets/
#cp arm64.so ../Assets/VideoWriter.bundle
rm ../Assets/VideoWriter.bundle.meta
