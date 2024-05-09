using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject _red, _green;
    [SerializeField] Text _turnMessage;
    bool _isPlayer, _hasGameFinished;

    const string RED_MESSAGE = "Red's Turn";
    const string GREEN_MESSAGE = "Green's Turn";

    Color RED_COLOR = new Color(231, 29, 54, 255) / 255;
    Color GREEN_COLOR = new Color(0, 222, 1, 255) / 255;

    Board _board;

    private void Awake()
    {
        _isPlayer = true;
        _hasGameFinished = false;
        _turnMessage.text = RED_MESSAGE;
        _turnMessage.color = RED_COLOR;
        _board = new Board();
    }

    public void GameStart()
    {
        SceneManager.LoadScene(0);
    }
    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_hasGameFinished) return;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if (!hit.collider) return;

            if (hit.collider.CompareTag("Clickable"))
            {
                if (hit.collider.gameObject.GetComponent<Column>()._targetLocation.y > 1.5f) return;

                //spawn the GameObject
                Vector3 spawnPos = hit.collider.gameObject.GetComponent<Column>()._spawnLocation;
                Vector3 targetPos = hit.collider.gameObject.GetComponent<Column>()._targetLocation;
                GameObject circle = Instantiate(_isPlayer ? _red : _green);
                circle.transform.position = spawnPos;
                circle.GetComponent<Mover>()._targetPosition = targetPos;

                //Increase the target Location Height
                hit.collider.gameObject.GetComponent<Column>()._targetLocation = new Vector3(targetPos.x, targetPos.y + 0.7f, targetPos.z);

                //Update Board
                _board.UpdateBoard(hit.collider.gameObject.GetComponent<Column>()._col - 1, _isPlayer);
                if (_board.Result(_isPlayer))
                {
                    _turnMessage.text = (_isPlayer ? "Red" : "Green") + " Wins!";
                    _hasGameFinished = true;
                    return;
                }

                //Turn Message
                _turnMessage.text = !_isPlayer ? RED_MESSAGE : GREEN_MESSAGE;
                _turnMessage.color = !_isPlayer ? RED_COLOR : GREEN_COLOR;

                //Change Player Turn
                _isPlayer = !_isPlayer;
            }
        }
    }
}
