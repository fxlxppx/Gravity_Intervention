using System;
using System.Collections.Generic;

public static class ButtonSystem
{
    public static event Action<DoorColorEnum, int> OnButtonPressedCountChanged;

    private static Dictionary<DoorColorEnum, int> pressedCounts = new Dictionary<DoorColorEnum, int>();

    public static void ReportButtonState(DoorColorEnum color, bool pressed)
    {
        if (!pressedCounts.ContainsKey(color)) pressedCounts[color] = 0;

        if (pressed) pressedCounts[color] = pressedCounts[color] + 1;
        else pressedCounts[color] = Math.Max(0, pressedCounts[color] - 1);

        OnButtonPressedCountChanged?.Invoke(color, pressedCounts[color]);
    }

    public static int GetPressedCount(DoorColorEnum color)
    {
        if (pressedCounts.TryGetValue(color, out int v)) return v;
        return 0;
    }
}
