using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

namespace IOTLib
{
    internal class CameraState
    {
        public float yaw;
        public float pitch;
        public float roll;
        public float x;
        public float y;
        public float z;

        public void SetFromTransform(Transform t)
        {
            pitch = t.eulerAngles.x > 180 ? t.eulerAngles.x -360.0f : t.eulerAngles.x;
            yaw = t.eulerAngles.y;
            roll = t.eulerAngles.z;

            x = t.position.x;
            y = t.position.y;
            z = t.position.z;
        }

        public void SetFromTransform(CameraState t)
        {
            pitch = t.pitch;
            yaw = t.yaw;
            roll = t.roll;
            x = t.x;
            y = t.y;
            z = t.z;
        }

        public void SetPos(Vector3 pos)
        {
            x = pos.x;
            y = pos.y;
            z = pos.z;
        }

        public void SetAngles(Vector3 eulerAngles)
        {
            pitch = eulerAngles.x;
            yaw = eulerAngles.y;
            roll = eulerAngles.z;
        }

        public void Translate(Vector3 translation)
        {
            Vector3 rotatedTranslation = Quaternion.Euler(pitch, yaw, roll) * translation;

            x += rotatedTranslation.x;
            y += rotatedTranslation.y;
            z += rotatedTranslation.z;

            // ÏÞÖÆY
            //y = Mathf.Max(y, 4.0f, y);
        }

        public Vector3 XYZ()
        {
            return new Vector3(x, y, z);
        }

        public Vector3 PYR(Vector3 translation) 
        {
            return Quaternion.Euler(pitch, yaw, roll) * translation;
        }

        public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct)
        {
            yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
            pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
            roll = Mathf.Lerp(roll, target.roll, rotationLerpPct);

            x = Mathf.Lerp(x, target.x, positionLerpPct);
            y = Mathf.Lerp(y, target.y, positionLerpPct);
            z = Mathf.Lerp(z, target.z, positionLerpPct);
        }

        public void UpdateTransform(Transform t)
        {
            t.eulerAngles = new Vector3(pitch, yaw, roll);
            t.position = new Vector3(x, y, z);
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5}", x,y,z, yaw, pitch, roll);
        }
    }
}