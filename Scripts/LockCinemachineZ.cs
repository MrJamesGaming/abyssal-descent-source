using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class LockCinemachineX : CinemachineExtension
{
    [Tooltip("Lock the camera's Y position to this value")]
    [SerializeField]public float m_YPosition = -10;
 
    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
        {
            var pos = state.RawPosition;
            pos.z = m_YPosition;
            state.RawPosition = pos;
        }
    }
}
