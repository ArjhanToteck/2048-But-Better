using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{

	public Vector2Int position;

	public bool recentlyMerged = false;

	public long value = 2;
	public Color color = Color.red;

	Material material;

	object targetPosition = null;
	bool destroyAfterMotion = false;

	void Start()
	{
		// 1/10 chance
		if(Random.Range(0, 10) == 0)
		{
			// spawns the tile with a value of four rather than 2
			UpdateValue(false, 4, false);
		}

		// triggers grow animation
		GetComponent<Animator>().SetTrigger("Grow");

		// clones material
		material = new Material(gameObject.GetComponent<MeshRenderer>().material);
		gameObject.GetComponent<MeshRenderer>().material = material;
	}

	public void UpdateValue(bool animate = true, long targetValue = -1, bool addToScore = true)
	{
		// triggers update animation
		if(animate) GetComponent<Animator>().SetTrigger("Update");

		long repeatUntil = targetValue == -1 ? value * 2 : targetValue;

		for(int i = 0; value < repeatUntil; i++)
        {
			// updates value and text for value
			value *= 2;
			if(addToScore) FindObjectOfType<Game>().AddToScore(value);
			transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = value.ToString();

			// updates color
			const float increment = 0.2f;

			// regulates colors
			if (color.r < increment) color.r = 0;
			if (color.g < increment) color.g = 0;
			if (color.b < increment) color.b = 0;

			if (color.r > 1 - increment) color.r = 1;
			if (color.g > 1 - increment) color.g = 1;
			if (color.b > 1 - increment) color.b = 1;

			// color hasn't reached a full color
			if (color.r < 1 && color.g == 0 && color.b == 0)
			{
				color.r += increment;
			}
			else
			{
				// red is currently at 1 and green is 0
				if (color.r == 1 && color.g < 1 && color.b == 0)
				{
					color.g += increment;
				}
				else
				{
					// green is currently at 1, blue is 0, and red is more than 0
					if (color.g == 1 && color.b == 0 && color.r > 0)
					{
						color.r -= increment;
					}
					else
					{
						// green is currently at 1, blue is 0, and red is 0
						if (color.g == 1 && color.b < 1 && color.r == 0)
						{
							color.b += increment;
						}
						else
						{
							// blue is currently at 1, red is 0, and green is more than 0
							if (color.b == 1 && color.r == 0 && color.g > 0)
							{
								color.g -= increment;
							}
							else
							{
								// blue is greater than 0, red is 0, and green is 0
								if (color.b > 0 && color.r == 0 && color.g == 0)
								{
									color.b -= increment;
								}
							}
						}
					}
				}
			}

			// shows new color
			gameObject.GetComponent<MeshRenderer>().material.color = color;
		}		
	}

	public void MoveUp(float distance, GameObject otherTile = null)
	{
		StopMoving();

		float originalZ = transform.position.z;

		targetPosition = new Vector3(transform.position.x, transform.position.y, originalZ + distance);
		destroyAfterMotion = !!otherTile;

		// starts coroutine
		StartCoroutine(MoveUpCoroutine());

		IEnumerator MoveUpCoroutine()
		{

			while (transform.position.z <= originalZ + distance)
			{
				transform.position += new Vector3(0, 0, SaveData.Settings.tileSpeed * 20f * Time.deltaTime);

				// waits till next frame
				yield return new WaitForEndOfFrame();
			}

			// perfects to intended position
			transform.position = new Vector3(transform.position.x, transform.position.y, originalZ + distance);

			// crashes into other tile if it exists
			if (!!otherTile) CrashIntoTile(otherTile);
		}
	}

	public void MoveDown(float distance, GameObject otherTile = null)
	{
		StopMoving();

		float originalZ = transform.position.z;

		targetPosition = new Vector3(transform.position.x, transform.position.y, originalZ - distance);
		destroyAfterMotion = !!otherTile;

		// starts coroutine
		StartCoroutine(MoveDownCoroutine());

		IEnumerator MoveDownCoroutine()
		{

			while (transform.position.z >= originalZ - distance)
			{
				transform.position -= new Vector3(0, 0, SaveData.Settings.tileSpeed * 20f * Time.deltaTime);

				// waits till next frame
				yield return new WaitForEndOfFrame();
			}

			// perfects to intended position
			transform.position = new Vector3(transform.position.x, transform.position.y, originalZ - distance);

			// crashes into other tile if it exists
			if (!!otherTile) CrashIntoTile(otherTile);
		}
	}

	public void MoveLeft(float distance, GameObject otherTile = null)
	{
		StopMoving();


		float originalX = transform.position.x;

		targetPosition = new Vector3(originalX - distance, transform.position.y, transform.position.z);
		destroyAfterMotion = !!otherTile;

		// starts coroutine
		StartCoroutine(MoveLeftCoroutine());

		IEnumerator MoveLeftCoroutine()
		{

			while (transform.position.x >= originalX - distance)
			{
				transform.position -= new Vector3(SaveData.Settings.tileSpeed * 20f * Time.deltaTime, 0, 0);

				// waits till next frame
				yield return new WaitForEndOfFrame();
			}

			// perfects to intended position
			transform.position = new Vector3(originalX - distance, transform.position.y, transform.position.z);

			// crashes into other tile if it exists
			if (!!otherTile) CrashIntoTile(otherTile);
		}
	}

	public void MoveRight(float distance, GameObject otherTile = null)
	{
		StopMoving();

		float originalX = transform.position.x;

		targetPosition = new Vector3(originalX + distance, transform.position.y, transform.position.z);
		destroyAfterMotion = !!otherTile;

		// starts coroutine
		StartCoroutine(MoveRightCoroutine());

		IEnumerator MoveRightCoroutine()
		{

			while (transform.position.x <= originalX + distance)
			{
				transform.position += new Vector3(SaveData.Settings.tileSpeed * 20f * Time.deltaTime, 0, 0);

				// waits till next frame
				yield return new WaitForEndOfFrame();
			}

			// perfects to intended position
			transform.position = new Vector3(originalX + distance, transform.position.y, transform.position.z);

			// crashes into other tile if it exists
			if (!!otherTile) CrashIntoTile(otherTile);
		}
	}

	public void CrashIntoTile(GameObject otherTile)
	{
		StopMoving();

		destroyAfterMotion = true;

		// updates value of other tile
		otherTile.GetComponent<Tile>().UpdateValue();

		// starts coroutine
		StartCoroutine(CrashIntoTileCoroutine());

		IEnumerator CrashIntoTileCoroutine()
		{
			// waits until at the same position as other tile
			while (transform.position != otherTile.transform.position)
			{
				transform.position = Vector3.MoveTowards(transform.position, otherTile.transform.position, SaveData.Settings.tileSpeed * 20f * Time.deltaTime);
				yield return null;
			}

			// destroys self
			Destroy(gameObject);
		}
	}

	public void StopMoving()
	{
		StopAllCoroutines();

		if (destroyAfterMotion) Destroy(gameObject);

		if (targetPosition != null) transform.position = (Vector3)targetPosition;

		targetPosition = null;
	}
}
