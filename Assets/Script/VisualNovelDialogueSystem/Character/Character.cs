using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Dialouge;
using TMPro;
using UnityEngine.Video;
using UnityEditor;


namespace Characters
{
    public abstract class Character 
    {
        public const bool enableOnStart = false;

        private const float unHighlightedDarkenStrength = 0.65f;

        public const bool defaultOriantationIsFacingLeft = true;
        public Color color { get; protected set; } = Color.white;
        protected Color displayColor => highlighted ? highlightedColor : unhighlightedColor;
        protected Color highlightedColor => color;
        protected Color unhighlightedColor => new Color(color.r * unHighlightedDarkenStrength, color.g * unHighlightedDarkenStrength, color.b * unHighlightedDarkenStrength, color.a);
        public bool highlighted { get; protected set; } = true;
        public Animator animator;
        public string name = "";
        public RectTransform root = null;

        public string displayName = "";
        public CharacterConfigData config;
        protected CharacterManager characterManager => CharacterManager.instance;
        public DialougeSystem dialougeSystem => DialougeSystem.instance;
        protected bool facingLeft = defaultOriantationIsFacingLeft;
        public int priority { get; private set; }
        public Vector2 targetPosition { get; private set; }
        protected Coroutine co_changingColor;
        protected Coroutine co_revealing, co_hinding;
        protected Coroutine co_moving;
        protected Coroutine co_highlighting;
        protected Coroutine co_flipping;
        public bool isChangingColor => co_changingColor != null;
        public bool isMoving => co_moving != null;
        public bool isRevealing => co_revealing != null;
        public bool isHinding => co_hinding != null;
        public bool isHighlighting => (highlighted && co_highlighting != null);
        public bool isUnHighlighting => (!highlighted && co_highlighting != null);
        public virtual bool isVisible { get; set; }
        public bool isFacingLeft => facingLeft;
        public bool isFacingRight => !facingLeft;
        public bool isFlipping => co_flipping != null;
        public Character(string name, CharacterConfigData config, GameObject prefab)
        {
            this.name = name;
            displayName = name;
            this.config = config;   

            if (prefab != null )
            {
                GameObject ob = Object.Instantiate(prefab, characterManager.characterPanel);
                ob.name = characterManager.FormatCharacterPath(characterManager.charaterPrefabNameFormat, name);
                ob.SetActive(true);
                root = ob.GetComponent<RectTransform>();
                animator = root.GetComponentInChildren<Animator>();
            }
        }

        public Character(RectTransform root)
        {
            this.root = root;
        }

        public Coroutine Say(string dialogue) => Say(new List<string> { dialogue});
       
        public Coroutine Say(List<string> dialogue)
        {
            dialougeSystem.ShowSpeakerName(displayName);
            UpdateTextCustomizationOnScreen();
            return dialougeSystem.Say(dialogue);
        }
        public void SetNameColor(Color color) => config.nameColor = color;
        public void SetDialogueColor(Color color) => config.dialogueColor = color;
        public void SetNameFont(TMP_FontAsset font) => config.nameFont = font;
        public void SetDialogueFont(TMP_FontAsset font) => config.dialogueFont = font;
        public void ResetConfigurationData() => config = CharacterManager.instance.GetCharacterConfig(name, getOriginal: true); 
        public void UpdateTextCustomizationOnScreen() => dialougeSystem.ApplySpeakerDataToDialogueContainer(config);

        public virtual Coroutine Show(float speedMultiplier = 1f)
        {
            if(isRevealing)
            {
                characterManager.StopCoroutine(co_revealing) ;
            }

            if (isHinding)
            {
                characterManager.StopCoroutine(co_hinding);
            }

            co_revealing = characterManager.StartCoroutine(ShowingOrHinding(true, speedMultiplier));

            return co_revealing;
        }
        public virtual Coroutine Hide(float speedMultiplier = 1f)
        {
            if (isHinding)
            {
                characterManager.StopCoroutine(co_hinding);
            }

            if (isRevealing)
            {
                characterManager.StopCoroutine(co_revealing);
            }

            co_hinding = characterManager.StartCoroutine(ShowingOrHinding(false, speedMultiplier));

            return co_hinding;
        }
        public virtual IEnumerator ShowingOrHinding(bool show, float speedMultiplier = 1f)
        {
            Debug.Log("show/hide cant be called from a base character type");
            yield return null;
        }
        public virtual void SetPosition(Vector2 position)
        {
            if(root == null)
            {
                return;
            }

            (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUITargetPositionToRelativeCharacterAnchorTargets(position);

            root.anchorMin = minAnchorTarget;
            root.anchorMax = maxAnchorTarget;

            targetPosition = position;
        }
        public virtual Coroutine MoveToPosition(Vector2 position, float speed = 2f, bool smooth = false)
        {
            if(root == null)
            {
                return null;
            }

            if(isMoving)
            {
                characterManager.StopCoroutine(co_moving);
            }

            co_moving = characterManager.StartCoroutine(MovingToPosition(position, speed, smooth));

            targetPosition = position;

            return co_moving;
        }
        private IEnumerator MovingToPosition(Vector2 position, float speed = 2f, bool smooth = false)
        {
            (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUITargetPositionToRelativeCharacterAnchorTargets(position);
            Vector2 padding = root.anchorMax - root.anchorMin;

            while(root.anchorMin != minAnchorTarget || root.anchorMax != maxAnchorTarget)
            {
                root.anchorMin = smooth ? Vector2.Lerp(root.anchorMin, minAnchorTarget, speed * Time.deltaTime) : Vector2.MoveTowards(root.anchorMin, minAnchorTarget, speed * Time.deltaTime * 0.35f);

                root.anchorMax = root.anchorMin + padding;

                if (smooth && Vector2.Distance(root.anchorMin, minAnchorTarget) <= 0.001f)
                {
                    root.anchorMin = minAnchorTarget;
                    root.anchorMax = maxAnchorTarget;
                    break;
                }

                yield return null;
            }

            Debug.Log("done moving");
            co_moving = null;
        }
        protected (Vector2, Vector2) ConvertUITargetPositionToRelativeCharacterAnchorTargets(Vector2 position)
        {
            Vector2 padding = root.anchorMax - root.anchorMin;

            float maxX = 1f - padding.x;
            float maxY = 1f - padding.y;

            Vector2 minAnchorTarget = new Vector2(maxX * position.x, maxY * position.y);
            Vector2 maxAnchorTarget = minAnchorTarget + padding;

            return (minAnchorTarget, maxAnchorTarget);
        }
        public virtual void SetColor(Color color)
        {
            this.color = color;
        }
        public Coroutine TransitionColor(Color color, float speed = 1f)
        {
            this.color = color;

            if(isChangingColor)
            {
                characterManager.StopCoroutine(co_changingColor);
            }

            co_changingColor = characterManager.StartCoroutine(ChangingColor(displayColor, speed));

            return co_changingColor;
        }

        public virtual IEnumerator ChangingColor(Color color, float speed)
        {
            Debug.Log("color changing is not applicable on this character");
            yield return null;
        }
        public Coroutine Highlight(float speed = 1f, bool immediate = false)
        {
            if (isHighlighting || isUnHighlighting)
            {
                characterManager.StopCoroutine(co_highlighting);
            }
           
            highlighted = true;
            co_highlighting = characterManager.StartCoroutine(HighLighting(speed, immediate));

            return co_highlighting;
        }
        public Coroutine UnHighlight(float speed = 1f, bool immediate = false)
        {
            if (isHighlighting || isUnHighlighting)
            {
                characterManager.StopCoroutine(co_highlighting);
            }

            highlighted = false;
            co_highlighting = characterManager.StartCoroutine(HighLighting(speed, immediate));

            return co_highlighting;
        }
        public virtual IEnumerator HighLighting(float speedMultiplier, bool immediate = false)
        {
            Debug.Log("highlighting changing is not available on this character");
            yield return null;
        }
        public Coroutine Flip(float speed = 1, bool immediate = false)
        {
            if (isFacingLeft)
            {
                return FaceRight(speed, immediate);
            }
            else
            {
                return FaceLeft(speed, immediate);
            }
        }
        public Coroutine FaceLeft(float speed = 1, bool immediate = false)
        {
            if (isFlipping)
            {
                characterManager.StopCoroutine(co_flipping);
            }

            facingLeft = true;
            co_flipping = characterManager.StartCoroutine(FaceDirection(facingLeft, speed, immediate));

            return co_flipping;
        }
        public Coroutine FaceRight(float speed = 1, bool immediate = false)
        {
            if (isFlipping)
            {
                characterManager.StopCoroutine(co_flipping);
            }

            facingLeft = false;
            co_flipping = characterManager.StartCoroutine(FaceDirection(facingLeft, speed, immediate));

            return co_flipping;
        }
        public virtual IEnumerator FaceDirection(bool faceLeft, float speedMultiplier, bool immediate)
        {
            Debug.Log("can not flip a character of this type");
            yield return null;
        }
        public void SetPriority(int priority, bool autoSortCharactersOnUI = true)
        {
            this.priority = priority;

            if (autoSortCharactersOnUI)
            {
                characterManager.SortCharacters();
            }
        }
        public virtual void OnReciveCastingExpresstion(int layer, string expression)
        {
            return;
        }
        public enum CharacterType
        {
            Text,
            Sprite,
            SpriteSheet,
            Live2D,
            Model3D
        }
        
    }
}