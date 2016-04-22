#pragma strict


public var innerSphere1 : GameObject;
public var innerSphere2 : GameObject;
public var outerSphere1 : GameObject;
public var outerSphere2 : GameObject;

private var fadeFlag : boolean;


function Start () {

}


function Update () {
	if (innerSphere1.GetComponent.<Renderer>().material.color.a > 0 && fadeFlag) {
		innerSphere1.GetComponent.<Renderer>().material.color.a = innerSphere1.GetComponent.<Renderer>().material.color.a - Time.deltaTime * 0.5;
		innerSphere2.GetComponent.<Renderer>().material.color.a = innerSphere2.GetComponent.<Renderer>().material.color.a - Time.deltaTime * 0.5;
		print(innerSphere1.GetComponent.<Renderer>().material.color.a);
	} else {
		fadeFlag = false;
	}	
}

function doTouchDown() {
	print("Down");
}
	
function doTouchUp() {
	print("Up");
	fadeFlag = true;
}
	
function CrossFade (newTexture1 : Texture, newTexture2 : Texture) {
	// set the texture to fade from to the inner sphere and make the inner sphere 100%
/*	innerSphere1.renderer.material.mainTexture = outerSphere1.renderer.material.mainTexture;
	innerSphere2.renderer.material.mainTexture = outerSphere2.renderer.material.mainTexture;
	innerSphere1.renderer.material.color.a = 100;
	innerSphere2.renderer.material.color.a = 100;
	// set the texture to fade to to the outer sphere
	outerSphere1.renderer.material.mainTexture = newTexture1;
	outerSphere2.renderer.material.mainTexture = newTexture2;
	outerSphere1.renderer.material.color.a = 100;
	outerSphere2.renderer.material.color.a = 100;
*/	
	fadeFlag = true;
}