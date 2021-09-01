using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if SINGULAR_MODULE_ENABLE
#if PURCHASE_MODULE_ENABLE
using UnityEngine.Purchasing;
#endif			// #if PURCHASE_MODULE_ENABLE

//! 싱귤러 관리자 - 분석
public partial class CSingularManager : CSingleton<CSingularManager> {
	#region 함수
	//! 분석 유저 식별자를 변경한다
	public void SetAnalyticsUserID(string a_oID) {
		CFunc.ShowLog($"CSingularManager.SetAnalyticsUserID: {a_oID}", KCDefine.B_LOG_COLOR_PLUGIN);
		CAccess.Assert(a_oID.ExIsValid());

#if (UNITY_IOS || UNITY_ANDROID) && SINGULAR_ANALYTICS_ENABLE
		// 초기화 되었을 경우
		if(this.IsInit) {
			SingularSDK.SetCustomUserId(a_oID);
		}
#endif			// #if (UNITY_IOS || UNITY_ANDROID) && SINGULAR_ANALYTICS_ENABLE
	}

	//! 메세지 토큰을 변경한다
	public void SetMsgToken(string a_oToken) {
		CAccess.Assert(a_oToken.ExIsValid());

#if (UNITY_IOS || UNITY_ANDROID) && SINGULAR_ANALYTICS_ENABLE
#if UNITY_IOS
		SingularSDK.RegisterDeviceTokenForUninstall(a_oToken);
#else
		SingularSDK.SetFCMDeviceToken(a_oToken);
#endif			// #if UNITY_IOS
#endif			// #if (UNITY_IOS || UNITY_ANDROID) && SINGULAR_ANALYTICS_ENABLE
	}

	//! 로그를 전송한다
	public void SendLog(string a_oName, Dictionary<string, object> a_oDataDict) {
		CFunc.ShowLog($"CSingularManager.SendLog: {a_oName}, {a_oDataDict}", KCDefine.B_LOG_COLOR_PLUGIN);
		CAccess.Assert(a_oName.ExIsValid());

#if ((UNITY_IOS || UNITY_ANDROID) && SINGULAR_ANALYTICS_ENABLE) && (ANALYTICS_TEST_ENABLE || ADHOC_BUILD || STORE_BUILD)
		// 초기화 되었을 경우
		if(this.IsInit) {
			var oDataDict = a_oDataDict ?? new Dictionary<string, object>();

			oDataDict.ExAddVal(KCDefine.L_LOG_KEY_DEVICE_ID, CCommonAppInfoStorage.Inst.AppInfo.DeviceID);
			oDataDict.ExAddVal(KCDefine.L_LOG_KEY_PLATFORM, CCommonAppInfoStorage.Inst.Platform);

#if AUTO_LOG_PARAMS_ENABLE
			oDataDict.ExAddVal(KCDefine.L_LOG_KEY_USER_TYPE, CCommonUserInfoStorage.Inst.UserInfo.UserType.ToString());
			oDataDict.ExAddVal(KCDefine.L_LOG_KEY_LOG_TIME, System.DateTime.UtcNow.ExToLongStr());
			oDataDict.ExAddVal(KCDefine.L_LOG_KEY_INSTALL_TIME, CCommonAppInfoStorage.Inst.AppInfo.UTCInstallTime.ExToLongStr());
#endif			// #if AUTO_LOG_PARAMS_ENABLE

			SingularSDK.Event(oDataDict, a_oName);
		}
#endif			// #if ((UNITY_IOS || UNITY_ANDROID) && SINGULAR_ANALYTICS_ENABLE) && (ANALYTICS_TEST_ENABLE || ADHOC_BUILD || STORE_BUILD)
	}
	#endregion			// 함수

	#region 조건부 함수
#if PURCHASE_MODULE_ENABLE
	//! 결제 로그를 전송한다
	public void SendPurchaseLog(Product a_oProduct) {
		CFunc.ShowLog($"CSingularManager.SendPurchaseLog: {a_oProduct}", KCDefine.B_LOG_COLOR_PLUGIN);
		CAccess.Assert(a_oProduct != null);

#if ((UNITY_IOS || UNITY_ANDROID) && SINGULAR_ANALYTICS_ENABLE) && (ANALYTICS_TEST_ENABLE || ADHOC_BUILD || STORE_BUILD)
		// 초기화 되었을 경우
		if(this.IsInit) {
			SingularSDK.InAppPurchase(a_oProduct, null);
		}
#endif			// #if ((UNITY_IOS || UNITY_ANDROID) && SINGULAR_ANALYTICS_ENABLE) && (ANALYTICS_TEST_ENABLE || ADHOC_BUILD || STORE_BUILD)
	}
#endif			// #if PURCHASE_MODULE_ENABLE
	#endregion			// 조건부 함수
}
#endif			// #if SINGULAR_MODULE_ENABLE
