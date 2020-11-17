using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameLord;

public class Ball : MonoBehaviour
{
    private bool freeze = true;
    public Rigidbody rigidbody;
    public AudioSource audioClipBounce;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (freeze)
        {
            transform.position = new Vector3(transform.position.x, 3, transform.position.z);
            rigidbody.velocity = new Vector3(0, 0, 0);
        }

    } 

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "OpponentSquareLeft")
            MatchManager.Instance.SetBouncePosition(MatchManager.CourtPosition.OpponentSquareLeft);
        else if (collision.gameObject.tag == "OpponentSquareRight")
            MatchManager.Instance.SetBouncePosition(MatchManager.CourtPosition.OpponentSquareRight);
        else if (collision.gameObject.tag == "OpponentHalf")
            MatchManager.Instance.SetBouncePosition(MatchManager.CourtPosition.OpponentHalf);
        else if (collision.gameObject.tag == "PlayerSquareLeft")
            MatchManager.Instance.SetBouncePosition(MatchManager.CourtPosition.PlayerSquareLeft);
        else if (collision.gameObject.tag == "PlayerSquareRight")
            MatchManager.Instance.SetBouncePosition(MatchManager.CourtPosition.PlayerSquareRight);
        else if (collision.gameObject.tag == "PlayerHalf")
            MatchManager.Instance.SetBouncePosition(MatchManager.CourtPosition.PlayerHalf);
        else if(collision.gameObject.tag == "Player" || collision.gameObject.tag == "Racket")
        {

        }
        else
            MatchManager.Instance.SetBouncePosition(MatchManager.CourtPosition.Out);
        audioClipBounce.Play();
    }

    public void Freeze(bool value)
    {
        freeze = value;        
    }
}
