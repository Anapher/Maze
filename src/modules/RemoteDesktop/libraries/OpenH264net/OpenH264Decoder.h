#pragma once

using namespace System;

namespace OpenH264Lib {

	public value struct DecodedFrame {
	public:
		byte* Pointer;
		int Width;
		int Height;
		int Stride;
	};

	public ref class OpenH264Decoder
	{
	private:
		ISVCDecoder* _decoder;

	private:
		typedef int(__stdcall *WelsCreateDecoderFunc)(ISVCDecoder** ppDecoder);
		WelsCreateDecoderFunc CreateDecoderFunc;

		typedef void(__stdcall *WelsDestroyDecoderFunc)(ISVCDecoder* ppDecoder);
		WelsDestroyDecoderFunc DestroyDecoderFunc;

	private:
		~OpenH264Decoder();
		!OpenH264Decoder();

	public:
		OpenH264Decoder(String ^dllName);

	public:
		///<summary>Decode h264 frame data to Bitmap.</summary>
		///<returns>Might be null if frame data is incomplete.</returns>
		DecodedFrame^ Decode(unsigned char *frame, int length);

	public:
		void ReleaseFrame(DecodedFrame^ frame);
		

	private:
		static byte* YUV420PtoRGB(byte* yplane, byte* uplane, byte* vplane, int width, int height, int stride);
	};
}
