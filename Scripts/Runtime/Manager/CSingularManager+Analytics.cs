using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if SINGULAR_MODULE_ENABLE && SINGULAR_ANALYTICS_ENABLE
#if PURCHASE_MODULE_ENABLE
using UnityEngine.Purchasing;
#endif			// #if PURCHASE_MODULE_ENABLE

//! 싱귤러 관리자 - 분석
public partial class CSingularManager : CSingleton<CSingularManager> {
	#region 함수
	//! 분석 유저 식별자를 변경한다
	public void SetAnalyticsUserID(string a_oID) {
		CFunc.ShowLog("CSingularManager.SetAnalyticsUserID: {0}", KCDefine.B_LOG_COLOR_PLUGIN, a_oID);

#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
		// 초기화 되었을 경우
		if(this.IsInit) {
			SingularSDK.SetCustomUserId(a_oID);
		}
#endif			// #if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
	}
	
	//! 로그를 전송한다
	public void SendLog(string a_oName) {
		this.SendLog(a_oName, null);
	}

	//! 로그를 전송한다
	public void SendLog(string a_oName, string a_oParam, List<string> a_oDataList) {
		this.SendLog(a_oName, new Dictionary<string, object>() {
			[a_oParam] = a_oDataList.ExToString(KCDefine.B_TOKEN_CSV_STRING)
		});
	}

	//! 로그를 전송한다
	public void SendLog(string a_oName, Dictionary<string, object> a_oDataList) {
		CFunc.ShowLog("CSingularManager.SendLog: {0}, {1}", KCDefine.B_LOG_COLOR_PLUGIN, a_oName, a_oDataList);

#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
#if ANALYTICS_TEST_ENABLE || (ADHOC_BUILD || STORE_BUILD)
		// 초기화 되었을 경우
		if(this.IsInit) {
			var oDataList = a_oDataList ?? new Dictionary<string, object>();

#if MSG_PACK_ENABLE
			oDataList.ExAddValue(KCDefine.U_LOG_KEY_DEVICE_ID, CCommonAppInfoStorage.Instance.AppInfo.DeviceID);

#if AUTO_LOG_PARAMS_ENABLE
			oDataList.ExAddValue(KCDefine.U_LOG_KEY_PLATFORM, CCommonAppInfoStorage.Instance.PlatformName);
			oDataList.ExAddValue(KCDefine.U_LOG_KEY_USER_TYPE, CCommonUserInfoStorage.Instance.UserInfo.UserType.ToString());
			
			oDataList.ExAddValue(KCDefine.U_LOG_KEY_LOG_TIME, System.DateTime.UtcNow.ExToLongString());
			oDataList.ExAddValue(KCDefine.U_LOG_KEY_INSTALL_TIME, CCommonAppInfoStorage.Instance.AppInfo.UTCInstallTime.ExToLongString());
#endif			// #if AUTO_LOG_PARAMS_ENABLE
#endif			// #if MSG_PACK_ENABLE

			SingularSDK.Event(oDataList, a_oName);
		}
#endif			// #if ANALYTICS_TEST_ENABLE || (ADHOC_BUILD || STORE_BUILD)
#endif			// #if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
	}
	#endregion			// 함수

	#region 조건부 함수
#if PURCHASE_MODULE_ENABLE
	//! 결제 로그를 전송한다
	public void SendPurchaseLog(Product a_oProduct, Dictionary<string, object> a_oDataList) {
		CFunc.ShowLog("CSingularManager.SendPurchaseLog: {0}", KCDefine.B_LOG_COLOR_PLUGIN, a_oProduct);

#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
#if ANALYTICS_TEST_ENABLE || (ADHOC_BUILD || STORE_BUILD)
		// 초기화 되었을 경우
		if(this.IsInit) {
			SingularSDK.InAppPurchase(a_oProduct, a_oDataList);
		}
#endif			// #if ANALYTICS_TEST_ENABLE || (ADHOC_BUILD || STORE_BUILD)
#endif			// #if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
	}
#endif			// #if PURCHASE_MODULE_ENABLE
	#endregion			// 조건부 함수
}
#endif			// #if SINGULAR_MODULE_ENABLE && SINGULAR_ANALYTICS_ENABLE
