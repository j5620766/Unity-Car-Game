using UnityEngine;
using System.Collections.Generic;

public class CarSwitcher : MonoBehaviour
{
	public List<GameObject> vehicles;
	public Transform spawnPoints;
	private DriftCamera m_DriftCamera;
	private int m_VehicleId;

	void Start() // Update 함수가 호출되기 직전에 한 번만 호출되는 함수
    {
		m_DriftCamera = GetComponent<DriftCamera>(); // DriftCamera 구성 요소 적용
	}
	
	void Update() // 매 프레임마다 호출되는 함수
    {
		if(Input.GetKeyUp(KeyCode.K)) // 만약 K키를 누르고 떼면
		{
            vehicles[m_VehicleId].SetActive(false); //차량 활동 비활성화
			m_VehicleId = (m_VehicleId + 1) % vehicles.Count; //차량 교체
			vehicles[m_VehicleId].SetActive(true); // 차량 활동 활성화
			Transform vehicleT = vehicles[m_VehicleId].transform; // 차량 위치 지정
			Transform camRig = vehicleT.Find("CamRig"); // 차량 카메라 지정
			m_DriftCamera.lookAtTarget = camRig.Find("CamLookAtTarget"); // DriftCamera 스크립트의 lookAtTarget 호출
			m_DriftCamera.positionTarget = camRig.Find("CamPosition"); // DriftCamera 스크립트의 positionTarget 호출
            m_DriftCamera.sideView = camRig.Find("CamSidePosition"); // DriftCamera 스크립트의 sideView 호출
        }

		if(Input.GetKeyUp(KeyCode.R)) // 만약 R키를 누르고 떼면
		{
			Transform vehicleTransform = vehicles[m_VehicleId].transform; // 차량 이동 위치 참조
			vehicleTransform.rotation = Quaternion.identity; // 차량 회전 참조
			Transform closest = spawnPoints.GetChild(0); // 차량 스폰 지점 참조

			for(int i = 0; i < spawnPoints.childCount; ++i) // 스폰 지점의 수만큼 반복
			{
				Transform thisTransform = spawnPoints.GetChild(i); // 스폰 지점 지정
				float distanceToClosest = Vector3.Distance(closest.position, vehicleTransform.position); // distanceToClosest 위치 변수
				float distanceToThis = Vector3.Distance(thisTransform.position, vehicleTransform.position); // distanceToThis 위치 변수

				if(distanceToThis < distanceToClosest) // distanceToThis < distanceToClosset이라면
				{
					closest = thisTransform; // closset = thisTransform
				}
			}

#if UNITY_EDITOR
			Debug.Log("Teleporting to " + closest.name); // 차량 스폰 시 디버깅 로그
#endif
            vehicleTransform.rotation = closest.rotation; // 차량 회전값 초기화

			var renderer = vehicleTransform.gameObject.GetComponentInChildren<MeshRenderer>(); // renderer 변수
			var wheel = vehicleTransform.gameObject.GetComponentInChildren<WheelCollider>();  // wheel 변수
			RaycastHit hit; // 충돌 감지 hit 변수

			if(Physics.BoxCast(closest.position, renderer.bounds.extents, Vector3.down, out hit, vehicleTransform.rotation, float.MaxValue, ~(1 << LayerMask.NameToLayer("Car"))) ) // 만약 충돌이 감지된다면
			{
				vehicleTransform.position = closest.position + Vector3.down * (hit.distance - wheel.radius); // 차량의 위치 지정
			}
			else // 그렇지 않으면
			{
				Debug.Log("Failed to locate the ground below the spawn point " + closest.name); // 디버깅 로그
				vehicleTransform.position = closest.position; // 위치값 초기화
			}
			var vehicleBody = vehicleTransform.gameObject.GetComponent<Rigidbody>(); // Rigidbody 구성 요소 적용
			vehicleBody.velocity = Vector3.zero; // 차량 속도 0으로 설정
			vehicleBody.angularVelocity = Vector3.zero; // 차량 각속도 0으로 설정
		}
	}
}
