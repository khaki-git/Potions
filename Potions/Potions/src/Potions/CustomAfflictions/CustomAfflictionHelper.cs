namespace Potions.CustomAfflictions;

internal static class CustomAfflictionHelper
{
    internal const int POTIONS_ENUM_START = 418;

    internal static int WhatIsMyNumber(int myId)
    {
        return POTIONS_ENUM_START + myId;
    }
}