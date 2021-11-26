using NP.Utilities;

namespace NP.Avalonia.Visuals.ColorUtils
{
    public record HslColor(int A, float H, float S, float L)
    {
        public override string ToString()
        {
            return $"{A.ToHex()}, {H}, {S.ToFixed(1)}%, {L.ToFixed(1)}%";
        }
    }
}
