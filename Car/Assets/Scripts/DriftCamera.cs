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
            transform.position = sideView.position; // ī�޶� ��ġ �̵�
            transform.rotation = sideView.rotation; // ī�޶� ȸ�� �̵�
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, positionTarget.position, Time.deltaTime * smoothing); // �� ���� ������ ���̸� ����
            transform.LookAt(lookAtTarget); // ī�޶� Ÿ���� ������ ����
        }
    }

    private void FixedUpdate() // �� ������ ������ ���̿� ���� �� ȣ��Ǵ� �Լ�
    {
        if(advancedOptions.updateCameraInFixedUpdate) // ���� updateCameraInFixedUpdate�� true���
            UpdateCamera(); // UpdateCamera �޼��� ȣ��
    }

    private void Update() // �� �����Ӹ��� ȣ��Ǵ� �Լ�
    {
        if(Input.GetKeyDown(advancedOptions.switchViewKey)) // ���� switchViewKey�� �����ٸ�
            m_ShowingSideView = !m_ShowingSideView; // ShowingSideView�� X

        if(advancedOptions.updateCameraInUpdate) // ���� updateCameraInUpdate���
            UpdateCamera(); // UpdateCamera �޼��� ȣ��
    }

    private void LateUpdate() // ��� Update �Լ��� ȣ��� �� �� �����Ӹ��� ȣ��Ǵ� �Լ�
    {
        if(advancedOptions.updateCameraInLateUpdate) // ���� updateCameraInLateUpdate���
            UpdateCamera(); // UpdateCamera �޼��� ȣ��
    }
}
