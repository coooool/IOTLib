
using IOTLib;
using UnityEngine;

/// <summary>
/// 代表像机Map操作模式下的场景中心
/// </summary>
internal class SceneCenterMouseTranslateAgent : MonoBehaviour
{
	/// <summary>
	/// Damper for move.
	/// </summary>
	[Tooltip("Damper for move.")]
	[Range(0f, 10f)]
	public float damper = 5f;

	/// <summary>
	/// Target offset base area center.
	/// </summary>
	protected Vector3 targetOffset;

	/// <summary>
	/// Current offset base area center.
	/// </summary>
	public Vector3 CurrentOffset { get; protected set; }

	public AroundCamera AroundCameraControl { get; internal set; }

	/// <summary>
	/// Component awake.
	/// </summary>
	protected virtual void Awake()
	{
		//CurrentOffset = targetOffset = transform.position - areaSettings.center.position;

		// 创建物体时，自己就是中心
		CurrentOffset = targetOffset = transform.position;
    }

	/// <summary>
	/// Component update.
	/// </summary>
	protected virtual void Update()
	{
        if (Input.GetMouseButton(0))
        {
            float num = Input.GetAxis("Mouse X") * CameraControlSetting.Setting.mousePointSensitivity;
            float num2 = Input.GetAxis("Mouse Y") * CameraControlSetting.Setting.mousePointSensitivity;
            targetOffset -= Camera.main.transform.right * num;
            targetOffset -= Vector3.Cross(Camera.main.transform.right, Vector3.up) * num2;

            //targetOffset.x = Mathf.Clamp(targetOffset.x, 0f - CameraControlSetting.Setting.li.width, areaSettings.width);
            //targetOffset.z = Mathf.Clamp(targetOffset.z, 0f - areaSettings.length, areaSettings.length);
        }

        CurrentOffset = Vector3.Lerp(CurrentOffset, targetOffset, damper * Time.deltaTime);
        Camera.main.transform.position = CurrentOffset;
    }
}
