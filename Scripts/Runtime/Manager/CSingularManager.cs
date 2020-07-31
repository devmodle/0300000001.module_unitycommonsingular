using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if SINGULAR_ENABLE
//! 싱귤러 관리자
public partial class CSingularManager : CSingleton<CSingularManager> {
	#region 프로퍼티
	public bool IsInit { get; private set; } = false;
	#endregion			// 프로퍼티 

	#region 함수
	//! 초기화
	public virtual void Init(System.Action<CSingularManager, bool> a_oCallback) {
		CFunc.ShowLog("CSingularManager.Init");

		if(!this.IsInit && CAccess.IsMobilePlatform()) {
			this.IsInit = true;
			SingularSDK.InitializeSingularSDK();
		}

		a_oCallback?.Invoke(this, this.IsInit);
	}
	#endregion			// 함수
}
#endif			// #if SINGULAR_ENABLE
