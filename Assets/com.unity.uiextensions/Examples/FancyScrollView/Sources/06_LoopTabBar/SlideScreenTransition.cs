using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI.Extensions.Examples.FancyScrollViewExample06
{
    class SlideScreenTransition : MonoBehaviour
    {
        [SerializeField] RectTransform targetTransform = default;
        [SerializeField] GraphicRaycaster graphicRaycaster = default;
        [SerializeField] CanvasGroup canvasGroup = default; // Alpha 조절을 위해 추가

        const float Duration = 0.3f;

        bool shouldAnimate, isOutAnimation;
        float timer, startX, endX;
        float startAlpha, endAlpha; // Alpha 시작/끝 값 저장을 위해 추가

        public void In(MovementDirection direction) => Animate(direction, false);

        public void Out(MovementDirection direction) => Animate(direction, true);

        void Animate(MovementDirection direction, bool isOut)
        {
            if (shouldAnimate) return;

            timer = Duration;
            isOutAnimation = isOut;
            shouldAnimate = true;
            graphicRaycaster.enabled = false;

            if (!isOutAnimation)
            {
                gameObject.SetActive(true);
            }

            switch (direction)
            {
                case MovementDirection.Left:
                    endX = -targetTransform.rect.width;
                    break;
                case MovementDirection.Right:
                    endX = targetTransform.rect.width;
                    break;
                default:
                    Debug.LogWarning("Example only support horizontal direction.");
                    break;
            }

            // 위치 설정
            startX = isOutAnimation ? 0 : -endX;
            endX = isOutAnimation ? endX : 0;

            // Alpha 설정: In일 때는 0->1, Out일 때는 1->0
            startAlpha = isOutAnimation ? 1f : 0f;
            endAlpha = isOutAnimation ? 0f : 1f;

            UpdatePosition(0f);
        }

        void Update()
        {
            if (!shouldAnimate) return;

            timer -= Time.deltaTime;

            if (timer > 0)
            {
                UpdatePosition(1f - timer / Duration);
                return;
            }

            shouldAnimate = false;
            graphicRaycaster.enabled = true;

            if (isOutAnimation)
            {
                gameObject.SetActive(false);
            }

            UpdatePosition(1f);
        }

        void UpdatePosition(float position)
        {
            // 1. 위치 이동 (Lerp)
            var x = Mathf.Lerp(startX, endX, position);
            targetTransform.anchoredPosition = new Vector2(x, targetTransform.anchoredPosition.y);

            // 2. Alpha 값 조절 (Lerp)
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, position);
            }
        }
    }
}