#include <stdlib.h>

#define EXPORT __declspec(dllexport)
#define INLINE __inline

typedef struct
{
	double real;
	double imag;
} complex;

typedef struct
{
	int index;
	int queue_size;
	double* queue;
	double* coeffs;
} simple_filter;

typedef struct
{
	int index_r;
	int index_i;
	int queue_size;
	double* queue_r;
	double* queue_i;
	double* coeffs;
} complex_filter;

INLINE double FirProcessSample(double sample, double *queue, double *coeffs, int queueSize, int *index)
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
	int buffer_size,
	simple_filter *filter)
{
	int i;

	for (i = 0; i < buffer_size; i++)
	{
		buffer[i] = FirProcessSample(buffer[i], filter->queue, filter->coeffs, filter->queue_size, &filter->index);
	}
}

EXPORT void __cdecl FirProcessComplexBuffer(
	complex *buffer,
	int buffer_size,
	complex_filter *filter)
{
	int i;

	for (i = 0; i < buffer_size; i++)
	{
		buffer[i].real = FirProcessSample(buffer[i].real, filter->queue_r, filter->coeffs, filter->queue_size, &filter->index_r);
		buffer[i].imag = FirProcessSample(buffer[i].imag, filter->queue_i, filter->coeffs, filter->queue_size, &filter->index_i);
	}
}

EXPORT void *MakeSimpleFilter(double *coeffs, int length)
{
	int len_in_bytes = length * sizeof(double);
	simple_filter *filter = (simple_filter *) malloc(sizeof(simple_filter));
	filter->index = 0;
	filter->queue_size = length;
	filter->queue = (double *) malloc(len_in_bytes);
	memset(filter->queue, 0, len_in_bytes);
	filter->coeffs = (double *) malloc(len_in_bytes);
	memcpy(filter->coeffs, coeffs, len_in_bytes);
	return filter;
}

EXPORT void FreeSimpleFilter(simple_filter *filter)
{
	free(filter->queue);
	free(filter->coeffs);
	free(filter);
}

EXPORT void *MakeComplexFilter(double *coeffs, int length)
{
	int len_in_bytes = length * sizeof(double);
	complex_filter *filter = (complex_filter *) malloc(sizeof(complex_filter));
	filter->index_r = 0;
	filter->index_i = 0;
	filter->queue_size = length;
	filter->queue_r = (double *) malloc(len_in_bytes);
	memset(filter->queue_r, 0, len_in_bytes);
	filter->queue_i = (double *) malloc(len_in_bytes);
	memset(filter->queue_i, 0, len_in_bytes);
	filter->coeffs = (double *) malloc(len_in_bytes);
	memcpy(filter->coeffs, coeffs, len_in_bytes);
	return filter;
}

EXPORT void FreeComplexFilter(complex_filter *filter)
{
	free(filter->queue_r);
	free(filter->queue_i);
	free(filter->coeffs);
	free(filter);
}
