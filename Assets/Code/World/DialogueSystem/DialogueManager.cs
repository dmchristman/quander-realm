using BeauRoutine;
using System;
using System.Collections.Generic;
using UnityEngine;
using BeauRoutine;

namespace Wrapper
{
    public class DialogueManager : MonoBehaviour
    {
        public DialogueView dialogueView = null;

        private const string dialoguePath = "_Wrapper/Dialogue";
        private const int dialogueViewLayerIndex = 1;
        private DialogueSequence currentSequence = null;
        private Dictionary<string, DialogueSequence> dialogueDictionary;
        Routine delayInteraction;
        [SerializeField]
        float delayTime = 1F;

        #region Unity Functions

        private void Awake()
        {
            InitDialogueDictionary();
        }

        private void OnEnable()
        {
            Events.PrintDialogue += PrintSequence;
            Events.StartDialogueSequence += OpenDialogueView;
            Events.ChangeDialogue += UpdateDialogueNumber;
        }

        private void OnDisable()
        {
            Events.PrintDialogue -= PrintSequence;
            Events.StartDialogueSequence -= OpenDialogueView;
            Events.ChangeDialogue -= UpdateDialogueNumber;
        }

        #endregion

        private void InitDialogueDictionary()
        {
            dialogueDictionary = new Dictionary<string, DialogueSequence>();
            Dialogue[] allDialogue = Resources.LoadAll<Dialogue>(dialoguePath);

            foreach (Dialogue node in allDialogue)
            {
                if (dialogueDictionary.ContainsKey(node.sequenceID))
                    dialogueDictionary[node.sequenceID].Add(node);
                else
                    dialogueDictionary.Add(node.sequenceID, new DialogueSequence(node));
            }

            Events.SortSequences?.Invoke();
        }

        private void OpenDialogueView(string sequenceID)
        {
            if (!(dialogueDictionary.ContainsKey(sequenceID)))
            {
                Debug.LogFormat("Could not find dialogue sequence with ID {0}", sequenceID);
                return;
            }

            if (dialogueView.animator.GetBool("View/On"))
            {
                dialogueView.animator.SetBool("View/On", false);
                dialogueView.animator.WaitToCompleteAnimation(dialogueViewLayerIndex);
            }

            currentSequence = dialogueDictionary[sequenceID];
            dialogueView.gameObject.SetActive(true);
            Events.OpenDialogueView?.Invoke(currentSequence.GetFirst());
        }

        private void UpdateDialogueNumber(int step)
        {
            if (delayInteraction.Exists()) return;

            if (step == 0) Events.CloseDialogueView?.Invoke();
            else Events.UpdateDialogueView?.Invoke(currentSequence.GetLine(step), step);
            delayInteraction.Replace(DelayInteraction());
        }

        private void PrintSequence(string sequenceID)
        {
            foreach (Dialogue node in dialogueDictionary[sequenceID].Nodes)
                Debug.LogFormat("Num: {0} Text: {1}", node.num, node.text);
        }

        System.Collections.IEnumerator DelayInteraction()
        {
            yield return delayTime;
        }
    } 
}
