/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.UI.Extensions.Examples.FancyScrollViewExample06
{
    class Example06 : MonoBehaviour
    {
        [SerializeField] ScrollView scrollView = default;
        [SerializeField] Window[] windows = default;
        [SerializeField] Button prevCellButton = default;
        [SerializeField] Button nextCellButton = default;
        Window currentWindow;

        void Start()
        {
            StartCoroutine(LateStart());
        }

        IEnumerator LateStart()
        {
            yield return null;
            windows = GetComponentsInChildren<Window>();
            prevCellButton.onClick.AddListener(scrollView.SelectPrevCell);
            nextCellButton.onClick.AddListener(scrollView.SelectNextCell);
            
            foreach (var window in windows)
            {
                window.gameObject.SetActive(false);
            }

            scrollView.OnSelectionChanged(OnSelectionChanged);

            var items = Enumerable.Range(0, windows.Length)
                .Select(i => new ItemData($"Tab {i}"))
                .ToList();

            scrollView.UpdateData(items);
            scrollView.SelectCell(0);
        }

        void OnSelectionChanged(int index, MovementDirection direction)
        {
            if (currentWindow != null)
            {
                currentWindow.Out(direction);
                currentWindow = null;
            }

            if (index >= 0 && index < windows.Length)
            {
                currentWindow = windows[index];
                currentWindow.In(direction);
            }
        }
    }
}
