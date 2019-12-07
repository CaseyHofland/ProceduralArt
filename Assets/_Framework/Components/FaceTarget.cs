using UnityEngine;

[ExecuteAlways]
public class FaceTarget : MonoBehaviour
{
	public Transform target = null;
	[Tooltip("Specifies what orientation the object should have towards the target.")]
	public Vector3 orientation = Vector3.zero;

	private void OnRenderObject()
	{
		Reorient();
	}

	private void Reorient()
	{
		if (!target)
			return;

		transform.LookAt(target.position);
		transform.Rotate(orientation, Space.Self);
	}
}
