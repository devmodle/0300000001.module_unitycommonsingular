using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if SINGULAR_MODULE_ENABLE
//! 싱귤러 관리자
public partial class CSingularManager : CSingleton<CSingularManager> {
	#region 변수
#if UNITY_IOS || UNITY_ANDROID
	private SingularSDK m_oSingularSDK = null;
#endif			// #if UNITY_IOS || UNITY_ANDROID
	#endregion			// 변수

	#region 프로퍼티
	public bool IsInit { get; private set; } = false;
	#endregion			// 프로퍼티 

	#region 함수
	//! 초기화
	public override void Awake() {
		base.Awake();

#if UNITY_IOS || UNITY_ANDROID
		m_oSingularSDK = CFactory.CreateCloneObj<SingularSDK>(KCDefine.U_OBJ_NAME_SINGULAR_SDK,
			CResManager.Instance.GetPrefab(KCDefine.U_OBJ_PATH_SINGULAR_SDK),
			this.gameObject);

		CAccess.Assert(m_oSingularSDK != null);
#endif			// #if UNITY_IOS || UNITY_ANDROID
	}

	//! 초기화
	public virtual void Init(string a_oAPIKey, string a_oAPISecret, System.Action<CSingularManager, bool> a_oCallback) {
		CAccess.Assert(a_oAPIKey.ExIsValid() && a_oAPISecret.ExIsValid());
		CFunc.ShowLog("CSingularManager.Init: {0}, {1}", a_oAPIKey, a_oAPISecret);

#if UNITY_IOS || UNITY_ANDROID
		// 초기화 되었을 경우
		if(this.IsInit) {
			a_oCallback?.Invoke(this, true);
		} else {
			m_oSingularSDK.SingularAPIKey = a_oAPIKey;
			m_oSingularSDK.SingularAPISecret = a_oAPISecret;

			SingularSDK.InitializeSingularSDK();
			
			// 약관 동의가 필요 할 경우
			if(!CCommonUserInfoStorage.Instance.UserInfo.IsAgree) {
				SingularSDK.TrackingOptIn();
			}

			this.IsInit = true;
			a_oCallback?.Invoke(this, this.IsInit);
		}
#else
		a_oCallback?.Invoke(this, false);
#endif			// #if UNITY_IOS || UNITY_ANDROID
	}
	#endregion			// 함수
}
#endif			// #if SINGULAR_MODULE_ENABLE
