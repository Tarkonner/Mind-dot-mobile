namespace UI
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	public class SafeAreaFitter : MonoBehaviour
	{
		[SerializeField] private RectTransform _rectTransform;
		[SerializeField] private CanvasScaler _canvasScaler;
		
		public event EventHandler<SafeAreaChangedEventArgs> onSafeAreaChanged;

		public class SafeAreaChangedEventArgs : EventArgs
		{
			public Vector2 offsetMin;
			public Vector2 offsetMax;
		}
		
		public Vector2 offsetMin => _rectTransform.offsetMin;
		public Vector2 offsetMax => _rectTransform.offsetMax;

		private Vector2? _defaultCanvasScalerResolution;
		public Vector2 defaultCanvasScalerResolution => _defaultCanvasScalerResolution ??= _canvasScaler.referenceResolution;

		private void Reset()
		{
			_rectTransform = GetComponent<RectTransform>();
		}

		private void Update()
		{
			var newSafeArea = Screen.safeArea;
			if (_appliedSafeArea != newSafeArea)
			{
				FitToSafeArea(newSafeArea);
			}
		}

		private Rect _appliedSafeArea;

		private void FitToSafeArea(Rect newSafeArea)
		{
			if (_rectTransform == null)
			{
				return;
			}

			// Update the size and position of the RectTransform
			var min = newSafeArea.position;
			var max = new Vector2(newSafeArea.position.x + newSafeArea.width - Screen.width, newSafeArea.position.y + newSafeArea.height - Screen.height);
			
			_rectTransform.offsetMin = min;
			_rectTransform.offsetMax = max;

			_appliedSafeArea = newSafeArea;

			_canvasScaler.referenceResolution = defaultCanvasScalerResolution + min - max;

			onSafeAreaChanged?.Invoke(this, new SafeAreaChangedEventArgs()
			{
				offsetMin = min,
				offsetMax = max
			});
		}
	}
}