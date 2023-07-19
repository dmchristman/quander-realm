using System;

namespace Qupcakery
{
    public static class Constants
    {
#if LITE_VERSION
    public const int MaxLevelCnt = 16;
#else
    public const int MaxLevelCnt = 27;
#endif
        public const int MaxPuzzleCnt = 10;
        public const int MaxGateTypeCnt = 5;
        public const int MaxCustomerPerBatch = 3;
        public const int MaxBeltPerBatch = 3;
        public const int MaxGatePerBelt = 4;
        public const int MaxNumberOfPuzzlePerLevel = 10;
        public const int MaxNumberOfLevels = 25;
        public const float FloatCmpMargin = 0.0001f;
    }
}
