#include <math.h>

#define EXPORT __declspec(dllexport)
#define PI 3.141592653589793238462

typedef struct
{
	float real;
	float imag;
} complex_t;

EXPORT void __stdcall FFT(complex_t *samples, int len)
{
    int I, J, JM1, K, L, M, LE, LE2, IP;
    int NM1 = len - 1;
    int ND2 = len / 2;
	int ND4;
    float UR, UI, SR, SI, TR, TI;

    // ex: m = CINT(LOG(N%)/LOG(2))

    M = 0;
    I = len;
    while (I > 1)
    {
        ++M;
        I = (I >> 1);
    } // -> m = log2( n )

    J = ND2;

    for (I = 1; I < NM1; ++I)   // Bit reversal sorting
    {
        if (I < J)                // 1120   IF I% >= J% THEN GOTO 1190
        {
            TR = samples[J].real;
            TI = samples[J].imag;
            samples[J].real = samples[I].real;
            samples[J].imag = samples[I].imag;
            samples[I].real = TR;
            samples[I].imag = TI;
        }

        K = ND2;                  // 1190

        while (K <= J)            // 1200   IF K% > J% THEN GOTO 1240
        {
            J = J - K;
            K = K / 2;
        }                         // 1230  GOTO 1200

        J += K;                   // 1240   J% = J%+K%
    }                           // 1250 NEXT I%

    for (L = 1; L <= M; ++L)    // 1270 Loop for each stage
    {
        LE = 1 << L;              // 1280  LE% = CINT(2^L%)
        LE2 = LE / 2;               // 1290  LE2% = LE%/2
        UR = 1;
        UI = 0;

        // Use the standard trig functions instead of table lookup.
        // (these calculations are rarely done; not worth to eliminate sin+cos here)

        SR = cos(PI / LE2);    // Calculate sine & cosine values
        SI = -sin(PI / LE2);

        for (J = 1; J <= LE2; ++J)    // 1340 Loop for each sub DFT
        {
            JM1 = J - 1;

            for (I = JM1; I <= NM1; I += LE)          // 1360 Loop for each butterfly
            {
                IP = I + LE2;
                TR = samples[IP].real * UR - samples[IP].imag * UI;     // Butterfly calculation
                TI = samples[IP].real * UI + samples[IP].imag * UR;
                samples[IP].real = samples[I].real - TR;
                samples[IP].imag = samples[I].imag - TI;
                samples[I].real = samples[I].real + TR;
                samples[I].imag = samples[I].imag + TI;
            } // NEXT I

            TR = UR;                                  // 1450
            UR = TR * SR - UI * SI;
            UI = TR * SI + UI * SR;
        } // NEXT J
    } // NEXT L
    ND4 = ND2 / 2;
    for (I = 0; I < ND4; I++)
    {
        complex_t tmp = samples[I];
        samples[I] = samples[ND2 - I - 1];
        samples[ND2 - I - 1] = tmp;

        tmp = samples[ND2 + I];
        samples[ND2 + I] = samples[ND2 + ND2 - I - 1];
        samples[ND2 + ND2 - I - 1] = tmp;
    }
}
