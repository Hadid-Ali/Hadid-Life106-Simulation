using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GameController : MonoBehaviour
{
   [SerializeField] private TextAsset _inputText;
   [SerializeField] private int _iterations = 10;
   [SerializeField] private bool _runOnStart = true;
   [SerializeField] private string _outputPath = "GameIO/Output/output.txt";

   private void Start()
   {
      if (_runOnStart)
         Simulate();
   }

   public void Simulate()
   {
      Run(_inputText);
   }
   
   public void Run(TextAsset textAsset)
   {
      if (textAsset == null)
      {
         Debug.LogError("[GameOfLife] No input TextAsset assigned.");
         return;
      }

      HashSet<GridCell> liveCells = FileIOComponent.ParseLife106(textAsset.text);
      Debug.Log($"[GameOfLife] Parsed {liveCells.Count} live cells from '{textAsset.name}'.");

      liveCells = SimulationHandler.Run(liveCells, _iterations);

      string life106 = FileIOComponent.ConvertToLife106(liveCells);
      Debug.Log($"{_iterations} iteration(s): {liveCells.Count} live cells.");
      Debug.Log(life106);
      WriteOutput(life106);
   }

   private void WriteOutput(string contents)
   {
      string fullPath = Path.Combine(Application.dataPath, _outputPath);
      string dir = Path.GetDirectoryName(fullPath);
      if (!string.IsNullOrEmpty(dir))
         Directory.CreateDirectory(dir);

      File.WriteAllText(fullPath, contents);
      Debug.Log($"[GameOfLife] Wrote result to {fullPath}");

#if UNITY_EDITOR
      AssetDatabase.Refresh();      
#endif
   }
}
