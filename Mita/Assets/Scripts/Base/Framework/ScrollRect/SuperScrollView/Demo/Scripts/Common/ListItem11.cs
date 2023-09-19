using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ListItem11 : BaseUIItem
    {
        public Button mRootButton;
        public Text mText;
        public GameObject mWaitingIcon;

        public override void OnBlind()
        {
            mRootButton.onClick.AddListener(OnLoadMoreBtnClicked);
        }

        public override void OnRelease()
        {

        }

        public override void SetData(object param, params object[] args)
        {
            ClientLog.Instance.Log("你说的都对 都对 11111111");
        }

        void OnLoadMoreBtnClicked()
        {
            ClientLog.Instance.Log("你说的都对 都对");
        }
    }
}
