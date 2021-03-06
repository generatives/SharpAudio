﻿using SharpAudio.ALBinding;
using System;
using System.Runtime.InteropServices;

namespace SharpAudio.AL
{
    internal class ALBuffer : AudioBuffer
    {
        private uint _buffer;

        public uint Buffer => _buffer;

        public ALBuffer()
        {
            var buffers = new uint[1];
            AlNative.alGenBuffers(1, buffers);
            ALEngine.checkAlError();
            _buffer = buffers[0];
        }

        public override unsafe void BufferData<T>(T[] buffer, AudioFormat format)
        {
            int fmt = (format.Channels==2) ? AlNative.AL_FORMAT_STEREO8 : AlNative.AL_FORMAT_MONO8;
            int sizeInBytes = sizeof(T) * buffer.Length;

            if (format.BitsPerSample==16)
            {
                fmt++;
            }

            var handle = GCHandle.Alloc(buffer);
            IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);

            AlNative.alBufferData(_buffer, fmt, ptr, sizeInBytes, format.SampleRate);
            ALEngine.checkAlError();

            handle.Free();
            _format = format;
        }

        public override void Dispose()
        {
            AlNative.alDeleteBuffers(1, new uint[] { _buffer });
        }
    }
}
