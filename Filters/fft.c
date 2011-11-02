#include "fft.h"

void __stdcall FFT(Sample *samples, int len)
{
    int I, J, JM1, K, L, M, LE, LE2, IP;
    int NM1 = len - 1;
    int ND2 = len / 2;
	int ND4;
    double UR, UI, SR, SI, TR, TI;

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
            TR = samples[J].Real;
            TI = samples[J].Imag;
            samples[J].Real = samples[I].Real;
            samples[J].Imag = samples[I].Imag;
            samples[I].Real = TR;
            samples[I].Imag = TI;
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
                TR = samples[IP].Real * UR - samples[IP].Imag * UI;     // Butterfly calculation
                TI = samples[IP].Real * UI + samples[IP].Imag * UR;
                samples[IP].Real = samples[I].Real - TR;
                samples[IP].Imag = samples[I].Imag - TI;
                samples[I].Real = samples[I].Real + TR;
                samples[I].Imag = samples[I].Imag + TI;
            } // NEXT I

            TR = UR;                                  // 1450
            UR = TR * SR - UI * SI;
            UI = TR * SI + UI * SR;
        } // NEXT J
    } // NEXT L
    ND4 = ND2 / 2;
    for (I = 0; I < ND4; I++)
    {
        Sample tmp = samples[I];
        samples[I] = samples[ND2 - I - 1];
        samples[ND2 - I - 1] = tmp;

        tmp = samples[ND2 + I];
        samples[ND2 + I] = samples[ND2 + ND2 - I - 1];
        samples[ND2 + ND2 - I - 1] = tmp;
    }
}
