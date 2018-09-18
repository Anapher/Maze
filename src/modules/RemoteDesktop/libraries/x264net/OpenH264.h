#pragma once

#include "lib/openh264/include/codec_api.h"

using namespace System;

public ref class OpenH264
{
private:
	ISVCDecoder* _decoder;

private:
	typedef int(__stdcall *WelsCreateDecoderFunc)(ISVCDecoder** ppDecoder);
	WelsCreateDecoderFunc CreateDecoderFunc;

	typedef void(__stdcall *WelsDestroyDecoderFunc)(ISVCDecoder* ppDecoder);
	WelsDestroyDecoderFunc DestroyDecoderFunc;

private:
	~OpenH264();
	!OpenH264();

public:
	OpenH264(String ^dllName);
};

