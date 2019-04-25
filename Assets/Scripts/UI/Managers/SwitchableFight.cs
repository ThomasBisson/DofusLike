using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchableFight : MonoBehaviour
{
    [SerializeField]
    private SpellsAndControls m_spellsAndControls;
    [SerializeField]
    private OthersInfos m_othersInfos;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void FillCallbacksSpellButtons(SpellButton.OnClick method, List<int> ids)
    {
        m_spellsAndControls.FillCallbacksSpellButtons(method, ids);
    }
}
