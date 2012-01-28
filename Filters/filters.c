#include <stdlib.h>

#define EXPORT __declspec(dllexport)
#define INLINE __inline

typedef struct
{
	float real;
	float imag;
} complex_t;

typedef struct
{
	int index;
	int queue_size;
	float* queue;
	float* coeffs;
} simple_filter_t;

typedef struct
{
	int index_r;
	int index_i;
	int queue_size;
	float* queue_r;
	float* queue_i;
	float* coeffs;
} complex_filter_t;

INLINE float FirProcessSample(float sample, float *queue, float *coeffs, int queueSize, int *index)
{
    int i;
    float result = 0.0f;
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
	float *buffer,
	int buffer_size,
	simple_filter_t *filter)
{
	int i;

	for (i = 0; i < buffer_size; i++)
	{
		buffer[i] = FirProcessSample(buffer[i], filter->queue, filter->coeffs, filter->queue_size, &filter->index);
	}
}

EXPORT void __cdecl FirProcessComplexBuffer(
	complex_t *buffer,
	int buffer_size,
	complex_filter_t *filter)
{
	int i;

	for (i = 0; i < buffer_size; i++)
	{
		buffer[i].real = FirProcessSample(buffer[i].real, filter->queue_r, filter->coeffs, filter->queue_size, &filter->index_r);
		buffer[i].imag = FirProcessSample(buffer[i].imag, filter->queue_i, filter->coeffs, filter->queue_size, &filter->index_i);
	}
}

EXPORT void *MakeSimpleFilter(float *coeffs, int length)
{
	int len_in_bytes = length * sizeof(float);
	simple_filter_t *filter = (simple_filter_t *) malloc(sizeof(simple_filter_t));
	filter->index = 0;
	filter->queue_size = length;
	filter->queue = (float *) malloc(len_in_bytes);
	memset(filter->queue, 0, len_in_bytes);
	filter->coeffs = (float *) malloc(len_in_bytes);
	memcpy(filter->coeffs, coeffs, len_in_bytes);
	return filter;
}

EXPORT void FreeSimpleFilter(simple_filter_t *filter)
{
	free(filter->queue);
	free(filter->coeffs);
	free(filter);
}

EXPORT void *MakeComplexFilter(float *coeffs, int length)
{
	int len_in_bytes = length * sizeof(float);
	complex_filter_t *filter = (complex_filter_t *) malloc(sizeof(complex_filter_t));
	filter->index_r = 0;
	filter->index_i = 0;
	filter->queue_size = length;
	filter->queue_r = (float *) malloc(len_in_bytes);
	memset(filter->queue_r, 0, len_in_bytes);
	filter->queue_i = (float *) malloc(len_in_bytes);
	memset(filter->queue_i, 0, len_in_bytes);
	filter->coeffs = (float *) malloc(len_in_bytes);
	memcpy(filter->coeffs, coeffs, len_in_bytes);
	return filter;
}

EXPORT void FreeComplexFilter(complex_filter_t *filter)
{
	free(filter->queue_r);
	free(filter->queue_i);
	free(filter->coeffs);
	free(filter);
}
