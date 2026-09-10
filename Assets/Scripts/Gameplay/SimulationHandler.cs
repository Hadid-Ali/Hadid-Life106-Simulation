using System.Collections.Generic;

public static class SimulationHandler
{
   public static HashSet<GridCell> Run(HashSet<GridCell> liveCells, int iterations)
   {
      for (int i = 0; i < iterations; i++)
         liveCells = Step(liveCells);

      return liveCells;
   }
   
   private static HashSet<GridCell> Step(HashSet<GridCell> liveCells)
   {
      var neighbourCounts = new Dictionary<GridCell, int>(liveCells.Count * 4);

      foreach (GridCell cell in liveCells)
      {
         for (int dx = -1; dx <= 1; dx++)
         {
            if (!JumpFromCell(cell.X, dx, out long nx))
               continue;

            for (int dy = -1; dy <= 1; dy++)
            {
               if (dx == 0 && dy == 0)
                  continue;
               if (!JumpFromCell(cell.Y, dy, out long ny))
                  continue;

               var neighbour = new GridCell(nx, ny);
               neighbourCounts.TryGetValue(neighbour, out int count);
               neighbourCounts[neighbour] = count + 1;
            }
         }
      }

      var next = new HashSet<GridCell>();
      foreach (KeyValuePair<GridCell, int> entry in neighbourCounts)
      {
         int n = entry.Value;
         
         if (n == 3 || (n == 2 && liveCells.Contains(entry.Key)))
            next.Add(entry.Key);
      }

      return next;
   }
   
   private static bool JumpFromCell(long value, int delta, out long result)
   {
      if ((delta > 0 && value == long.MaxValue) || (delta < 0 && value == long.MinValue))
      {
         result = 0;
         return false;
      }

      result = value + delta;
      return true;
   }
}
