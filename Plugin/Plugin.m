#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>
#import <AVFoundation/AVAssetWriter.h>

static AVAssetWriter* _writer;
static AVAssetWriterInput* _writerInput;
static AVAssetWriterInputPixelBufferAdaptor* _bufferAdaptor;
static CFTimeInterval _startTime;
static int _counter;

extern void VideoWriter_Start(void)
{
    NSURL* fileURL = [NSURL fileURLWithPath:@"test.mp4"];

    _writer =
      [[AVAssetWriter alloc] initWithURL: fileURL
                                fileType: AVFileTypeQuickTimeMovie
                                   error: nil];

    NSDictionary* settings =
      @{ AVVideoCodecKey: AVVideoCodecTypeH264,
         AVVideoWidthKey: @(1920),
        AVVideoHeightKey: @(1080) };

    _writerInput =
      [AVAssetWriterInput assetWriterInputWithMediaType: AVMediaTypeVideo
                                         outputSettings: settings];

    _writerInput.expectsMediaDataInRealTime = true;
    
    NSDictionary* attribs =
      @{ (NSString*)kCVPixelBufferPixelFormatTypeKey: @(kCVPixelFormatType_32BGRA),
                   (NSString*)kCVPixelBufferWidthKey: @(1920),
                  (NSString*)kCVPixelBufferHeightKey: @(1080) };

    _bufferAdaptor =
      [AVAssetWriterInputPixelBufferAdaptor
        assetWriterInputPixelBufferAdaptorWithAssetWriterInput: _writerInput
                                   sourcePixelBufferAttributes: attribs];
    
    [_writer addInput:_writerInput];
    [_writer startWriting];
    [_writer startSessionAtSourceTime:kCMTimeZero];
    _startTime = CACurrentMediaTime();
    _counter = 0;
    
    [_writerInput retain];
    [_bufferAdaptor retain];
}

extern void VideoWriter_Update(void)
{
    if (!_writerInput.isReadyForMoreMediaData)
    {
        NSLog(@"_writerInput.isReadyForMoreMediaData is false");
        return;
    }
    
    if (_bufferAdaptor.pixelBufferPool == NULL)
    {
        NSLog(@"pixelBufferPool is null");
        return;
    }
    
    CVPixelBufferRef buffer;
    CVReturn ret = CVPixelBufferPoolCreatePixelBuffer
      (NULL, _bufferAdaptor.pixelBufferPool, &buffer);
    
    if (ret != kCVReturnSuccess)
    {
        NSLog(@"CVPixelBufferPoolCreatePixelBuffer failed");
        return;
    }
    
    CVPixelBufferLockBaseAddress(buffer, 0);
    void* pointer = CVPixelBufferGetBaseAddress(buffer);
    size_t stride = CVPixelBufferGetBytesPerRow(buffer);
    uint32_t fill = ((_counter * 8) & 0xff) | 0xff000000;
    
    for (int y = 0; y < 1080; y++)
    {
        uint32_t* array = pointer + stride * y;
        for (int x = 0; x < 1920; x++)
        {
            array[x] = fill;
        }
    }
    
    CFTimeInterval frameTime = CACurrentMediaTime() - _startTime;
    CMTime preTime = CMTimeMakeWithSeconds(frameTime, 240);
    
    [_bufferAdaptor appendPixelBuffer:buffer withPresentationTime:preTime];
    CVPixelBufferUnlockBaseAddress(buffer, 0);
    
    _counter++;
}

extern void VideoWriter_End(void)
{
    [_writerInput markAsFinished];
    [_writer finishWritingWithCompletionHandler:^{ NSLog(@"Done"); }];
    [_writer release];
}
