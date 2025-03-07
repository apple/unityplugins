//
//  PHASEWrapperRingBuffer.mm
//  AudioPluginPHASE
//
//  Copyright © 2024 Apple Inc. All rights reserved.
//

#import "PHASEWrapperRingBuffer.h"
#import <Accelerate/Accelerate.h>

@interface PHASEBuffer : NSObject
@property (strong) AVAudioPCMBuffer* mBuffer;
@end
@implementation PHASEBuffer
{
    AVAudioFrameCount mCurReadPosition;
}

- (int)read:(AudioBufferList*)output offset:(AVAudioFrameCount)offset frameCount:(AVAudioFrameCount)frameCount
{
    // If we have more than (or exactly what we asked for) read it all, otherwise read what we have.
    AVAudioFrameCount framesAvailable = _mBuffer.frameCapacity - mCurReadPosition;
    AVAudioFrameCount framesToRead = std::min(frameCount, framesAvailable);
    for (int chan = 0; chan < _mBuffer.format.channelCount; ++chan)
    {
        float* inputChannel = _mBuffer.floatChannelData[chan];
        float* outputChannel = static_cast<float*>(output->mBuffers[chan].mData);
        
        memcpy(outputChannel + offset,
               inputChannel + mCurReadPosition,
               sizeof(float) * framesToRead);
    }
    mCurReadPosition += framesToRead;
    
    // NSLog(@"PHASE Wrapper Ring Buffer: Read %d at offset %d. Current ReadPosition %d", framesToRead, offset, mCurReadPosition);
    
    // Return how much we read
    return framesToRead;
}

- (void)write:(float*)input frameCount:(AVAudioFrameCount)frameCount
{
    // TODO: Assuming stereo for fast deinterleave.
    DSPSplitComplex out = { _mBuffer.floatChannelData[0], _mBuffer.floatChannelData[1] };
    vDSP_ctoz((DSPComplex const*) input, 2, &out, 1, frameCount);
    mCurReadPosition = 0;
}

- (BOOL)hasData
{
    return mCurReadPosition < _mBuffer.frameCapacity;
}

@end

@implementation PHASEWrapperRingBuffer
{
    NSArray<PHASEBuffer*>* mBuffers;
    int mReadIdx;
    int mWriteIdx;
    int mNumberOfBuffers;
}

- (nullable instancetype)initWithFrameSize:(int)frameSize
                           numberOfBuffers:(int)numberOfBuffers
                                    format:(AVAudioFormat*)format
{
    NSMutableArray* buffers = [NSMutableArray new];
    for (int bufferIdx = 0; bufferIdx < numberOfBuffers; ++bufferIdx)
    {
        PHASEBuffer* buffer = [PHASEBuffer alloc];
        buffer.mBuffer = [[AVAudioPCMBuffer alloc] initWithPCMFormat:format frameCapacity:frameSize];
        if (buffer.mBuffer == nil)
        {
            NSLog(@"PHASE Wrapper Ring Buffer: Insufficient memory for buffer.");
            return nil;
        }
        
        [buffers addObject:buffer];
    }
    
    mBuffers = buffers;
    mReadIdx = mWriteIdx = 0;
    mNumberOfBuffers = numberOfBuffers;
    
    return self;
}

- (BOOL)read:(AudioBufferList*)output frameCount:(AVAudioFrameCount)frameCount
{
    AVAudioFrameCount totalFramesRead = 0;
    AVAudioFrameCount framesToRead = frameCount;
    do
    {
        // NSLog(@"PHASE Wrapper Ring Buffer: Attempting to read %d at offset %d from buffer %d", framesToRead, totalFramesRead, mReadIdx);
        
        int framesRead = [mBuffers[mReadIdx] read:output offset:totalFramesRead frameCount:framesToRead];
        if (framesRead == 0)
        {
            return NO;
        }
        
        if (![mBuffers[mReadIdx] hasData])
        {
            mReadIdx = (mReadIdx + 1) % mNumberOfBuffers;
        }
        
        framesToRead -= framesRead;
        totalFramesRead += framesRead;
    } while (totalFramesRead < frameCount);
    
    return YES;
}

- (BOOL)write:(float*)input frameCount:(AVAudioFrameCount)frameCount
{
    // NSLog(@"PHASE Wrapper Ring Buffer: Writing %d to buffer %d", frameCount, mWriteIdx);
    [mBuffers[mWriteIdx] write:input frameCount:frameCount];
    mWriteIdx = (mWriteIdx + 1) % mNumberOfBuffers;
    return YES;
}

@end
