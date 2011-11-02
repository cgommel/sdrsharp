// Filters.c : Defines the exported functions for the DLL application.
//

#include "Filters.h"

int _indexI;
int _indexQ;
int _indexA;

int _lenIQ;
int _lenA;
double _coeffsIQ[10000];
double _coeffsA[10000];

double _queueI[10000];
double _queueQ[10000];
double _queueA[10000];


void __stdcall InitIQ(double *coeffs, int len)
{
	int i;
	_lenIQ = len;
	for (i = 0; i < len; i++)
	{
		_coeffsIQ[i] = coeffs[i];
	}
}

void __stdcall InitAudio(double *coeffs, int len)
{
	int i;
	_lenA = len;
	for (i = 0; i < len; i++)
	{
		_coeffsA[i] = coeffs[i];
	}
}

double FirProcessI(double sample)
{
	int i;
    int n = _lenIQ;
    double result = 0.0;
    if (--_indexI < 0)
        _indexI = n - 1;
    _queueI[_indexI] = sample;
    for (i = 0; i < n; i++)
    {
        result += _queueI[_indexI] * _coeffsIQ[i];
        if (++_indexI >= n)
            _indexI = 0;
    }
    return result;
}

double FirProcessQ(double sample)
{
	int i;
    int n = _lenIQ;
    double result = 0.0;
    if (--_indexQ < 0)
        _indexQ = n - 1;
    _queueQ[_indexQ] = sample;
    for (i = 0; i < n; i++)
    {
        result += _queueQ[_indexQ] * _coeffsIQ[i];
        if (++_indexQ >= n)
            _indexQ = 0;
    }
    return result;
}

void __stdcall FirProcessIQ(Sample* iq, int len)
{
	int i;
	for (i = 0; i < len; i++)
	{
		iq[i].Real = FirProcessI(iq[i].Real);
		iq[i].Imag = FirProcessQ(iq[i].Imag);
	}
}

double FirProcessA(double sample)
{
	int i;
    int n = _lenA;
    double result = 0.0;
    if (--_indexA < 0)
        _indexA = n - 1;
    _queueA[_indexA] = sample;
    for (i = 0; i < n; i++)
    {
        result += _queueA[_indexA] * _coeffsA[i];
        if (++_indexA >= n)
            _indexA = 0;
    }
    return result;
}

void __stdcall FirProcessAudio(double* buffer, int len)
{
	int i;
	for (i = 0; i < len; i++)
	{
		buffer[i] = FirProcessA(buffer[i]);
	}
}
