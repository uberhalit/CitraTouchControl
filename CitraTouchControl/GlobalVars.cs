namespace CitraTouchControl
{
    class GlobalVars
    {
        // fast access variables so user4settings doesn't have to be loaded/looped through every time user presses a control button
        internal static short A_KEY = 0x41;         // A
        internal static short B_KEY = 0x53;         // S
        internal static short X_KEY = 0x5A;         // Y (Z on QWERTZ-Layout)
        internal static short Y_KEY = 0x58;         // X
        internal static short L_KEY = 0x51;         // Q
        internal static short R_KEY = 0x57;         // W
        internal static short LEFT_KEY = 0x25;      // Left
        internal static short RIGHT_KEY = 0x27;     // Right
        internal static short UP_KEY = 0x26;        // Up
        internal static short DOWN_KEY = 0x28;      // Down
        internal static short START_KEY = 0x4D;     // M
        internal static short SELECT_KEY = 0x4E;    // N

        internal static bool IsTouchEnabled = false;
        internal static bool AreControlsHidden = false;
        internal static int KeyPressDuration = 50;
    }
}
