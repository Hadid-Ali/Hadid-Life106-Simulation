using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Reads and writes the Life 1.06 text format. This is the only place that knows
/// what the format looks like; the simulator deals purely in <see cref="GridCell"/>s.
/// </summary>
public static class FileIOComponent
{
    public static readonly string HEADER = "#Life 1.06";

    /// <summary>
    /// Parses Life 1.06 text into a de-duplicated live-cell set. Blank lines and
    /// '#' comment lines are ignored; malformed coordinate lines are skipped with
    /// a warning.
    /// </summary>
    public static HashSet<GridCell> ParseLife106(string text)
    {
        var liveCells = new HashSet<GridCell>();
        if (string.IsNullOrEmpty(text))
            return liveCells;

        string[] lines = text.Split('\n');
        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();
            if (line.Length == 0 || line[0] == '#')
                continue;

            string[] parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2
                || !long.TryParse(parts[0], out long x)
                || !long.TryParse(parts[1], out long y))
            {
                Debug.LogWarning($"[GameOfLife] Skipping malformed line: '{rawLine}'");
                continue;
            }

            liveCells.Add(new GridCell(x, y));
        }

        return liveCells;
    }

    /// <summary>
    /// Serialises the live set to Life 1.06, emitting cells in the order the
    /// simulation yields them (no reordering).
    /// </summary>
    public static string ConvertToLife106(IEnumerable<GridCell> liveCells)
    {
        var sb = new StringBuilder();
        sb.Append(HEADER).Append('\n');
        foreach (GridCell cell in liveCells)
            sb.Append(cell.X).Append(' ').Append(cell.Y).Append('\n');
        return sb.ToString();
    }
}
