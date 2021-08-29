using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if SINGULAR_MODULE_ENABLE
//! 싱귤러 관리자
public partial class CSingularManager : CSingleton<CSingularManager> {
	//! 매개 변수
	public struct STParams {
		public string m_oAPIKey;
		public string m_oAPISecret;
	}

	//! 콜백 매개 변수
	public struct STCallbackParams {
		public System.Action<CSingularManager, bool> m_oInitCallback;
	}

	#region 변수
	private STParams m_stParams;
	private STCallbackParams m_stCallbackParams;

#if UNITY_IOS || UNITY_ANDROID
	private SingularSDK m_oSingular = null;
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
		var oObj = CFactory.CreateObj(KCDefine.U_OBJ_N_SINGULAR_M_SINGULAR, null);
		oObj.SetActive(false);
		oObj.transform.SetParent(this.transform, false);

		m_oSingular = oObj.AddComponent<SingularSDK>();
		m_oSingular.InitializeOnAwake = false;
#endif			// #if UNITY_IOS || UNITY_ANDROID
	}

	//! 초기화
	public override void Start() {
		base.Start();

#if UNITY_IOS || UNITY_ANDROID
		m_oSingular.gameObject.SetActive(true);
#endif			// #if UNITY_IOS || UNITY_ANDROID
	}

	//! 초기화
	public virtual void Init(STParams a_stParams, STCallbackParams a_stCallbackParams) {
		CAccess.Assert(a_stParams.m_oAPIKey.ExIsValid() && a_stParams.m_oAPISecret.ExIsValid());
		CFunc.ShowLog($"CSingularManager.Init: {a_stParams.m_oAPIKey}, {a_stParams.m_oAPISecret}");

#if UNITY_IOS || UNITY_ANDROID
		// 초기화 되었을 경우
		if(this.IsInit) {
			a_stCallbackParams.m_oInitCallback?.Invoke(this, true);
		} else {
			m_stParams = a_stParams;
			m_stCallbackParams = a_stCallbackParams;

			m_oSingular.SingularAPIKey = a_stParams.m_oAPIKey;
			m_oSingular.SingularAPISecret = a_stParams.m_oAPISecret;

#if UNITY_IOS
			m_oSingular.waitForTrackingAuthorizationWithTimeoutInterval = KCDefine.U_TIMEOUT_SINGULAR_M_AGREE_TRACKING;
#endif			// #if UNITY_IOS

			SingularSDK.InitializeSingularSDK();

			// 추적 동의 상태 일 경우
			if(CCommonAppInfoStorage.Inst.AppInfo.IsAgreeTracking) {
				SingularSDK.TrackingOptIn();	
			}

#if !SINGULAR_ANALYTICS_ENABLE || !(ANALYTICS_TEST_ENABLE || (ADHOC_BUILD || STORE_BUILD))
			SingularSDK.StopAllTracking();
#endif			// #if !SINGULAR_ANALYTICS_ENABLE || !(ANALYTICS_TEST_ENABLE || (ADHOC_BUILD || STORE_BUILD))

			this.ExLateCallFunc((a_oSender, a_oParams) => this.OnInit());
		}
#else
		a_stCallbackParams.m_oInitCallback?.Invoke(this, false);
#endif			// #if UNITY_IOS || UNITY_ANDROID
	}
	#endregion			// 함수

	#region 조건부 함수
#if UNITY_IOS || UNITY_ANDROID
	// 초기화 되었을 경우
	private void OnInit() {
		CScheduleManager.Inst.AddCallback(KCDefine.U_KEY_SINGULAR_M_INIT_CALLBACK, () => {
			CFunc.ShowLog("CSingularManager.OnInit", KCDefine.B_LOG_COLOR_PLUGIN);
			this.IsInit = true;
			
			CFunc.Invoke(ref m_stCallbackParams.m_oInitCallback, this, this.IsInit);
		});
	}
#endif			// #if UNITY_IOS || UNITY_ANDROID
	#endregion			// 조건부 함수
}
#endif			// #if SINGULAR_MODULE_ENABLE
