using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if SINGULAR_MODULE_ENABLE
//! 싱귤러 관리자
public partial class CSingularManager : CSingleton<CSingularManager> {
	#region 변수
	private SingularSDK m_oSingularSDK = null;
	private System.Action<CSingularManager, bool> m_oInitCallback = null;
	#endregion			// 변수

	#region 프로퍼티
	public bool IsInit { get; private set; } = false;
	#endregion			// 프로퍼티 

	#region 함수
	//! 초기화
	public override void Awake() {
		base.Awake();

		var oObj = CFactory.CreateObj(KCDefine.U_OBJ_NAME_SINGULAR_SDK, null);
		oObj.SetActive(false);

		m_oSingularSDK = oObj.AddComponent<SingularSDK>();
		m_oSingularSDK.InitializeOnAwake = false;

#if UNITY_EDITOR
		oObj.transform.SetParent(this.transform, false);
#endif			// #if UNITY_EDITOR
	}

	//! 초기화
	public override void Start() {
		base.Start();
		m_oSingularSDK.gameObject.SetActive(true);
	}

	//! 초기화
	public virtual void Init(string a_oAPIKey, 
		string a_oAPISecret, System.Action<CSingularManager, bool> a_oCallback) 
	{
		CAccess.Assert(a_oAPIKey.ExIsValid() && a_oAPISecret.ExIsValid());
		CFunc.ShowLog("CSingularManager.Init: {0}, {1}", a_oAPIKey, a_oAPISecret);

#if UNITY_IOS || UNITY_ANDROID
		// 초기화 되었을 경우
		if(this.IsInit) {
			a_oCallback?.Invoke(this, true);
		} else {
			m_oInitCallback = a_oCallback;

			m_oSingularSDK.SingularAPIKey = a_oAPIKey;
			m_oSingularSDK.SingularAPISecret = a_oAPISecret;

			SingularSDK.InitializeSingularSDK();

#if SINGULAR_ANALYTICS_ENABLE
			// 약관 동의가 필요 할 경우
			if(!CCommonUserInfoStorage.Inst.UserInfo.IsAgree) {
				SingularSDK.TrackingOptIn();	
			}

#if !ANALYTICS_TEST_ENABLE && (!ADHOC_BUILD && !STORE_BUILD)
			SingularSDK.StopAllTracking();
#endif			// #if !ANALYTICS_TEST_ENABLE && (!ADHOC_BUILD && !STORE_BUILD)
#else
			SingularSDK.TrackingOptIn();
			SingularSDK.StopAllTracking();
#endif			// SINGULAR_ANALYTICS_ENABLE

			this.ExLateCallFunc((a_oSender, a_oParams) => this.OnInit());
		}
#else
		a_oCallback?.Invoke(this, false);
#endif			// #if UNITY_IOS || UNITY_ANDROID
	}
	#endregion			// 함수

	#region 조건부 함수
#if UNITY_IOS || UNITY_ANDROID
	//! 초기화 되었을 경우
	private void OnInit() {
		CScheduleManager.Inst.AddCallback(KCDefine.U_KEY_SINGULAR_M_INIT_CALLBACK, () => {
			CFunc.ShowLog("CSingularManager.OnInit", KCDefine.B_LOG_COLOR_PLUGIN);

			this.IsInit = true;
			CFunc.Invoke(ref m_oInitCallback, this, this.IsInit);
		});
	}
#endif			// #if UNITY_IOS || UNITY_ANDROID
	#endregion			// 조건부 함수
}
#endif			// #if SINGULAR_MODULE_ENABLE
