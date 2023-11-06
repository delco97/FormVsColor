using System;
using TMPro;
using UnityEngine;

public class EndMenu : MonoBehaviour {

    [SerializeField] private GameObject title;
    
    public void Initialize(MatchResult matchResult) {
        //TODO: set animation
        switch (matchResult) {
            case MatchResult.DRAW:
                title.GetComponent<TextMeshProUGUI>().text = "Pareggio!";
                break;
            case MatchResult.FORM_WIN:
                title.GetComponent<TextMeshProUGUI>().text = "Ha vinto Form!";
                break;
                break;
            case MatchResult.COLOR_WIN:
                title.GetComponent<TextMeshProUGUI>().text = "Ha vinto Color!";
                break;            
            default:
                throw new ArgumentException("Invalid match result: " + matchResult);
        }
    }
    
}
