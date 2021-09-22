using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/*
 * Simple GUI display : scores and team turn
 */

public class GUIMgr : MonoBehaviour
{
    [SerializeField]
    public Text myPseudo;

    [SerializeField]
    public Text opponentPseudo;

    #region singleton
    static GUIMgr instance = null;
    public static GUIMgr Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<GUIMgr>();
            return instance;
        }
    }
    #endregion

    Transform whiteToMoveTr = null;
    Transform blackToMoveTr = null;
    Text whiteScoreText = null;
    Text blackScoreText = null;

    void Awake()
    {
        whiteToMoveTr = transform.Find("WhiteTurnText");
        blackToMoveTr = transform.Find("BlackTurnText");

        whiteToMoveTr.gameObject.SetActive(false);
        blackToMoveTr.gameObject.SetActive(false);

        whiteScoreText = transform.Find("WhiteScoreText").GetComponent<Text>();
        blackScoreText = transform.Find("BlackScoreText").GetComponent<Text>();

        ChessGameMgr.Instance.OnPlayerTurn += DisplayTurn;
        ChessGameMgr.Instance.OnScoreUpdated += UpdateScore;
    }
	
    void UpdateScore(uint whiteScore, uint blackScore)
    {
        whiteScoreText.text = string.Format("White : {0}", whiteScore);
        blackScoreText.text = string.Format("Black : {0}", blackScore);
    }

    public void DisplayTurn(bool isWhiteMove)
    {
        whiteToMoveTr.gameObject.SetActive(isWhiteMove);
        blackToMoveTr.gameObject.SetActive(!isWhiteMove);
    }

    public void SetMyPseudo(Text pseudo)
    {
        myPseudo.text = pseudo.text;
    }

    public void SetOpponentPseudo(string pseudo)
    {
        opponentPseudo.text = pseudo;
    }
}
