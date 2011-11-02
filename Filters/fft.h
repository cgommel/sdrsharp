// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the FFT_EXPORTS
// symbol defined on the command line. this symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// FFT_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef FFT_EXPORTS
#define FFT_API __declspec(dllexport)
#else
#define FFT_API __declspec(dllimport)
#endif

#include <math.h>

#define PI 3.141592653589793238462

#ifdef __cplusplus
extern "C" {
#endif

typedef struct {
	double Real;
	double Imag;
} Sample;

FFT_API void __stdcall FFT(Sample *samples, int len);

#ifdef __cplusplus
}
#endif
