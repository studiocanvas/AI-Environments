using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Selector : MonoBehaviour {



	public GameObject selectedunit;
	public List<GameObject> selectedunits = new List<GameObject>();
	RaycastHit hit;
	public Transform target;
	private Vector3 MouseDownPoint, CurrentDownPoint;
	public bool IsDragging;
	private float BoxWidth, BoxHeight, BoxLeft, BoxTop;
	private Vector2 BoxStart, BoxFinish;
	public List<GameObject> UnitsOnScreenSpace = new List<GameObject>();
	public List<GameObject> UnitInDrag = new List<GameObject>();

	void OnGUI()
	{
		if(IsDragging)
		{
			GUI.Box(new Rect(BoxLeft, BoxTop, BoxWidth, BoxHeight), "");
		}
	}
	void LateUpdate()
	{
		UnitInDrag.Clear();
		if(IsDragging && UnitsOnScreenSpace.Count > 0)
		{
			selectedunit = null;
			for (int i = 0; i < UnitsOnScreenSpace.Count; i++)
			{
				GameObject UnitObj = UnitsOnScreenSpace[i] as GameObject;
				SelectChar PosScript = UnitObj.transform.GetComponent<SelectChar>();
				GameObject selectmarker = UnitObj.transform.Find("Marker").gameObject;
				if(PosScript && !UnitInDrag.Contains(UnitObj))
				{
					if (UnitWithinDrag(PosScript.ScreenPos))
					{
						selectmarker.SetActive(true);
						UnitInDrag.Add(UnitObj);
					}
					else
					{
						if (!UnitInDrag.Contains(UnitObj))
							selectmarker.SetActive(false);
					}
				}
			}
		}
	}

	void Update () {
		if (Input.GetMouseButton(0))
		{
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
			{
				if(hit.transform.tag != "SelectableUnit" && hit.transform.tag != "Building")
				{
					if(CheckIfMouseIsDragging())
					{
						IsDragging = true;
					}
				}
			}
		}
		if(Input.GetMouseButtonUp(0))
		{
			PutUnitsFromDragIntoSelectedUnits();
			IsDragging = false;
		}
		if (selectedunit == null)
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
				{
					if (hit.transform.tag == "SelectableUnit" || hit.transform.tag == "Building")
					{
						selectedunit = hit.transform.gameObject;
						selectedunit.transform.Find("Marker").gameObject.SetActive(true);
						for (int i = 0; i < selectedunits.Count; i++)
						{
							selectedunits[i].transform.Find("Marker").gameObject.SetActive(false);
						}
						selectedunits.Clear();
					}
					if(hit.transform.tag == "Floor")
					{
						for (int i = 0; i < selectedunits.Count; i++)
						{
							selectedunits[i].transform.Find("Marker").gameObject.SetActive(false);
						}
						selectedunits.Clear();
					}
				}
			}
		}
		else
		{
			if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
			{
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
				{
					if (hit.transform.tag == "SelectableUnit" || hit.transform.tag == "Building")
					{
						selectedunit.transform.Find("Marker").gameObject.SetActive(false);
						selectedunit = null;
						selectedunit = hit.transform.gameObject;
						selectedunit.transform.Find("Marker").gameObject.SetActive(true);

					}
					if (hit.transform.tag == "Floor")
					{
						selectedunit.transform.Find("Marker").gameObject.SetActive(false);
						selectedunit = null;
					}
				}
			}
		}
		if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
		{
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
			{
				if (hit.transform.tag == "SelectableUnit" || hit.transform.tag == "Building")
				{
					if(selectedunit != null)
					{
						selectedunits.Add(selectedunit);
						selectedunit = null;
					}
					selectedunits.Add(hit.transform.gameObject);
					for (int i = 0; i < selectedunits.Count; i++)
					{
						selectedunits[i].transform.Find("Marker").gameObject.SetActive(true);
					}

				}
			}
		}
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
			CurrentDownPoint = hit.point;
		if(Input.GetMouseButtonDown(0))
		{
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
				MouseDownPoint = hit.point;
		}
		if(IsDragging)
		{
			BoxWidth = Camera.main.WorldToScreenPoint(MouseDownPoint).x - Camera.main.WorldToScreenPoint(CurrentDownPoint).x;
			BoxHeight = Camera.main.WorldToScreenPoint(MouseDownPoint).y - Camera.main.WorldToScreenPoint(CurrentDownPoint).y;
			BoxLeft = Input.mousePosition.x;
			BoxTop = (Screen.height - Input.mousePosition.y) - BoxHeight;

			if(BoxWidth > 0f && BoxHeight < 0f)
			{
				BoxStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			}
			else if (BoxWidth > 0f && BoxHeight > 0f)
			{
				BoxStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y + BoxHeight);
			}
			else if (BoxWidth < 0f && BoxHeight < 0f)
			{
				BoxStart = new Vector2(Input.mousePosition.x + BoxWidth, Input.mousePosition.y);
			}
			else if (BoxWidth < 0f && BoxHeight > 0f)
			{
				BoxStart = new Vector2(Input.mousePosition.x + BoxWidth, Input.mousePosition.y + BoxHeight);
			}
			BoxFinish = new Vector2(BoxStart.x + Unsigned(BoxWidth), BoxStart.y - Unsigned(BoxHeight));
		}
		if (Input.GetMouseButtonDown(1))
		{
			if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
			{
				target = hit.transform;
			}
		}
	}
	float Unsigned(float val)
	{
		if (val < 0f)
			val *= -1;
		return val;
	}
	private bool CheckIfMouseIsDragging()
	{
		if (CurrentDownPoint.x - 2 >= MouseDownPoint.x || CurrentDownPoint.y - 2 >= MouseDownPoint.y || CurrentDownPoint.z - 2 >= MouseDownPoint.z ||
			CurrentDownPoint.x < MouseDownPoint.x - 2 || CurrentDownPoint.y < MouseDownPoint.y - 2 || CurrentDownPoint.z < MouseDownPoint.z - 2)
			return true;
		else
			return false;
	}
	public bool UnitWithinScreenSpace(Vector2 UnitScreenPos)
	{
		if ((UnitScreenPos.x < Screen.width && UnitScreenPos.y < Screen.height) && (UnitScreenPos.x > 0f && UnitScreenPos.y > 0f))
			return true;
		else
			return false;
	}
	public bool UnitWithinDrag(Vector2 UnitScreenPos)
	{
		if ((UnitScreenPos.x > BoxStart.x && UnitScreenPos.y < BoxStart.y) && (UnitScreenPos.x < BoxFinish.x && UnitScreenPos.y > BoxFinish.y))
			return true;
		else
			return false;
	}
	public void PutUnitsFromDragIntoSelectedUnits()
	{
		if(UnitInDrag.Count > 0)
		{
			for (int i = 0; i < UnitInDrag.Count; i++)
			{
				if (!selectedunits.Contains(UnitInDrag[i]))
					selectedunits.Add(UnitInDrag[i]);
			}
		}
		UnitInDrag.Clear();
	}
}