//
//  PHASEWrapperRingBuffer.h
//  AudioPluginPHASE
//
//  Copyright © 2024, 2026 Apple Inc.
//

#ifndef PHASEWrapperRingBuffer_h
#define PHASEWrapperRingBuffer_h

#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>

NS_HEADER_AUDIT_BEGIN(nullability)

@interface PHASEWrapperRingBuffer : NSObject
- (nullable instancetype)initWithFrameSize:(int)frameSize
                           numberOfBuffers:(int)numberOfBuffers
                                    format:(AVAudioFormat*)format;
- (BOOL)read:(AudioBufferList*)output frameCount:(AVAudioFrameCount)frameCount;
- (BOOL)write:(float*)input frameCount:(AVAudioFrameCount)frameCount;
- (BOOL)isEmpty;
- (uint64_t)consumeAndResetUnderrunCount;
@end

NS_HEADER_AUDIT_END(nullability)

#endif /* PHASEWrapperRingBuffer_h */
