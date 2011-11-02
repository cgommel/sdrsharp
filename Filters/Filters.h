// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the FILTERS_EXPORTS
// symbol defined on the command line. this symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// FILTERS_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef FILTERS_EXPORTS
#define FILTERS_API __declspec(dllexport)
#else
#define FILTERS_API __declspec(dllimport)
#endif

#ifdef __cplusplus
extern "C" {
#endif

#include "fft.h"

FILTERS_API void __stdcall InitIQ(double *coeffs, int len);
FILTERS_API void __stdcall InitAudio(double *coeffs, int len);
FILTERS_API void __stdcall FirProcessIQ(Sample* sample, int len);
FILTERS_API void __stdcall FirProcessAudio(double* sample, int len);

#ifdef __cplusplus
}
#endif
