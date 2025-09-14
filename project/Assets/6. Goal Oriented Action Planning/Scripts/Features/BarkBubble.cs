using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Sammoh.GOAP
{
    [RequireComponent(typeof(Canvas))]
        [RequireComponent(typeof(GraphicRaycaster))]
        [RequireComponent(typeof(CanvasGroup))]
        [DisallowMultipleComponent]

    public class BarkBubble : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Text titleText;

        [SerializeField] private Text bodyText;
        [SerializeField] private RectTransform bubbleRoot; // The panel we size/move (can be this RectTransform)
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Behavior")] [Tooltip("World-space offset from the parent/agent this bubble is attached to.")]
        public Vector3 worldOffset = new Vector3(0f, 2.0f, 0f);

        [Tooltip("How far upward (in meters) the bubble drifts while visible.")]
        public float floatUpDistance = 0.25f;

        [Tooltip("Seconds to fade in and out.")]
        public float fadeInDuration = 0.25f;

        public float fadeOutDuration = 0.25f;

        [Tooltip("Auto-disable the GameObject after fade-out.")]
        public bool disableAfterHide = true;

        Transform _followTarget;
        Vector3 _baseWorldPos;
        Coroutine _routine;
        bool _isShowing;

        void Reset()
        {
            var c = GetComponent<Canvas>();
            c.renderMode = RenderMode.WorldSpace;
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            if (bubbleRoot == null) bubbleRoot = transform as RectTransform;
        }

        void Awake()
        {
            var c = GetComponent<Canvas>();
            if (c != null) c.renderMode = RenderMode.WorldSpace;

            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            if (bubbleRoot == null) bubbleRoot = transform as RectTransform;

            // Try to discover texts if not assigned
            if (titleText == null || bodyText == null)
            {
                var texts = GetComponentsInChildren<Text>(true);
                foreach (var t in texts)
                {
                    if (titleText == null && t.gameObject.name.ToLower().Contains("title")) titleText = t;
                    if (bodyText == null && (t.gameObject.name.ToLower().Contains("body") ||
                                             t.gameObject.name.ToLower().Contains("message"))) bodyText = t;
                }
            }

            canvasGroup.alpha = 0f;
            _isShowing = false;

            // Default follow target is parent
            _followTarget = transform.parent;
            _baseWorldPos = GetTargetWorldPos();
            transform.position = _baseWorldPos + worldOffset;
        }

        void LateUpdate()
        {
            // Keep billboard facing via a separate BillboardToCamera, this only follows the parent
            if (_followTarget != null && !_isShowing)
            {
                _baseWorldPos = GetTargetWorldPos();
                transform.position = _baseWorldPos + worldOffset;
            }
        }

        Vector3 GetTargetWorldPos()
        {
            return _followTarget != null ? _followTarget.position : transform.position;
        }

        public void SetFollowTarget(Transform target)
        {
            _followTarget = target;
            _baseWorldPos = GetTargetWorldPos();
        }

        public void ShowBark(string message, float durationSeconds, string title = null)
        {
            if (bodyText != null) bodyText.text = message;
            if (!string.IsNullOrEmpty(title) && titleText != null) titleText.text = title;

            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }

            gameObject.SetActive(true);
            _routine = StartCoroutine(Co_ShowHide(durationSeconds));
        }

        public void HideImmediate()
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }

            canvasGroup.alpha = 0f;
            _isShowing = false;

            // Reset to base position
            transform.position = _baseWorldPos + worldOffset;
            if (disableAfterHide) gameObject.SetActive(false);
        }

        IEnumerator Co_ShowHide(float visibleDuration)
        {
            _isShowing = true;
            _baseWorldPos = GetTargetWorldPos();
            float elapsed = 0f;

            // Fade in
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / fadeInDuration);
                canvasGroup.alpha = t;
                // Slight initial rise
                transform.position = Vector3.Lerp(_baseWorldPos + worldOffset,
                    _baseWorldPos + worldOffset + Vector3.up * (floatUpDistance * 0.25f), t);
                yield return null;
            }

            canvasGroup.alpha = 1f;

            // Visible + drift upward
            float driftElapsed = 0f;
            float driftDuration = Mathf.Max(0.001f, visibleDuration);
            while (driftElapsed < driftDuration)
            {
                driftElapsed += Time.deltaTime;
                float t = Mathf.Clamp01(driftElapsed / driftDuration);
                transform.position = Vector3.Lerp(_baseWorldPos + worldOffset + Vector3.up * (floatUpDistance * 0.25f),
                    _baseWorldPos + worldOffset + Vector3.up * floatUpDistance, t);
                yield return null;
            }

            // Fade out
            elapsed = 0f;
            Vector3 endPos = _baseWorldPos + worldOffset + Vector3.up * floatUpDistance;
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / fadeOutDuration);
                canvasGroup.alpha = 1f - t;
                // Nudge a tiny bit more upward on fade
                transform.position = Vector3.Lerp(endPos, endPos + Vector3.up * 0.05f, t);
                yield return null;
            }

            canvasGroup.alpha = 0f;

            // Reset and finalize
            _isShowing = false;
            transform.position = _baseWorldPos + worldOffset;
            if (disableAfterHide) gameObject.SetActive(false);
            _routine = null;
        }

        // Quick test from context menu in Inspector
        [ContextMenu("Test Show Bark (2s)")]
        void TestShow()
        {
            ShowBark("Your bark message here!", 2f, "Title");
        }
    }
}