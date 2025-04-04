using NPC;
using NPC.NpcActions;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCActionData", menuName = "NPC/NPC Action Data")]
public class NPCActionData : ScriptableObject
{
    public NpcAction[] actions;
}

