using UnityEngine;

[CreateAssetMenu(fileName = "ChatLineDatabase", menuName = "Lumae/Chat Line Database", order = 1)]
public class ChatLineDatabase : ScriptableObject
{
    public string[] userNames;
    public string[] chatSamples;
}