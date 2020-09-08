using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if SINGULAR_MODULE_ENABLE
//! 싱귤러 관리자
public partial class CSingularManager : CSingleton<CSingularManager> {
	#region 컴포넌트
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
	private SingularSDK m_oSingularSDK = null;
#endif			// #if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
	#endregion			// 컴포넌트

	#region 프로퍼티
	public bool IsInit { get; private set; } = false;
	#endregion			// 프로퍼티 

	#region 함수
	//! 초기화
	public override void Awake() {
		base.Awake();

#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
		m_oSingularSDK = CFactory.CreateCloneObj<SingularSDK>(KCDefine.U_OBJ_NAME_SINGULAR_SDK,
			CResManager.Instance.GetPrefab(KCDefine.U_OBJ_PATH_SINGULAR_SDK),
			this.gameObject);
#endif			// #if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
	}

	//! 초기화
	public virtual void Init(System.Action<CSingularManager, bool> a_oCallback) {
		CFunc.ShowLog("CSingularManager.Init");
		
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
		// 초기화 가능 할 경우
		if(!this.IsInit && CAccess.IsMobilePlatform()) {
			this.IsInit = true;
			SingularSDK.InitializeSingularSDK();

#if MSG_PACK_ENABLE
			// 약관 동의가 필요 할 경우
			if(!CCommonUserInfoStorage.Instance.UserInfo.IsAgree) {
				SingularSDK.TrackingOptIn();
			}
#else
			SingularSDK.TrackingOptIn();
#endif			// #if MSG_PACK_ENABLE
		}
#endif			// #if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)

		a_oCallback?.Invoke(this, this.IsInit);
	}
	#endregion			// 함수
}
#endif			// #if SINGULAR_MODULE_ENABLE
