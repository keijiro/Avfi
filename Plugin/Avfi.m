#import <AVFoundation/AVFoundation.h>

#if TARGET_OS_IOS
#import <UIKit/UIKit.h>
#endif

// Internal objects
static AVAssetWriter* _writer;
static AVAssetWriterInput* _writerInput;
static AVAssetWriterInputPixelBufferAdaptor* _bufferAdaptor;

extern void Avfi_StartRecording(const char* filePath, int width, int height)
{
    if (_writer)
    {
        NSLog(@"Recording has already been initiated.");
        return;
    }

    // Asset writer setup
    NSURL* filePathURL =
      [NSURL fileURLWithPath:[NSString stringWithUTF8String:filePath]];

    NSError* err;
    _writer =
      [[AVAssetWriter alloc] initWithURL: filePathURL
                                fileType: AVFileTypeQuickTimeMovie
                                   error: &err];

    if (err)
    {
        NSLog(@"Failed to initialize AVAssetWriter (%@)", err);
        return;
    }

    // Asset writer input setup
    NSDictionary* settings =
      @{ AVVideoCodecKey: AVVideoCodecTypeH264,
         AVVideoWidthKey: @(width),
        AVVideoHeightKey: @(height) };

    _writerInput =
      [[AVAssetWriterInput assetWriterInputWithMediaType: AVMediaTypeVideo
                                          outputSettings: settings] retain];
    _writerInput.expectsMediaDataInRealTime = true;

    [_writer addInput:_writerInput];

    // Pixel buffer adaptor setup
    NSDictionary* attribs =
      @{ (NSString*)kCVPixelBufferPixelFormatTypeKey: @(kCVPixelFormatType_32BGRA),
                   (NSString*)kCVPixelBufferWidthKey: @(width),
                  (NSString*)kCVPixelBufferHeightKey: @(height) };

    _bufferAdaptor =
      [[AVAssetWriterInputPixelBufferAdaptor
         assetWriterInputPixelBufferAdaptorWithAssetWriterInput: _writerInput
                                    sourcePixelBufferAttributes: attribs] retain];

    // Recording start
    if (![_writer startWriting])
    {
        NSLog(@"Failed to start (%ld: %@)", _writer.status, _writer.error);
        return;
    }

    [_writer startSessionAtSourceTime:kCMTimeZero];
}

extern void Avfi_AppendFrame(const void* source, uint32_t size, double time)
{
    if (!_writer)
    {
        NSLog(@"Recording hasn't been initiated.");
        return;
    }

    if (!_writerInput.isReadyForMoreMediaData)
    {
        NSLog(@"Writer is not ready.");
        return;
    }

    // Buffer allocation
    CVPixelBufferRef buffer;
    CVReturn ret = CVPixelBufferPoolCreatePixelBuffer
      (NULL, _bufferAdaptor.pixelBufferPool, &buffer);

    if (ret != kCVReturnSuccess)
    {
        NSLog(@"Can't allocate a pixel buffer (%d)", ret);
        NSLog(@"%ld: %@", _writer.status, _writer.error);
        return;
    }

    // Buffer update
    CVPixelBufferLockBaseAddress(buffer, 0);

    void* pointer = CVPixelBufferGetBaseAddress(buffer);
    size_t buffer_size = CVPixelBufferGetDataSize(buffer);
    memcpy(pointer, source, MIN(size, buffer_size));

    CVPixelBufferUnlockBaseAddress(buffer, 0);

    // Buffer submission
    [_bufferAdaptor appendPixelBuffer:buffer
                 withPresentationTime:CMTimeMakeWithSeconds(time, 240)];

    CVPixelBufferRelease(buffer);
}

extern void Avfi_EndRecording(void)
{
    if (!_writer)
    {
        NSLog(@"Recording hasn't been initiated.");
        return;
    }

    [_writerInput markAsFinished];

#if TARGET_OS_IOS

    NSString* path = [_writer.outputURL.path retain];
    [_writer finishWritingWithCompletionHandler: ^{
        UISaveVideoAtPathToSavedPhotosAlbum(path, nil, nil, nil);
        [path release];
    }];

#else

    [_writer finishWritingWithCompletionHandler: ^{}];

#endif

    [_writer release];
    [_writerInput release];
    [_bufferAdaptor release];

    _writer = NULL;
    _writerInput = NULL;
    _bufferAdaptor = NULL;
}
