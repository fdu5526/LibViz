using UnityEngine;
using System.Collections;

public class ModelViewer: MonoBehaviour
{
	private KeyCode rotateRightKey = KeyCode.RightArrow;
	private KeyCode rotateLeftKey = KeyCode.LeftArrow;
	private KeyCode previousModelKey = KeyCode.UpArrow;
	private KeyCode nextModelKey = KeyCode.DownArrow;
	private KeyCode nextStateKey = KeyCode.Space;

	//private int frameOffset = 0;

	private int currentModelIndex = 0;

	public GameObject pedestalTop;

	BillBoardModel[] modelList;

	void Start ()
	{
		modelList = GetComponentsInChildren <BillBoardModel> ();
		foreach (BillBoardModel model in modelList) model.gameObject.SetActive(false);
		modelList[0].gameObject.SetActive(true);
	}

	void Update ()
	{
		if (modelList.Length == 0) {
		
			return;
		}

		BillBoardModel model = modelList [currentModelIndex];

		//float rotationThrottle = Time.deltaTime * rotateSpeed;
		float rotateAngle = 0.0f;
		if (Input.GetKey (rotateRightKey) && !Input.GetKey (rotateLeftKey)) {
			//rotationThrottle *= -1;
			rotateAngle = 0.5f;
			model.Rotate (rotateAngle);
		}
		if (!Input.GetKey (rotateRightKey) && Input.GetKey (rotateLeftKey)) {
			//rotationThrottle *= -1;
			rotateAngle = -0.5f;
			model.Rotate (rotateAngle);
		}

		if (!Input.GetKeyUp (nextModelKey) && Input.GetKeyUp (previousModelKey) && modelList.Length > 1) {

			int nextModelIndex = (currentModelIndex + modelList.Length - 1) % modelList.Length;
			modelList[nextModelIndex].gameObject.SetActive (true);
			model.gameObject.SetActive (false);
			
			currentModelIndex = nextModelIndex;
			model = modelList[currentModelIndex];
		}

		if (Input.GetKeyUp (nextModelKey) && !Input.GetKeyUp (previousModelKey) && modelList.Length > 1) {

			int nextModelIndex = (currentModelIndex + 1) % modelList.Length;
			modelList[nextModelIndex].gameObject.SetActive (true);
			model.gameObject.SetActive (false);

			currentModelIndex = nextModelIndex;
			model = modelList[currentModelIndex];
		}

		if (Input.GetKeyUp (nextStateKey) && 
		    //If the model is a SimpleBillboardModel, it must not be animateable to switch the state
		    (!(model is SimpleBillboardModel) || !(model as SimpleBillboardModel).autoAnimate)) {
			//rotationThrottle *= -1;
			//frameOffset = (modelList[currentModelIndex].leftQuad.frameOffset+1)%modelList[currentModelIndex].stateCount;
			model.billboardFrameIndex ++;
		}
	}
}
