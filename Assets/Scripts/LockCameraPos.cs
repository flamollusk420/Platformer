using UnityEngine;
using Cinemachine;
 
/// <summary>
/// An add-on module for Cinemachine Virtual Camera that locks the camera's X/Y co-ordinates
/// </summary>
[ExecuteInEditMode] [SaveDuringPlay] [AddComponentMenu("")] // Hide in menu
public class LockCameraPos : CinemachineExtension
{
    [Tooltip("Lock the camera's X position to this value")]
    public float m_XPosition = 0;
    [Tooltip("Lock the camera's Y position to this value")]
    public float m_YPosition = 0;
    [Tooltip("Lock camera X?")]
    public bool lockX;
    [Tooltip("Lock camera Y?")]
    public bool lockY;
 
    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
        {
            var pos = state.RawPosition;
            if(lockX) {
                pos.x = m_XPosition;
            }
            if(lockY) {
                pos.y = m_YPosition;
            }
            state.RawPosition = pos;
        }
    }
}
 