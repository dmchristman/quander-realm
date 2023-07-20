using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BeauRoutine;
using System.Collections;
using System;

namespace Wrapper
{
    public class DialogueView : MonoBehaviour
    {
        #region Variables

        [Header("DialogueView Parts")]
        public Animator animator;

        [SerializeField] private TextMeshProUGUI dialogueBody;
        [SerializeField] private TextMeshProUGUI dupePage;
        [SerializeField] private Image contextImage; //todo: might have to work with container object instead of image

        [Space(10)]
        [SerializeField] private GameObject charMountLeft;
        [SerializeField] private Image charNameBGLeft;
        [SerializeField] private TextMeshProUGUI charNameTextLeft;

        [Space(10)]
        [SerializeField] private GameObject charMountRight;
        [SerializeField] private Image charNameBGRight;
        [SerializeField] private TextMeshProUGUI charNameTextRight;

        [Header("Character Prefabs")]
        [SerializeField] private GameObject mollyPrefab;
        [SerializeField] private GameObject tanglePrefab;
        [SerializeField] private GameObject bytePrefab;
        [SerializeField] private GameObject wolfiePrefab;
        [SerializeField] private GameObject waynePrefab;
        [SerializeField] private GameObject franPrefab;
        [SerializeField] private GameObject kenPrefab;
        [SerializeField] private GameObject franken_twinsPrefab;
        [SerializeField] private GameObject chefPrefab;

        private Dictionary<Character, GameObject> charDictionary; // todo: what about Speaker.None?
        private Color nameTextColorSpeaker = Color.white;
        private Color nameTextColorListener;

        private Color nameBGColorSpeaker = Color.white;
        private Color nameBGColorListener;

        private Character charLeft = Character.None;
        private Animator animatorCharLeft;

        private Character charRight = Character.None;
        private Animator animatorCharRight;
        
        private string contextImagePath;
        private string tempDialogueText; //todo: refactor this?

        // Main animator layer indexes
        private const int contextImagePosLayerIndex = 2;
        private const int contextImageShakeLayerIndex = 3;
        private const int leftCharAnimLayerIndex = 5;
        private const int rightCharAnimLayerIndex = 6;
        
        // Character animator layer indexes
        private const int charAnimExpressionLayerIndex = 2;
        private const int charAnimSpeakingLayerIndex = 3;

        #endregion

        #region Unity Functions, Opening/Closing

        private void Awake()
        {
            SetupListenerColors();
            InitCharacterDictionary();
        }

        private void OnEnable()
        {
            Events.OpenDialogueView += Open;
            Events.CloseDialogueView += Close;
            Events.UpdateDialogueView += UpdateView;
            Events.SwitchNextButton += SwitchNextButton;
        }

        private void OnDisable()
        {
            Events.OpenDialogueView -= Open;
            Events.CloseDialogueView -= Close;
            Events.UpdateDialogueView -= UpdateView;
            Events.SwitchNextButton -= SwitchNextButton;
        }

        private void Open(Dialogue dialogue)
        {
            dialogueBody.text = "";
            animator.SetBool("View/On", true);
            SwitchNextButton(false);
            Routine.Start(UpdateViewRoutine(dialogue, default, true));
        }

        private void Close()
        {
            Routine.Start(CloseRoutine());
            Events.DialogueSequenceEnded?.Invoke();
        }

        private IEnumerator CloseRoutine()
        {
            yield return ClearNoneCharacters(null, false);
            Events.TogglePreviousButton?.Invoke(false);
            animator.SetBool("View/On", false);
            yield return animator.WaitToCompleteAnimation();
            gameObject.SetActive(false);
        }

        #endregion

        #region Initialization

        private void InitCharacterDictionary()
        {
            charDictionary = new Dictionary<Character, GameObject>
            {
                { Character.Molly, mollyPrefab },
                { Character.Tangle, tanglePrefab },
                { Character.Byte, bytePrefab },
                { Character.Wolfie, wolfiePrefab },
                { Character.Wane, waynePrefab },
                { Character.Fran, franPrefab },
                { Character.Ken, kenPrefab },
                { Character.Franken_Twins, franken_twinsPrefab },
                { Character.Chef, chefPrefab }
            };
        }

        private void SetupListenerColors()
        {
            ColorUtility.TryParseHtmlString("#DAD5E0", out nameTextColorListener);
            ColorUtility.TryParseHtmlString("#BCB4C5", out nameBGColorListener);
        }

        #endregion

        #region Update Elements

        private void UpdateView(Dialogue dialogue, int step = 1)
        {
            Routine.Start(UpdateViewRoutine(dialogue, step));
        }

        private IEnumerator UpdateViewRoutine(Dialogue dialogue, int step = 1, bool initial = false)
        {
            //todo: rethink closing here
            if (dialogue == null)
            {
                Close();
                yield break;
            }

            yield return ClearNoneCharacters(dialogue, initial);
            yield return UpdateCharacters(dialogue);
            InitiateDialogueAnimation(dialogue, step);

            yield return Routine.Combine(
                UpdateExpressions(dialogue), 
                UpdateContextImage(dialogue), 
                UpdateSpeakerHighlight(dialogue));
        }

        private IEnumerator UpdateCharacters(Dialogue dialogue)
        {
            bool speakerIsNone = dialogue.speaker == Character.None;
            bool listenerIsNone = dialogue.listener == Character.None;

            bool speakerPresent = dialogue.speaker == charLeft || dialogue.speaker == charRight;
            bool listenerPresent = dialogue.listener == charLeft || dialogue.listener == charRight;
            
            if (!speakerIsNone && !speakerPresent)
            {
                if (listenerIsNone)
                    yield return NewCharacter(dialogue.speaker, Side.Left); // by default, speaker spawns on the left
                else
                {
                    switch (GetCharacterPosition(dialogue.listener))
                    {
                        case Side.Left:                                                 // listener on the left
                            yield return NewCharacter(dialogue.speaker, Side.Right);    // speaker spawns on the right
                            break;
                        case Side.Right:                                                // listener on the right
                        default:
                            yield return NewCharacter(dialogue.speaker, Side.Left);     // by default, speaker spawns on the left
                            break;
                    }
                }
            }

            if (!listenerIsNone && !listenerPresent)
            {
                if (speakerIsNone)
                    yield return NewCharacter(dialogue.listener, Side.Right); // by default, listener spawns on the right
                else
                {
                    // choose which side the LISTENER spawns based on where the speaker is
                    switch (GetCharacterPosition(dialogue.speaker))
                    {
                        case Side.Right:                                                // speaker on the right
                            yield return NewCharacter(dialogue.listener, Side.Left);    // listener spawns on the left
                            break;
                        case Side.Left:                                                 // speaker on the left
                        default:
                            yield return NewCharacter(dialogue.listener, Side.Right);   // by default, listener spawns on the right
                            break;
                    }
                }
            }
        }

        private IEnumerator NewCharacter(Character character, Side side)
        {
            if (side == Side.Left)
            {
                if (animator.GetBool("CharacterLeft/On"))
                {
                    animator.SetBool("CharacterLeft/On", false);
                    yield return animator.WaitToCompleteAnimation(leftCharAnimLayerIndex);
                }

                charLeft = character;
                ClearChildren(charMountLeft);
                charNameTextLeft.text = character.ToString().Replace("_", " ");
                GameObject charLeftGO = Instantiate(charDictionary[character], charMountLeft.transform);
                animatorCharLeft = charLeftGO.GetComponentInChildren<Animator>();
                
                animator.SetBool("CharacterLeft/On", true);
                animatorCharLeft.SetBool("Speaking", false);
            }
            else
            {
                if (animator.GetBool("CharacterRight/On"))
                {
                    animator.SetBool("CharacterRight/On", false);
                    yield return animator.WaitToCompleteAnimation(rightCharAnimLayerIndex);
                }

                charRight = character;
                ClearChildren(charMountRight);
                charNameTextRight.text = character.ToString().Replace("_", " ");
                GameObject charRightGO = Instantiate(charDictionary[character], charMountRight.transform);
                animatorCharRight = charRightGO.GetComponentInChildren<Animator>();
                
                animator.SetBool("CharacterRight/On", true);
                animatorCharRight.SetBool("Speaking", false);
                animatorCharRight.SetBool("CharacterRight", true);
            }
        }

        private IEnumerator UpdateSpeakerHighlight(Dialogue dialogue)
        {
            if (dialogue.speaker == charLeft)
            {
                if (dialogue.speaker != Character.None)
                {
                    animatorCharLeft.SetBool("Speaking", true);
                    charNameTextLeft.color = nameTextColorSpeaker;
                    charNameBGLeft.color = nameBGColorSpeaker;
                }

                if (dialogue.listener != Character.None)
                {
                    animatorCharRight.SetBool("Speaking", false);
                    charNameTextRight.color = nameTextColorListener;
                    charNameBGRight.color = nameBGColorListener;
                }
            }
            else
            {
                if (dialogue.speaker != Character.None)
                {
                    animatorCharRight.SetBool("Speaking", true);
                    charNameTextRight.color = nameTextColorSpeaker;
                    charNameBGRight.color = nameBGColorSpeaker;
                }

                if (dialogue.listener != Character.None)
                {
                    animatorCharLeft.SetBool("Speaking", false);
                    charNameTextLeft.color = nameTextColorListener;
                    charNameBGLeft.color = nameBGColorListener;
                }
            }

            if (animatorCharLeft != null)
                yield return animatorCharLeft.WaitToCompleteAnimation(charAnimSpeakingLayerIndex);

            if (animatorCharRight != null)
                yield return animatorCharRight.WaitToCompleteAnimation(charAnimSpeakingLayerIndex);
        }

        private IEnumerator UpdateExpressions(Dialogue dialogue)
        {
            if (dialogue.speaker == charLeft)
            {
                if (dialogue.speaker != Character.None)
                    animatorCharLeft.SetInteger("Expression", (int)dialogue.speakerExpression);

                if (dialogue.listener != Character.None)
                    animatorCharRight.SetInteger("Expression", (int)dialogue.listenerExpression);
            }
            else
            {
                if (dialogue.listener != Character.None)
                    animatorCharLeft.SetInteger("Expression", (int)dialogue.listenerExpression);

                if (dialogue.speaker != Character.None)
                    animatorCharRight.SetInteger("Expression", (int)dialogue.speakerExpression);
            }

            if (animatorCharLeft != null)
                yield return animatorCharLeft.WaitToCompleteAnimation(charAnimExpressionLayerIndex);

            if (animatorCharRight != null)
                yield return animatorCharRight.WaitToCompleteAnimation(charAnimExpressionLayerIndex);
        }

        private IEnumerator UpdateContextImage(Dialogue dialogue)
        {
            if (dialogue.contextImagePath == string.Empty)
            {
                animator.SetBool("ContextImage/On", false);
                yield break;
            }

            Sprite sprite = Resources.Load<Sprite>(dialogue.contextImagePath);
            if (sprite == null)
            {
                Debug.LogFormat("Could not find image at path: {0}", dialogue.contextImagePath);
                yield break;
            }

            contextImage.sprite = sprite;

            animator.SetBool("ContextImage/On", true);

            yield return Routine.Combine(
                animator.WaitToCompleteAnimation(contextImagePosLayerIndex),
                animator.WaitToCompleteAnimation(contextImageShakeLayerIndex));
        }

        /// <summary>
        /// Start the first step of the dialogue snimation, the second step and dialogue body text switching are 
        /// triggered by an animation event. 
        /// </summary>
        /// <param name="step">positive = moving forward, negative = moving backward - in the dialogue sequence</param>
        private void InitiateDialogueAnimation(Dialogue dialogue, int step)
        {
            if (step > 0)
            {
                SetDupeText(dialogue.text);
                animator.SetTrigger("DialogueNext");
            }
            else
            {
                SetDupeText(dialogueBody.text);
                tempDialogueText = dialogue.text;
                animator.SetTrigger("DialoguePrevious");
            }
        }

        /// <summary>
        /// Triggered by an animation event, this method switches dialogue text on the UI and triggeres the second part
        /// of the animation
        /// </summary>
        public void SwitchDialogueBody(int step)
        {
            if (step > 0) //Next
            {
                SetDialogText(dupePage.text);
                animator.SetTrigger("DialogueNext");
            }
            else
            {
                SetDialogText(tempDialogueText);
                animator.SetTrigger("DialoguePrevious");
            }
        }

        /// <summary>
        /// Switch the Next button image to either "dismiss" or "next", depending on the current dialogue being the 
        /// last or not
        /// </summary>
        private void SwitchNextButton(bool isLast)
        {
            animator.SetBool("DialogueLast", isLast);
        }

        #endregion

        private void ClearChildren(GameObject mount)
        {
            foreach(Transform child in mount.transform)
                Destroy(child.gameObject);
        }

        private Side GetCharacterPosition(Character character)
        {
            Side side = Side.None;

            if (character == charLeft)
                side = Side.Left;
            else if (character == charRight)
                side = Side.Right;

            return side;
        }

        private IEnumerator ClearNoneCharacters(Dialogue dialogue, bool initial)
        {
            if (dialogue == null)
            {
                animator.SetBool("CharacterLeft/On", false);
                charLeft = Character.None;
                charNameTextLeft.text = "";
                animatorCharLeft = null;

                animator.SetBool("CharacterRight/On", false);
                charRight = Character.None;
                charNameTextRight.text = "";
                animatorCharRight = null;

                yield break;
            }

            if (charLeft != dialogue.speaker && charLeft != dialogue.listener && animator.GetBool("CharacterLeft/On"))
            {
                animator.SetBool("CharacterLeft/On", false);
                charLeft = Character.None;
                charNameTextLeft.text = "";
                animatorCharLeft = null;
            }

            if (charRight != dialogue.speaker && charRight != dialogue.listener && animator.GetBool("CharacterRight/On"))
            {
                animator.SetBool("CharacterRight/On", false);
                charRight = Character.None;
                charNameTextRight.text = "";
                animatorCharRight = null;
            }

            if (initial) yield return null;
            else 
                yield return Routine.Combine(
                    animator.WaitToCompleteAnimation(leftCharAnimLayerIndex),
                    animator.WaitToCompleteAnimation(rightCharAnimLayerIndex));
        }

        void SetDupeText(string text)
        {
            if (text.Contains("*this part of town*"))
            {
                // replace with minigame name
                var game = Events.GetMinigameTitle.Invoke(Events.GetCurrentGame.Invoke());
                text = text.Replace("*this part of town*", game);
            }
            dupePage.text = text;
        }

        void SetDialogText(string text)
        {
            if (text.Contains("*this part of town*"))
            {
                // replace with minigame name
                var game = Events.GetMinigameTitle.Invoke(Events.GetCurrentGame.Invoke());
                text = text.Replace("*this part of town*", game);
            }
            dialogueBody.text = text;
        }
    }
}
