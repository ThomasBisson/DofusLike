var scrollSpeed:float ;
//var uvtexture:Texture;//��2d�D
function Update () 
{
var offset = Time.time * scrollSpeed;
GetComponent.<Renderer>().material.SetTextureOffset ("_MainTex", Vector2(0,-offset));
}