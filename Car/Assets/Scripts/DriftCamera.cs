using System;
using UnityEngine;

public class DriftCamera : MonoBehaviour
{
    [Serializable]
    public class AdvancedOptions
    {
        public bool updateCameraInUpdate;
        public bool updateCameraInFixedUpdate = true;
        public bool updateCameraInLateUpdate;
        public KeyCode switchViewKey = KeyCode.Space;
    }

    public float smoothing = 6f;
    public Transform lookAtTarget;
    public Transform positionTarget;
    public Transform sideView;
    public AdvancedOptions advancedOptions;
    bool m_ShowingSideView;

    private void UpdateCamera()
    {
        if(m_ShowingSideView)
        {
            transform.position = sideView.position; // 카메라 위치 이동
            transform.rotation = sideView.rotation; // 카메라 회전 이동
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, positionTarget.position, Time.deltaTime * smoothing); // 두 벡터 사이의 길이를 보간
            transform.LookAt(lookAtTarget); // 카메라가 타겟을 보도록 지정
        }
    }

    private void FixedUpdate() // 매 렌더링 프레임 사이에 여러 번 호출되는 함수
    {
        if(advancedOptions.updateCameraInFixedUpdate) // 만약 updateCameraInFixedUpdate가 true라면
            UpdateCamera(); // UpdateCamera 메서드 호출
    }

    private void Update() // 매 프레임마다 호출되는 함수
    {
        if(Input.GetKeyDown(advancedOptions.switchViewKey)) // 만약 switchViewKey를 눌렀다면
            m_ShowingSideView = !m_ShowingSideView; // ShowingSideView가 X

        if(advancedOptions.updateCameraInUpdate) // 만약 updateCameraInUpdate라면
            UpdateCamera(); // UpdateCamera 메서드 호출
    }

    private void LateUpdate() // 모든 Update 함수가 호출된 후 매 프레임마다 호출되는 함수
    {
        if(advancedOptions.updateCameraInLateUpdate) // 만약 updateCameraInLateUpdate라면
            UpdateCamera(); // UpdateCamera 메서드 호출
    }
}
