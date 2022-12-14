float4x4 QuaternionMatrix(float qi, float qj, float qk, float qr)
{
    return float4x4(1 - 2 * (pow(qj, 2) + pow(qk, 2)), 2 * (qi * qj - qk * qr), 2 * (qi * qk + qj * qr), 0, 
            2 * (qi * qj + qk * qr), 1 - 2 * (pow(qi, 2) + pow(qk, 2)), 2 * (qj * qk - qi * qr), 0,
            2 * (qi * qk - qj * qr), 2 * (qj * qk + qi * qr), 1 - 2 * (pow(qi, 2) + pow(qj, 2)), 0,
            0, 0, 0, 1
        );
}