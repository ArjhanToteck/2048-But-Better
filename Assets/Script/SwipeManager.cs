using UnityEngine;

public class SwipeManager : MonoBehaviour
{
    public enum SwipeDirection
	{
        None = 0,
        Left = 1,
        Right = 2,
        Up = 4,
        Down = 8
	}

	public SwipeDirection swipeDirection;
    public Vector3 touchPosition;
    public Vector2 swipeResistance;

	void Update()
	{
		swipeDirection = SwipeDirection.None;

		if (Input.GetMouseButtonDown(0))
		{
            touchPosition = Input.mousePosition;
		}

		if (Input.GetMouseButtonUp(0))
		{
			Vector2 deltaSwipe = touchPosition - Input.mousePosition;

			if(Mathf.Abs(deltaSwipe.x) > swipeResistance.x && Mathf.Abs(deltaSwipe.x) > Mathf.Abs(deltaSwipe.y))
			{
				swipeDirection = (deltaSwipe.x < 0) ? SwipeDirection.Right : SwipeDirection.Left;
			}
			else if (Mathf.Abs(deltaSwipe.y) > swipeResistance.y && Mathf.Abs(deltaSwipe.y) > Mathf.Abs(deltaSwipe.x))
			{
				swipeDirection = (deltaSwipe.y < 0) ? SwipeDirection.Up : SwipeDirection.Down;
			}
		}
	}
}
