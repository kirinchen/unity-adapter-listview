using UnityEngine;
using System.Collections;

public class ListViewUtils : MonoBehaviour {

    internal void enableFast(ItemBundle ib, bool b) {
        Vector3 v = b ? new Vector3(1, 1, 1) : Vector3.zero;
        ib.trans.localScale = v;
        ib.enableStatus = b ? ItemBundle.EnableStatus.Enabled : ItemBundle.EnableStatus.Disabled;
    }

    internal void enableOnAnim(ItemBundle ib, bool b) {
        ib.enableStatus = b ? ItemBundle.EnableStatus.Enabling : ItemBundle.EnableStatus.Disabling;
        StartCoroutine(runEnable(ib));
    }

    private IEnumerator runEnable(ItemBundle ib) {
        while (ib.isScaling()) {
            Vector3 toV = ib.getScaleTo();
            float d = Vector3.Distance(toV, ib.getCurrentScale());
            if (d < 0.005f) {
                ib.setScaleFinshStatus();
            } else {
                Vector3 shiftV = (toV - ib.getCurrentScale()) * 0.035f;
                ib.trans.localScale += shiftV;
            }
            yield return new WaitForSeconds(0.01f);
        }
    }


}
