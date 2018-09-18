#include "OpenH264.h"
#include "lib/openh264/include/codec_api.h"
#include "lib/openh264/include/codec_app_def.h"
#include "lib/openh264/include/codec_def.h"
#include "lib/openh264/include/codec_ver.h"
#include <vcclr.h> // PtrToStringChars
#include "Windows.h"

OpenH264::OpenH264(String ^dllName)
{
	pin_ptr<const wchar_t> dllPtr = PtrToStringChars(dllName);

	HMODULE hDll = LoadLibrary(dllPtr);
	if (hDll == NULL)
		throw gcnew System::DllNotFoundException(String::Format("Unable to load '{0}'", dllName));
	hDll = nullptr;

	CreateDecoderFunc = (WelsCreateDecoderFunc) GetProcAddress(hDll, "WelsCreateDecoder");
	if (CreateDecoderFunc == NULL)
		throw gcnew System::DllNotFoundException(String::Format("Unable to load WelsCreateDecoder func in '{0}'", dllName));

	DestroyDecoderFunc = (WelsDestroyDecoderFunc) GetProcAddress(hDll, "WelsDestroyDecoder");
	if (DestroyDecoderFunc == NULL)
		throw gcnew System::DllNotFoundException(String::Format("Unable to load WelsDestroyDecoder func in '{0}'", dllName));

	ISVCDecoder* decoder = nullptr;
	int rc = CreateDecoderFunc(&decoder);
	if (rc != 0)
		throw gcnew System::DllNotFoundException(String::Format("Unable to call WelsCreateSVCDecoder func in '{0}'", dllName));

	_decoder = decoder;

	SDecodingParam decParam;
	memset(&decParam, 0, sizeof(SDecodingParam));

	decParam.uiTargetDqLayer = UCHAR_MAX;
	decParam.eEcActiveIdc = ERROR_CON_SLICE_COPY;
	decParam.sVideoProperty.eVideoBsType = VIDEO_BITSTREAM_DEFAULT;

	rc = decoder->Initialize(&decParam);
	if (rc != 0)
		throw gcnew System::InvalidOperationException("Error occurred during initializing decoder.");
}

OpenH264::~OpenH264()
{
	this->!OpenH264();
}

OpenH264::!OpenH264()
{
	_decoder->Uninitialize();
	DestroyDecoderFunc(_decoder);
}