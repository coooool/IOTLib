using System.Transactions;
using UnityEngine;

namespace IOTLib
{
	public class AroundCamera
	{
		/// <summary>
		/// Around center.
		/// </summary>
		[Tooltip("Around center.")]
		public Transform target = null;

		/// <summary>
		/// Range limit of angle.
		/// </summary>
		[Tooltip("Range limit of angle.")]
		public Vector2 angleRange = new Vector2(15f, 90f);

		/// <summary>
		/// Range limit of distance.
		/// </summary>
		//[Tooltip("Range limit of distance.")]
		//public Vector2 distanceRange = new Vector2(1f, 10f);

		/// <summary>
		/// Camera target angls.
		/// </summary>
		protected Vector2 TargetAngles;

		/// <summary>
		/// Target distance from camera to target.
		/// </summary>
		protected float TargetDistance;

		/// <summary>
		/// Camera current angls.
		/// </summary>
		public Vector2 CurrentAngles { get; protected set; }

		/// <summary>
		/// Current distance from camera to target.
		/// </summary>
		public float CurrentDistance { get; protected set; }

		private Camera m_Camera;
		
		internal GameObject __center__;

        #region 平移
        protected Vector3 Translate_TargetOffset;
		public Vector3 Translate_CurrentOffset;
		bool CurIsTranslate = false;
		bool CurIsRotate = false;

		float m_Boost = 1.0f;
		float m_WhellBoost = 1.0f;
        #endregion

        internal void Init()
		{
			m_Camera = Camera.main;

			if(target == null)
			{
				__center__ = new GameObject("__Center__");
				__center__.transform.position = CameraControlSetting.Setting.MapControlInitWorldCenter;

				target = __center__.transform;
			}

			CurIsRotate = false;
			CurIsTranslate = false;

            CurrentAngles = TargetAngles = m_Camera.transform.eulerAngles;
            CurrentDistance = TargetDistance = Vector3.Distance(m_Camera.transform.position, target.position);

            Translate_CurrentOffset = Translate_TargetOffset = m_Camera.transform.position;
        }

		// 平移
		void UpdateTranslate()
		{
            if (Input.GetMouseButton(0))
            {
                float num = Input.GetAxis("Mouse X") * CameraControlSetting.Setting.mouseTranslationSensitivity * m_Boost;
                float num2 = Input.GetAxis("Mouse Y") * CameraControlSetting.Setting.mouseTranslationSensitivity * m_Boost;
                Translate_TargetOffset -= m_Camera.transform.right * num;
                Translate_TargetOffset -= Vector3.Cross(m_Camera.transform.right, Vector3.up) * num2;

                //targetOffset.x = Mathf.Clamp(targetOffset.x, 0f - CameraControlSetting.Setting.li.width, areaSettings.width);
                //targetOffset.z = Mathf.Clamp(targetOffset.z, 0f - areaSettings.length, areaSettings.length);
            }
			
            Translate_CurrentOffset  = Vector3.Lerp(Translate_CurrentOffset, Translate_TargetOffset, CameraControlSetting.Setting.boost * Time.deltaTime);  
			m_Camera.transform.position = Translate_CurrentOffset;
        }

        internal void Update()
		{
            var PointerOverGameObject = CameraHandle.IsPointerOverGameObject || CameraHandle.IsPointerHoverGameObject || CGHandleDragMouse.MouseInDragWindow;

            if (Input.GetMouseButtonDown(0))
			{
                if (PointerOverGameObject)
                    return;

				if (!CurIsTranslate)
				{
					CurIsTranslate = true;
					CurIsRotate = false;

					Translate_TargetOffset = Translate_CurrentOffset = m_Camera.transform.position;
				}
            }
			else if(Input.GetMouseButtonDown (1) || Input.mouseScrollDelta.y != 0)
			{
                if (PointerOverGameObject)
                    return;

				if (!CurIsRotate)
				{
					CurIsTranslate = false;
					CurIsRotate = true;

					ClearARoundCameraData();
				}
            }

            // 使用动态因子
            if (CameraControlSetting.Setting.UseDynamicBoost)
            {
                m_Boost = CameraControlSetting.Setting.DynamicBoostCurve.Evaluate(Vector3.Distance(m_Camera.transform.position, target.position));
				m_WhellBoost = Vector3.Distance(m_Camera.transform.position, target.position) / CameraControlSetting.Setting.DBoostScale; //CameraControlSetting.Setting.MouseWhellCurve.Evaluate(Vector3.Distance(m_Camera.transform.position, target.position));
				m_WhellBoost = CameraControlSetting.Setting.mouseWheelSensitivity * m_WhellBoost;

            }

            if (CurIsTranslate)
			{
                if (CameraControlSetting.Setting.HasCameraControlMethod(ModelControlTypeEnum.Translate))
                {
                    UpdateTranslate();
                }
            }
            else if (CurIsRotate)
            {
                if (CameraControlSetting.Setting.HasCameraControlMethod(ModelControlTypeEnum.Rotate))
                {
                    UpdateAround();
                }
            }  
        }

		void ClearARoundCameraData()
		{
            m_Camera.GetLookAtGroundPoint(out var AroundCenter);
            target.transform.position = AroundCenter;

            CurrentAngles = TargetAngles = m_Camera.transform.eulerAngles;
            CurrentDistance = TargetDistance = Vector3.Distance(m_Camera.transform.position, target.position);
        }

        protected void UpdateAround()
		{
            if (Input.GetMouseButton(1))
			{
                TargetAngles.y += Input.GetAxis("Mouse X") * CameraControlSetting.Setting.mousePointSensitivity;
                TargetAngles.x -= Input.GetAxis("Mouse Y") * CameraControlSetting.Setting.mousePointSensitivity;
                TargetAngles.x = Mathf.Clamp(TargetAngles.x, angleRange.x, angleRange.y);
            }

            TargetDistance -= Input.GetAxis("Mouse ScrollWheel") * CameraControlSetting.Setting.mouseWheelSensitivity * m_WhellBoost;
            
			if(CameraControlSetting.Setting.LimitMapArea)
				TargetDistance = Mathf.Clamp(TargetDistance, CameraControlSetting.Setting.LimitHeight.x, CameraControlSetting.Setting.LimitHeight.y);

            CurrentAngles = Vector2.Lerp(CurrentAngles, TargetAngles, CameraControlSetting.Setting.boost * Time.deltaTime);
            CurrentDistance = Mathf.Lerp(CurrentDistance, TargetDistance, CameraControlSetting.Setting.boost * Time.deltaTime);

            m_Camera.transform.rotation = Quaternion.Euler(CurrentAngles);
            m_Camera.transform.position = target.position - m_Camera.transform.forward * CurrentDistance;
		}

        internal void SetCustomPos(Vector3 pos)
		{
			CurIsTranslate = false;
			CurrentDistance = TargetDistance;
            m_Camera.transform.position = pos;
		}

		internal void SetCustomEulerAngles(Vector3 angles)
		{
			CurIsRotate = false;
			CurrentAngles = TargetAngles = angles;
            m_Camera.transform.eulerAngles = angles;
		}
	}
}