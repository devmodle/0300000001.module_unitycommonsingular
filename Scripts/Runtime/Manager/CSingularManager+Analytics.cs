using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if SINGULAR_MODULE_ENABLE
#if PURCHASE_MODULE_ENABLE
using UnityEngine.Purchasing;
#endif			// #if PURCHASE_MODULE_ENABLE

//! 싱귤러 관리자 - 분석
public partial class CSingularManager : CSingleton<CSingularManager> {
	#region 함수
	//! 분석 유저 식별자를 변경한다
	public void SetAnalyticsUserID(string a_oID) {
		CAccess.Assert(a_oID.ExIsValid());
		CFunc.ShowLog("CSingularManager.SetAnalyticsUserID: {0}", KCDefine.B_LOG_COLOR_PLUGIN, a_oID);

#if SINGULAR_ANALYTICS_ENABLE && (UNITY_IOS || UNITY_ANDROID)
		// 초기화 되었을 경우
		if(this.IsInit) {
			SingularSDK.SetCustomUserId(a_oID);
		}
#endif			// #if SINGULAR_ANALYTICS_ENABLE && (UNITY_IOS || UNITY_ANDROID)
	}

	//! 메세지 토큰을 변경한다
	public void SetMsgToken(string a_oToken) {
		CAccess.Assert(a_oToken.ExIsValid());

#if SINGULAR_ANALYTICS_ENABLE && (UNITY_IOS || UNITY_ANDROID)
#if UNITY_IOS
		SingularSDK.RegisterDeviceTokenForUninstall(a_oToken);
#else
		SingularSDK.SetFCMDeviceToken(a_oToken);
#endif			// #if UNITY_IOS
#endif			// #if SINGULAR_ANALYTICS_ENABLE && (UNITY_IOS || UNITY_ANDROID)
	}

	//! 로그를 전송한다
	public void SendLog(string a_oName, Dictionary<string, object> a_oDataList) {
		CAccess.Assert(a_oName.ExIsValid());
		CFunc.ShowLog("CSingularManager.SendLog: {0}, {1}", KCDefine.B_LOG_COLOR_PLUGIN, a_oName, a_oDataList);

#if SINGULAR_ANALYTICS_ENABLE && (UNITY_IOS || UNITY_ANDROID)
#if ANALYTICS_TEST_ENABLE || (ADHOC_BUILD || STORE_BUILD)
		// 초기화 되었을 경우
		if(this.IsInit) {
			var oDataList = a_oDataList ?? new Dictionary<string, object>();

			oDataList.ExAddValue(KCDefine.L_LOG_KEY_DEVICE_ID, CCommonAppInfoStorage.Inst.AppInfo.DeviceID);
			oDataList.ExAddValue(KCDefine.L_LOG_KEY_PLATFORM, CCommonAppInfoStorage.Inst.Platform);

#if AUTO_LOG_PARAMS_ENABLE
			oDataList.ExAddValue(KCDefine.L_LOG_KEY_USER_TYPE, CCommonUserInfoStorage.Inst.UserInfo.UserType.ToString());
			oDataList.ExAddValue(KCDefine.L_LOG_KEY_LOG_TIME, System.DateTime.UtcNow.ExToLongStr());
			oDataList.ExAddValue(KCDefine.L_LOG_KEY_INSTALL_TIME, CCommonAppInfoStorage.Inst.AppInfo.UTCInstallTime.ExToLongStr());
#endif			// #if AUTO_LOG_PARAMS_ENABLE

			SingularSDK.Event(oDataList, a_oName);
		}
#endif			// #if ANALYTICS_TEST_ENABLE || (ADHOC_BUILD || STORE_BUILD)
#endif			// #if SINGULAR_ANALYTICS_ENABLE && (UNITY_IOS || UNITY_ANDROID)
	}
	#endregion			// 함수

	#region 조건부 함수
#if PURCHASE_MODULE_ENABLE
	//! 결제 로그를 전송한다
	public void SendPurchaseLog(Product a_oProduct) {
		CAccess.Assert(a_oProduct != null);
		CFunc.ShowLog("CSingularManager.SendPurchaseLog: {0}", KCDefine.B_LOG_COLOR_PLUGIN, a_oProduct);

#if SINGULAR_ANALYTICS_ENABLE && (UNITY_IOS || UNITY_ANDROID)
#if ANALYTICS_TEST_ENABLE || (ADHOC_BUILD || STORE_BUILD)
		// 초기화 되었을 경우
		if(this.IsInit) {
			SingularSDK.InAppPurchase(a_oProduct, null);
		}
#endif			// #if ANALYTICS_TEST_ENABLE || (ADHOC_BUILD || STORE_BUILD)
#endif			// #if SINGULAR_ANALYTICS_ENABLE && (UNITY_IOS || UNITY_ANDROID)
	}
#endif			// #if PURCHASE_MODULE_ENABLE
	#endregion			// 조건부 함수
}
#endif			// #if SINGULAR_MODULE_ENABLE
