using UnityEngine;
using System;

[Serializable]
public enum DriveType // 자동차 구동 유형 열거
{
	RearWheelDrive, // 후륜 구동
	FrontWheelDrive, // 전륜 구동
	AllWheelDrive // 사륜 구동
}

public class WheelDrive : MonoBehaviour
{
    [Tooltip("Maximum steering angle of the wheels.")] // 스티어링 : 자동차가 그 진행방향을 바꾸기 위해 앞바퀴의 회전축 방향을 바꾸는 장치
    public float maxAngle = 30f; // 바퀴의 최대 스티어링 각도
	[Tooltip("Maximum torque applied to the driving wheels.")]
	public float maxTorque = 300f; // 움직이는 바퀴에 적용되는 최대 토크
	[Tooltip("Maximum brake torque applied to the driving wheels.")]
	public float brakeTorque = 30000f; // 움직이는 바퀴에 적용되는 최대 브레이크 토크
    [Tooltip("If you need the visual wheels to be attached automatically, drag the wheel shape here.")]
	public GameObject wheelShape; // 바퀴 모양 프리팹
    [Tooltip("The vehicle's speed when the physics engine can use different amount of sub-steps(in m/s).")]
	public float criticalSpeed = 5f; // 물리 엔진이 다른 양의 하위 단계를 사용할 수 있는 차량의 임계 속도(m/s)
	[Tooltip("Simulation sub-steps when the speed is above critical.")]
	public int stepsBelow = 5; // 임계 속도 초과일 때 하위 단계 시뮬레이션
	[Tooltip("Simulation sub-steps when the speed is below critical.")]
	public int stepsAbove = 1; // 임계 속도 미만일 때 하위 단계 시뮬레이션
    [Tooltip("The vehicle's drive type: rear-wheels drive, front-wheels drive or all-wheels drive.")]
    public DriveType driveType; // 차량의 구동 유형
    private WheelCollider[] m_Wheels; // 바퀴에 대한 참조

	void Start() // Update 함수가 호출되기 직전에 한 번만 호출되는 함수
    {
		m_Wheels = GetComponentsInChildren<WheelCollider>(); // 각 바퀴에 휠 콜라이더 추가

        for(int i = 0; i < m_Wheels.Length; ++i) // 바퀴의 수만큼 반복
		{
			var wheel = m_Wheels[i]; // 각 바퀴마다 번호 지정

			if(wheelShape != null) // 바퀴 모양의 프리팹이라면
			{
				var ws = Instantiate(wheelShape); // 바퀴의 모양 인스턴스화
				ws.transform.parent = wheel.transform; // 바퀴의 프리팹 지정
			}
		}
	}

	void Update() // 매 프레임마다 호출되는 함수
	{
		m_Wheels[0].ConfigureVehicleSubsteps(criticalSpeed, stepsBelow, stepsAbove); // 바퀴의 하위 단계 요소 설정
		float angle = maxAngle * Input.GetAxis("Horizontal"); // 앵글의 회전축 설정
		float torque = maxTorque * Input.GetAxis("Vertical"); // 토크의 회전축 설정
		float handBrake = Input.GetKey(KeyCode.X) ? brakeTorque : 0; // X 키를 눌렀을 때 브레이크 작동

		foreach(WheelCollider wheel in m_Wheels) // 각각의 바퀴 호출
		{
			if(wheel.transform.localPosition.z > 0) // 만약 바퀴의 z축 값이 0보다 크다면
				wheel.steerAngle = angle; // steerAngle = angle

			if(wheel.transform.localPosition.z < 0) // 만약 바퀴의 z축 값이 0보다 작다면
				wheel.brakeTorque = handBrake; // brakeTorque = handBrake

			if(wheel.transform.localPosition.z < 0 && driveType != DriveType.FrontWheelDrive) // 만약 바퀴의 z축 값이 0보다 작고 전륜 구동 타입이라면
				wheel.motorTorque = torque; // motorTorque = torque

			if(wheel.transform.localPosition.z >= 0 && driveType != DriveType.RearWheelDrive) // 만약 바퀴의 z축 값이 0보다 크거나 같고 후륜 구동 타입이라면
				wheel.motorTorque = torque; // motorTorque = torque;

			if(wheelShape)
			{
				Quaternion q; // 바퀴의 회전에 대한 참조
                Vector3 p; // 바퀴의 위치에 대한 참조
                wheel.GetWorldPose(out p, out q); // 바퀴의 회전과 위치에 대한 참조 적용
				Transform shapeTransform = wheel.transform.GetChild(0); // 바퀴의 위치 지정
				shapeTransform.position = p; // 바퀴 위치값 지정
				shapeTransform.rotation = q; // 바퀴 회전값 지정
			}
		}
	}
}
