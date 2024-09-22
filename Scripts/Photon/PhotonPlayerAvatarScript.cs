using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonPlayerAvatarScript : MonoBehaviour, IPunObservable
{
    #region References to Components, Scripts and Objects
    [Header("References To Objects, Scripts, Components")]
    [SerializeField] private PhotonView photonView;
    [SerializeField] private Animator avatarAnimator;
    [SerializeField] private Rigidbody2D avatarRigidbody;
    //[SerializeField] private SpriteRenderer avatarSprite;
    [SerializeField] private float movementSpeed = 1f;
    #endregion

    #region Functions for Jumping
    [SerializeField] LayerMask groundLayerMask;
    private Vector2 groundRayPosition = new Vector2();

    private bool CheckIfPlayerOnGround()
    {
        return false;
    }
    #endregion

    #region Lag Compensation Functions
    private Vector3 positionOnNetwork;
    private Vector2 velocityOnNetwork;

    private void MovePlayerWithLagCompensation()
    {
        avatarRigidbody.position = Vector3.Lerp(avatarRigidbody.position, positionOnNetwork, Time.deltaTime * 5);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(avatarRigidbody.velocity);
        }
        else if (stream.IsReading)
        {
            positionOnNetwork = (Vector3)stream.ReceiveNext();
            velocityOnNetwork = (Vector2)stream.ReceiveNext();

            //getting network position with lag
            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            positionOnNetwork += (Vector3)velocityOnNetwork * lag;
        }
    }
    #endregion

    #region Camera Functions
    private Camera mainCamera;

    private IEnumerator Routine_FindTheCamera()
    {
        while (mainCamera == null)
        {
            mainCamera = Camera.main;
            yield return new WaitForSeconds(0.2f);
        }
        yield break;
    }

    private void SetPositionOfMainCameraForLocalPlayer()
    {
        mainCamera.gameObject.transform.position = 
            new Vector3(transform.position.x, transform.position.y, -10); //make it to +10 when flipped or create new

        //mainCamera.transform.parent = transform;
    }

    #endregion

    #region Player Move Function
    private void HandleFlippingOfSpriteBaseOnInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            youTextCanvas.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (horizontalInput < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            youTextCanvas.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 180, 0);
            //youTextCanvas.transform.rotation = Quaternion.Euler(5, -5, 5);
        }

    }
    
    private void PlayerMovementOfPlayer()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        Vector2 movement = new Vector2(horizontalInput, avatarRigidbody.velocity.y);

        if (Mathf.Abs(horizontalInput) > 0)
        {
            avatarRigidbody.velocity = movement * movementSpeed;
        }
        
    }

    #region Useless PlayerStatesInThisCase
    public enum PlayerStates
    {
        Walk,
        Idle,
        OffGround
    }

    public PlayerStates playerStates = PlayerStates.Idle;

    private void HandlePlayerStates()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        //player is off ground
        if (CheckIfPlayerOnGround() == false)
        {
            playerStates = PlayerStates.OffGround;
            return;
        }

        //walking
        if (Mathf.Abs(horizontalInput) > 0)
        {
            playerStates = PlayerStates.Walk;
            return;
        }

        playerStates = PlayerStates.Idle;
    }

    private void HandleVariousPlayerStatesInUpdate()
    {
        switch (playerStates)
        {
            case PlayerStates.Idle:
                PlayerMovementOfPlayer();
                break;
            case PlayerStates.Walk:
                PlayerMovementOfPlayer();
                break;
        }
    }
    #endregion

    #endregion

    #region Animator and Animation
    private static readonly int walkAnimation = Animator.StringToHash("walk");
    private static readonly int idleAnimation = Animator.StringToHash("idle");
    private static readonly int velocity = Animator.StringToHash("Velocity");

    //Will handle animation transitions
    private void HandleAnimationTransitions() //mathF.Abs = Absolute | it returns positive values, so walking back will be + aswell
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        avatarAnimator.SetFloat(velocity, Mathf.Abs(horizontalInput)); 
    }
    #endregion

    #region You Text Canvas Function
    [Header("You Text Canvas")]
    [SerializeField] private GameObject youTextCanvas;

    private void EnableYouTextCanvas()
    {
        youTextCanvas.SetActive(true);
    }
    #endregion

    #region Unity Functions
    private void Start()
    {
        if (photonView.IsMine == false)
        {
            avatarRigidbody.bodyType = RigidbodyType2D.Kinematic; //idk sounds kinda stupid
            return;
        }
            

        StartCoroutine(Routine_FindTheCamera());

        //If the player is local player, than enable the you text
        EnableYouTextCanvas();
    }

    private void Update()
    {
        if (photonView.IsMine == false) return;

        HandlePlayerStates();
        HandleAnimationTransitions();
        HandleFlippingOfSpriteBaseOnInput();
        SetPositionOfMainCameraForLocalPlayer();
    }
    private void FixedUpdate()
    {
        if (photonView.IsMine == false)
        {
            MovePlayerWithLagCompensation();
            return;
        } 

        
        HandleVariousPlayerStatesInUpdate();
    }
    #endregion


}
