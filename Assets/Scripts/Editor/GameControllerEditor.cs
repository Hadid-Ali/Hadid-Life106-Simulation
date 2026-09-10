using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameController))]
public class GameControllerEditor : Editor
{
    private GameController _gameController;

    void OnEnable()
    {
        _gameController = target as GameController;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Simulate"))
        {
            _gameController.Simulate();
        }
    }
}
