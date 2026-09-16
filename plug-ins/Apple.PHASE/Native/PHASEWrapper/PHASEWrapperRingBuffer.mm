//
//  PHASEWrapperRingBuffer.mm
//  AudioPluginPHASE
//
//  Copyright © 2024, 2026 Apple Inc.
//

#import "PHASEWrapperRingBuffer.h"
#import <Accelerate/Accelerate.h>
#include <atomic>

@implementation PHASEWrapperRingBuffer
{
    NSArray<AVAudioPCMBuffer*>* mBuffers;
    int mNumberOfBuffers;
    int mReadIdx;
    int mWriteIdx;
    AVAudioFrameCount mCurReadPosition;
    AVAudioFrameCount mCurWritePosition;
    int mBufferSize;
    std::atomic<uint64_t> mUnderrunCount;
}

- (nullable instancetype)initWithFrameSize:(int)frameSize
                           numberOfBuffers:(int)numberOfBuffers
                                    format:(AVAudioFormat*)format
{
    NSMutableArray* buffers = [NSMutableArray new];
    for (int bufferIdx = 0; bufferIdx < numberOfBuffers; ++bufferIdx)
    {
        AVAudioPCMBuffer* buffer = [[AVAudioPCMBuffer alloc] initWithPCMFormat:format frameCapacity:frameSize];
        if (buffer == nil)
        {
            NSLog(@"PHASE Wrapper Ring Buffer: Insufficient memory for buffer.");
            return nil;
        }
        
        [buffers addObject:buffer];
    }
    
    mBuffers = buffers;
    mReadIdx = mWriteIdx = 0;
    mNumberOfBuffers = numberOfBuffers;
    mBufferSize = frameSize;
    
    return self;
}

- (BOOL)read:(AudioBufferList*)output frameCount:(AVAudioFrameCount)frameCount
{
    AVAudioFrameCount totalFramesRead = 0;
    while (totalFramesRead < frameCount)
    {
        if ([self isEmpty])
        {
            mUnderrunCount.fetch_add(1, std::memory_order_relaxed);
            return NO;
        }
        
        AVAudioPCMBuffer* currentReadBuffer = mBuffers[mReadIdx];
        AVAudioFrameCount framesAvailable = mBufferSize - mCurReadPosition;
        AVAudioFrameCount framesToRead = std::min(framesAvailable, frameCount - totalFramesRead);
        
        for (int chan = 0; chan < currentReadBuffer.format.channelCount; ++chan)
        {
            float* inputChannel = currentReadBuffer.floatChannelData[chan];
            float* outputChannel = static_cast<float*>(output->mBuffers[chan].mData);
            
            memcpy(outputChannel + totalFramesRead,
                   inputChannel + mCurReadPosition,
                   sizeof(float) * framesToRead);
        }
        
        totalFramesRead += framesToRead;
        
        mCurReadPosition += framesToRead;
        if (mCurReadPosition >= mBufferSize)
        {
            mReadIdx = (mReadIdx + 1) % mNumberOfBuffers;
            mCurReadPosition = 0;
        }
    };
    
    return YES;
}

- (BOOL)write:(float*)input frameCount:(AVAudioFrameCount)frameCount
{
    AVAudioFrameCount totalFramesWritten = 0;
    while (totalFramesWritten < frameCount)
    {
        AVAudioPCMBuffer* currentWriteBuffer = mBuffers[mWriteIdx];
        AVAudioFrameCount framesAvailable = mBufferSize - mCurWritePosition;
        AVAudioFrameCount framesToWrite = std::min(framesAvailable, frameCount - totalFramesWritten);

        int channelCount = currentWriteBuffer.format.channelCount;

        if (channelCount == 1)
        {
            float* outputChannel = currentWriteBuffer.floatChannelData[0] + mCurWritePosition;
            memcpy(outputChannel, input + totalFramesWritten, sizeof(float) * framesToWrite);
        }
        else if (channelCount == 2)
        {
            DSPSplitComplex out = {
                currentWriteBuffer.floatChannelData[0] + mCurWritePosition,
                currentWriteBuffer.floatChannelData[1] + mCurWritePosition
            };
            vDSP_ctoz((DSPComplex const*)(input + totalFramesWritten * 2), 2, &out, 1, framesToWrite);
        }
        else
        {
            return NO;
        }

        totalFramesWritten += framesToWrite;

        mCurWritePosition += framesToWrite;
        if (mCurWritePosition >= mBufferSize)
        {
            mWriteIdx = (mWriteIdx + 1) % mNumberOfBuffers;
            mCurWritePosition = 0;
        }
    }

    return YES;
}

- (BOOL)isEmpty
{
    return (mReadIdx == mWriteIdx) && (mCurReadPosition == mCurWritePosition);
}

- (uint64_t)consumeAndResetUnderrunCount
{
    return mUnderrunCount.exchange(0, std::memory_order_relaxed);
}

@end
