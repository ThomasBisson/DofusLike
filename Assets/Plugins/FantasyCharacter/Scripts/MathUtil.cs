using UnityEngine;
using System.Collections;

public class MathUtil
{
	public static Vector3 calcTargetPosByRotation(Transform target, float angle, float Distance, bool isVer = false)
	{
		Quaternion r = Quaternion.Euler (0, angle, 0) * target.rotation;
		if(isVer)
		{
			r = Quaternion.Euler (0, 0, angle) * target.rotation;
		}
		Vector3 dis = r * Vector3.forward * Distance;
		Vector3 newPos = dis + target.position;
		return newPos;
	}

	public static Vector3 calcTargetPosByRotation(Vector3 pos, Quaternion rotation, float angle, float Distance)
	{
		Quaternion r = Quaternion.Euler (0, angle, 0) * rotation;
		Vector3 dis = r * Vector3.forward * Distance;
		Vector3 newPos = dis + pos;
		return newPos;
	}

    public static Vector3 calcTargetPosByRotation1(Vector3 pos, Quaternion rotation, float angle, float Distance)
    {
        Quaternion r = Quaternion.Euler(angle, 0, 0) * rotation;
        Vector3 dis = r * Vector3.forward * Distance;
        Vector3 newPos = dis + pos;
        return newPos;
    }

    public static Vector3 calcTargetPosByOffset(Transform target, Vector3 off)
	{
		Quaternion r = target.rotation;
		return calcTargetPosByOffset (r, target.position, off.x, off.y, off.z);
	}

	public static Vector3 calcTargetPosByOffset(Transform target, float x, float y, float z)
	{
		Quaternion r = target.rotation;
		return calcTargetPosByOffset (r, target.position, x, y, z);
	}

	public static Vector3 calcTargetPosByDis(Vector3 pos1, Vector3 pos2, float dis)
	{
		Vector3 sub = pos2 - pos1;
		Quaternion r = Quaternion.LookRotation (sub);
		Vector3 newPos = pos1 + (r * Vector3.forward) * dis;
		return newPos;
	}

	public static Vector3 calcTargetPosByDis1(Vector3 pos1, Vector3 pos2, float dis)
	{
		Vector3 sub = pos2 - pos1;
		Quaternion r = Quaternion.LookRotation (sub);
		Vector3 newPos = pos2 + (r * Vector3.forward) * dis;
		return newPos;
	}

	public static Vector3 calcTargetPosByOffset(Quaternion r, Vector3 pos, float x, float y, float z)
	{
		Vector3 newPos;
		newPos = pos + (r * Vector3.right) * z;		
		newPos = newPos + (r * Vector3.forward) * x;		
		newPos = newPos + (r * Vector3.up) * y;
		return newPos;
	}

	public static Vector3 calcTargetPosByOffset (Vector3 vector, Vector3 pos, float x, float y, float z)
	{
		Quaternion r = Quaternion.LookRotation (vector);

		return calcTargetPosByOffset (r, pos, x, y, z);
	}

	public static bool checkPointIsInRect(Vector3 center, Quaternion myRotation, Vector3 targetPos, float haflen, float hafwidth)
	{
		Vector2 point3 = new Vector2 (targetPos.x, targetPos.z);
		Quaternion rectW = new Quaternion (0, myRotation.y, 0, myRotation.w);
		Vector2 point1 = new Vector2 (center.x, center.z);
		Vector3 pointW2Diff = rectW * Vector3.forward;
		Vector2 pointW2 = point1 + new Vector2(pointW2Diff.x, pointW2Diff.z);
		if(getVerticalDisSqrt(point1, pointW2, point3) > hafwidth * hafwidth)
		{
			return false;
		}
		Vector3 pointL2Diff = rectW * Vector3.right;
		Vector2 pointL2 = point1 + new Vector2(pointL2Diff.x, pointL2Diff.z);
		if(getVerticalDisSqrt(point1, pointL2, point3) > haflen * haflen)
		{
			return false;
		}
		return true;
	}

	public static Vector2 getVerticalPoint(Vector2 point1, Vector2 point2, Vector2 point3)
	{
		float k1 = (point2.y - point1.y) / (point2.x - point1.x);
		float b1 = point1.y - k1 * point1.x;
		float k2 = -1f / k1;
		float b2 = point3.y - k2 * point3.x;
		
		float x = (b2 - b1) / (k1 - k2);
		float y = (b1 * k2 - b2 * k1) / (k2 - k1);
		return new Vector2 (x, y);
	}

	public static float getVerticalDisSqrt(Vector2 point1, Vector2 point2, Vector2 point3)
	{
		float k1 = (point2.y - point1.y) / (point2.x - point1.x);
		float b1 = point1.y - k1 * point1.x;
		float k2 = -1f / k1;
		float b2 = point3.y - k2 * point3.x;
		
		float x = (b2 - b1) / (k1 - k2);
		float y = (b1 * k2 - b2 * k1) / (k2 - k1);
		return (point3.x - x) * (point3.x - x) + (point3.y - y) * (point3.y - y);
	}

	public static Vector2 getVerticalPos(Vector2 point1, Vector2 point2, Vector2 point3)
	{
		float k1 = (point2.y - point1.y) / (point2.x - point1.x);
		float b1 = point1.y - k1 * point1.x;
		float k2 = -1f / k1;
		float b2 = point3.y - k2 * point3.x;

		float x = (b2 - b1) / (k1 - k2);
		float y = (b1 * k2 - b2 * k1) / (k2 - k1);
		return new Vector2(x, y);
	}

	public static bool checkIsInPointRange(Vector3 point1, Vector3 point2, Vector3 point3)
	{
		Vector2 vP = getVerticalPos(new Vector2(point1.x, point1.z), new Vector2(point2.x, point2.z), new Vector2(point3.x, point3.y));
		float maxX = Mathf.Max (point1.x, point2.x);
		float minX = Mathf.Min (point1.x, point2.x);
		float maxZ = Mathf.Max (point1.y, point2.y);
		float minZ = Mathf.Min (point1.y, point2.y);
		if (vP.x <= maxX && vP.x >= minX && vP.y <= maxZ && vP.y >= minZ) 
		{
			return true;		
		}
		return false;
	}

	public static Quaternion getSlerpQuaternion(Vector3 vec1, Vector3 vec2, float speed)
	{
		//Quaternion r = Quaternion.FromToRotation (vec1, vec2);

		Quaternion r1 = Quaternion.LookRotation (vec1);
		Quaternion r2 = Quaternion.LookRotation (vec2);
		return getSlerpQuaternion (r1, r2, speed);
	}

	public static Quaternion getSlerpQuaternion(Quaternion r1, Quaternion r2, float speed)
	{
		float angle = Quaternion.Angle (r1, r2);
		float t = 1f;
		t = speed / angle * Time.deltaTime;
		if(t > 1f)
		{
			t = 1f;
		}
		return Quaternion.Slerp (r1, r2, t);
	}

	public static string getVector3Str(Vector3 pos)
	{
		return "(" + pos.x + "," + pos.y + "," + pos.z + ")";
	}

	public static float getXZSqr(Vector3 pos1, Vector3 pos2)
	{
		pos1.y = 0;
		pos2.y = 0;
		return (pos1 - pos2).sqrMagnitude;
	}

	public static float PointToSegmentDistance (Vector3 point, Vector3 ep0, Vector3 ep1)
	{
		// convert the test point to be "local" to ep0
		Vector3 local = point - ep0;
		Vector3 segmentNormal = ep1 - ep0;
		// find the projection of "local" onto "segmentNormal"
		float segmentProjection = Vector3.Dot(segmentNormal, local);
		float segmentLength = segmentNormal.magnitude;
		Vector3 chosen;
		// handle boundary cases: when projection is not on segment, the
		// nearest point is one of the endpoints of the segment
		if (segmentProjection < 0)
		{
			chosen = ep0;
			segmentProjection = 0;
			return (point- ep0).magnitude;
		}
		if (segmentProjection > segmentLength)
		{
			chosen = ep1;
			segmentProjection = segmentLength;
			return (point-ep1).magnitude;
		}
		
		// otherwise nearest point is projection point on segment
		chosen = segmentNormal * segmentProjection;
		chosen +=  ep0;
		return (point-chosen).magnitude;
	}

    public static Transform findChild(Transform tr, string name)
    {
        Transform result = null;
        result = tr.Find(name);
        if(result == null)
        {
            foreach(Transform child in tr)
            {
                result = findChild(child, name);
                if(result != null)
                {
                    return result;
                }
            }
        }
        return result;
    }
}
