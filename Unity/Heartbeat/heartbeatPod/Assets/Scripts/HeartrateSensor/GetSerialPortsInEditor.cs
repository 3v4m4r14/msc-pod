using UnityEditor;
using System.IO.Ports;

[CustomEditor(typeof(GetIbiRate))]
public class GetSerialPortsInEditor : Editor
{
    string[] _choices;
    int _choiceIndex = 0;


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        int choiceIndex = -1;
        if(_choices != null)
        {
            if (_choices.Length > 0)
            {
                choiceIndex = EditorGUILayout.Popup(_choiceIndex, _choices);
            }
        }
        
        if(choiceIndex > -1)
        {
            _choiceIndex = choiceIndex;
        }
            

        
        var someClass = target as GetIbiRate;
        if(_choices != null)
        {
            if (_choices.Length > 0)
            {
                someClass.choice = _choices[_choiceIndex];
            }
        }
        
       


        EditorUtility.SetDirty(target);

        if (UnityEngine.GUILayout.Button("update ports"))
        {
            GetPorts();
        }


    }
    private void GetPorts()
    {
        _choices = SerialPort.GetPortNames();
    }
}


