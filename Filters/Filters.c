#include <stdlib.h>

#define EXPORT __declspec(dllexport)
#define INLINE __inline

typedef struct
		{
			double real;
			double imag;
		} complex;

EXPORT double INLINE __cdecl FirProcessSample(double sample, double *queue, double *coeffs, int queueSize, int *index)
{
    int i;
    double result = 0.0f;
    if (--*index < 0)
        *index = queueSize - 1;
    queue[*index] = sample;
    for (i = 0; i < queueSize; i++)
    {
        result += queue[*index] * coeffs[i];
        if (++*index >= queueSize)
            *index = 0;
    }
    return result;
}

EXPORT void __cdecl FirProcessBuffer(
	double *buffer,
	int bufferSize,
	double *queue,
	double *coeffs,
	int queueSize,
	int *index)
{
	int i, idx;
	idx = *index;

	for (i = 0; i < bufferSize; i++)
	{
		buffer[i] = FirProcessSample(buffer[i], queue, coeffs, queueSize, &idx);
	}
	
	*index = idx;
}

EXPORT void __cdecl FirProcessComplexBuffer(
	complex *buffer,
	int bufferSize,
	double *queue_r,
	double *queue_i,
	double *coeffs,
	int queueSize,
	int *index_r,
	int *index_i)
{
	int i, ir, ii;
	ir = *index_r;
	ii = *index_i;

	for (i = 0; i < bufferSize; i++)
	{
		buffer[i].real = FirProcessSample(buffer[i].real, queue_r, coeffs, queueSize, &ir);
		buffer[i].imag = FirProcessSample(buffer[i].imag, queue_i, coeffs, queueSize, &ii);
	}
	
	*index_r = ir;
	*index_i = ii;
}
