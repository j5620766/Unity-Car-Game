using UnityEngine;

[ExecuteInEditMode]
public class EasySuspension : MonoBehaviour
{
    [Range(0.1f, 20f)] // 변수의 값을 0.1에서 20까지로 제한
	[Tooltip("Natural frequency of the suspension springs. Describes bounciness of the suspension.")] // 서스펜션 : 자동차의 구조장치로서 노면의 충격이 차체나 탑승자에게 전달되지 않게 충격을 흡수하는 장치
    public float naturalFrequency = 10; // 서스펜션의 탄력성
    [Range(0f, 3f)] // 변수의 값을 0에서 3까지로 제한
	[Tooltip("Damping ratio of the suspension springs. Describes how fast the spring returns back after a bounce.")]
	public float dampingRatio = 0.8f; // 서스펜션의 스프링이 튕긴 후 되돌아 오는 속도
    [Range(-1f, 1f)] // 변수의 값을 -1에서 1까지로 제한
	[Tooltip("The distance along the Y axis the suspension forces application point is offset below the center of mass.")]
	public float forceShift = 0.03f; // 질량 중심의 아래로 상쇄되는 서스펜션 힘 작용 지점의 y축 거리
    [Tooltip("Adjust the length of the suspension springs according to the natural frequency and damping ratio. When off, can cause unrealistic suspension bounce.")]
	public bool setSuspensionDistance = true; // 서스펜션 스프링의 고유 진동수 및 되돌아 오는 속도에 따라 스프링의 길이를 조정하는 옵션
    Rigidbody m_Rigidbody; // Rigidbody에 대한 참조

    void Start() // Update 함수가 호출되기 직전에 한 번만 호출되는 함수
    {
        m_Rigidbody = GetComponent<Rigidbody>(); // Rigidbody 구성 요소 적용
    }
    
	void Update() // 매 프레임마다 호출되는 함수
    {
		foreach(WheelCollider wc in GetComponentsInChildren<WheelCollider>()) // 휠 콜라이더 호출
        {
			JointSpring spring = wc.suspensionSpring;
            float sqrtWcSprungMass = Mathf.Sqrt(wc.sprungMass); // sqrtWcSprungMass = sprungMass의 제곱근
            spring.spring = sqrtWcSprungMass * naturalFrequency * sqrtWcSprungMass * naturalFrequency; // 스프링의 힘
            spring.damper = 2f * dampingRatio * Mathf.Sqrt(spring.spring * wc.sprungMass); // 스프링에서 진동 에너지를 흡수하는 장치의 힘
			wc.suspensionSpring = spring;
			Vector3 wheelRelativeBody = transform.InverseTransformPoint(wc.transform.position); // 위치 지정
            float distance = m_Rigidbody.centerOfMass.y - wheelRelativeBody.y + wc.radius; // distance 변수값 설정
			wc.forceAppPointDistance = distance - forceShift; // 정지된 바퀴 휠의 적용 지점에서 측정된 서스펜션 및 타이어 힘의 적용 점

            if(spring.targetPosition > 0 && setSuspensionDistance) // joint가 도달하려는 궤도 위치가 0보다 크고 setSuspensionDistance 옵션이 true라면
				wc.suspensionDistance = wc.sprungMass * Physics.gravity.magnitude / (spring.targetPosition * spring.spring); // 서스펜션의 최대 연장 거리 설정
		}
	}
}
