Public Module DExp1

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="[Dim]"></param>
    ''' <param name="Xv">Xv is the input matrix, Dim*Dim dimension</param>
    ''' <param name="len"></param>
    ''' <param name="result">result is a vector with length = len, corresponding to the 7 definitions and dmean</param>
    Public Sub dExp1(ByRef [Dim] As Integer, Xv As Double(), ByRef len As Integer, ByRef result As Double())
        Dim N As Integer = [Dim]
        ' int n = *len;
        ' int i, j, k, l;
        Dim i As Integer
        Dim j As Integer
        Dim k As Integer
        Dim tmp As Double
        Dim tmpij As Double
        Dim tmpjk As Double
        Dim tmpik As Double

        'Initilize the result vector to all 0
        For i = 0 To len - 1
            result(i) = 0
        Next

        'Calculate mean
        For i = 0 To N - 2
            For j = i + 1 To N - 1
                result(0) += Xv(i * N + j)
            Next
        Next
        result(0) = result(0) / (CDbl(N) * (N - 1) / 2)

        'Minus mean from Xv and calculate Def.1 and Def.3
        For i = 0 To N - 2
            For j = i + 1 To N - 1
                Xv(i * N + j) -= result(0)
                Xv(j * N + i) -= result(0)
                tmp = Xv(i * N + j) * Xv(i * N + j)
                result(1) += tmp
                tmp = tmp * Xv(i * N + j)
                result(3) += tmp
                result(8) += tmp * Xv(i * N + j)
            Next
        Next

        result(1) = result(1) / (CDbl(N) * (N - 1) / 2)
        result(3) = result(3) / (CDbl(N) * (N - 1) / 2)
        result(8) = result(8) / (CDbl(N) * (N - 1) / 2)

        'Calculate Def.2, Def.4 and Def.5
        For i = 0 To N - 3
            For j = i + 1 To N - 2
                For k = j + 1 To N - 1
                    result(2) += Xv(i * N + j) * Xv(i * N + k) + Xv(i * N + j) * Xv(j * N + k) + Xv(i * N + k) * Xv(j * N + k)

                    tmpij = Xv(i * N + j) * Xv(i * N + j)
                    tmpik = Xv(i * N + k) * Xv(i * N + k)
                    tmpjk = Xv(j * N + k) * Xv(j * N + k)

                    ' result[10] +=	Xv[i*N+j] * Xv[i*N+j] * Xv[i*N+k] * Xv[i*N+k] +
                    ' 				Xv[i*N+j] * Xv[i*N+j] * Xv[j*N+k] * Xv[j*N+k] +
                    ' 				Xv[i*N+k] * Xv[i*N+k] * Xv[j*N+k] * Xv[j*N+k];

                    result(10) += tmpij * (tmpik + tmpjk) + tmpik * tmpjk

                    ' result[4] +=	Xv[i*N+j] * Xv[i*N+j] * Xv[i*N+k] +
                    ' 				Xv[i*N+j] * Xv[i*N+k] * Xv[i*N+k] +
                    ' 				Xv[i*N+j] * Xv[i*N+j] * Xv[j*N+k] +
                    ' 				Xv[i*N+j] * Xv[j*N+k] * Xv[j*N+k] +
                    ' 				Xv[i*N+k] * Xv[i*N+k] * Xv[j*N+k] +
                    ' 				Xv[i*N+k] * Xv[j*N+k] * Xv[j*N+k];

                    ' result[4] +=	Xv[i*N+j] * Xv[i*N+j] * (Xv[i*N+k] + Xv[j*N+k]) +
                    ' 				Xv[i*N+k] * Xv[i*N+k] * (Xv[i*N+j] + Xv[j*N+k]) +
                    ' 				Xv[j*N+k] * Xv[j*N+k] * (Xv[i*N+j] + Xv[i*N+k]);

                    tmpij = tmpij * (Xv(i * N + k) + Xv(j * N + k))
                    tmpik = tmpik * (Xv(i * N + j) + Xv(j * N + k))
                    tmpjk = tmpjk * (Xv(i * N + j) + Xv(i * N + k))

                    result(4) += tmpij + tmpik + tmpjk

                    ' result[9] +=	Xv[i*N+j] * Xv[i*N+j] * Xv[i*N+j] * Xv[i*N+k] +
                    ' 				Xv[i*N+j] * Xv[i*N+k] * Xv[i*N+k] * Xv[i*N+k] +
                    ' 				Xv[i*N+j] * Xv[i*N+j] * Xv[i*N+j] * Xv[j*N+k] +
                    ' 				Xv[i*N+j] * Xv[j*N+k] * Xv[j*N+k] * Xv[j*N+k] +
                    ' 				Xv[i*N+k] * Xv[i*N+k] * Xv[i*N+k] * Xv[j*N+k] +
                    ' 				Xv[i*N+k] * Xv[j*N+k] * Xv[j*N+k] * Xv[j*N+k];

                    ' result[9] +=	Xv[i*N+j] * Xv[i*N+j] * Xv[i*N+j] * (Xv[i*N+k] + Xv[j*N+k]) +
                    ' 				Xv[i*N+k] * Xv[i*N+k] * Xv[i*N+k] * (Xv[i*N+j] + Xv[j*N+k]) +
                    ' 				Xv[j*N+k] * Xv[j*N+k] * Xv[j*N+k] * (Xv[i*N+j] + Xv[i*N+k]);

                    result(9) += tmpij * Xv(i * N + j) + tmpik * Xv(i * N + k) + tmpjk * Xv(j * N + k)

                    tmp = Xv(i * N + j) * Xv(j * N + k) * Xv(i * N + k)
                    result(5) += tmp
                    result(11) += tmp * (Xv(i * N + j) + Xv(j * N + k) + Xv(i * N + k))

                Next
            Next
        Next
        result(2) = result(2) / 3 / (CDbl(N) * (N - 1) * (N - 2) / 6)
        result(10) = result(10) / 3 / (CDbl(N) * (N - 1) * (N - 2) / 6)
        result(4) = result(4) / 6 / (CDbl(N) * (N - 1) * (N - 2) / 6)
        result(9) = result(9) / 6 / (CDbl(N) * (N - 1) * (N - 2) / 6)
        result(5) = result(5) / (CDbl(N) * (N - 1) * (N - 2) / 6)
        result(11) = result(11) / 3 / (CDbl(N) * (N - 1) * (N - 2) / 6)

        ' //Calculate Def.6, Def7
        ' for(i = 0; i < N - 3; i++){
        ' 	for(j = i + 1; j < N - 2; j++){
        ' 		for(k = j + 1; k < N - 1; k++){
        ' 			for(l = k + 1; l < N; l++){
        ' 				result[6] +=	Xv[i*N+j] * Xv[k*N+l] * (Xv[i*N+k] + Xv[i*N+l] + Xv[j*N+k] + Xv[j*N+l]) +
        ' 								Xv[i*N+k] * Xv[j*N+l] * (Xv[i*N+j] + Xv[i*N+l] + Xv[k*N+j] + Xv[k*N+l]) +
        ' 								Xv[i*N+l] * Xv[j*N+k] * (Xv[i*N+j] + Xv[i*N+k] + Xv[j*N+l] + Xv[k*N+l]);
        '
        ' 				// result[7] +=	Xv[i*N+j] * Xv[i*N+k] * Xv[i*N+l] +
        ' 				// 				Xv[i*N+j] * Xv[j*N+k] * Xv[j*N+l] +
        ' 				// 				Xv[i*N+k] * Xv[j*N+k] * Xv[k*N+l] +
        ' 				// 				Xv[i*N+l] * Xv[j*N+l] * Xv[k*N+l];
        '
        ' 				result[7] +=	Xv[i*N+j] * (Xv[i*N+k] * Xv[i*N+l] + Xv[j*N+k] * Xv[j*N+l]) +
        ' 								Xv[k*N+l] * (Xv[i*N+k] * Xv[j*N+k] + Xv[i*N+l] * Xv[j*N+l]);
        '
        ' 			}
        ' 		}
        '
        ' 	}
        ' }
        '
        ' result[6] = result[6] / 12 / (((double)N) * (N-1) * (N-2) * (N-3) / 24);
        ' result[7] = result[7] / 4 / (((double)N) * (N-1) * (N-2) * (N-3) / 24);

    End Sub
End Module

