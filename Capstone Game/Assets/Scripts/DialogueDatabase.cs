using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Dialogue Database", menuName = "Dialogue/Database")]
public class DialogueDatabase : ScriptableObject
{
    public Dialogue[] dialogues;
}
