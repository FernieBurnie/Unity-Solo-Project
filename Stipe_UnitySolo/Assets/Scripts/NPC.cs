using UnityEngine;

public class NPC : MonoBehaviour
{
    //delete later if it doesn't work, testing dialogue systems

    bool firstInteraction = true;
    int repeatStartPosition;

    public string npcName;

    //try putting class before dialogue assest
    public DialogueAsset dialogueAssest;

    [HideInInspector]
    public int StartPosition
    {
        get
        {
            if (firstInteraction)
            {
                firstInteraction = false;
                return 0;
            }

            else
            {
                return repeatStartPosition;
            }
        }
    }
}