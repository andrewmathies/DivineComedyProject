using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    private float horizontalInput, runningTimer = 15f;
    private Vector3 curPos, virgilPos, nessusPos;
    private SpriteRenderer danteSR, virgilSR, nessusSR;
    private bool dialogueActive, centaursRunning, first, dgEnded, nessusLeaving;
    private Dialogue[] dgs;
    private int curDg = 0;

    public float minInput, moveDist, followDist;
    public GameObject virgil, dialogueBox, Minotaur, dialogueManager, lavaBridgeTile;
    public GameObject centaur0, centaur1, centaur2;

    enum State {
        Start,
        MinotaurDialogue,
        Landslide,
        CentaurDialogue,
        ChironDialogue,
        Follow,
        NessusDialogue,
        NessusDialogue2,
        NessusDialogue3,
        End
    }

    private State curState;

    void Start() {
        curState = State.Start;

        initDgs();
        initLavaBridge();

        danteSR = GetComponent<SpriteRenderer>();
        virgilSR = virgil.GetComponent<SpriteRenderer>();
        nessusSR = centaur0.GetComponent<SpriteRenderer>();

        dialogueActive = false;
        centaursRunning = false;
        first = true;
        dgEnded = false;
        nessusLeaving = false;
    }

	// Update is called once per frame
	void Update () {
        horizontalInput = Input.GetAxis("Horizontal");

        if (centaursRunning) {
            runningTimer -= Time.deltaTime;
            if (runningTimer < 0) {
                centaursRunning = false;
                centaur1.SetActive(false);
                centaur2.SetActive(false);
                runningTimer = 15f;
            }
        } else if (nessusLeaving) {
            runningTimer -= Time.deltaTime;
            if (runningTimer < 0) {
                nessusLeaving = false;
                centaur0.SetActive(false);
            }
        }
    }

    void FixedUpdate()
    {
        curPos = transform.position;
        virgilPos = virgil.transform.position;
        nessusPos = centaur0.transform.position;

        handleGame();

        // centaurs
        if (centaursRunning) {
            Vector3 cPos1, cPos2;
            cPos1 = centaur1.transform.position;
            cPos2 = centaur2.transform.position;

            centaur1.transform.SetPositionAndRotation(new Vector3(cPos1.x + (moveDist * 1.8f), cPos1.y), Quaternion.identity);
            centaur2.transform.SetPositionAndRotation(new Vector3(cPos2.x + (moveDist * 1.8f), cPos2.y), Quaternion.identity);
        } else if (nessusLeaving) {
            centaur0.transform.SetPositionAndRotation(new Vector3(nessusPos.x - (moveDist * 1.5f), nessusPos.y), Quaternion.identity);
        }

        if (!dialogueActive)
            handleMovement();
    }

    void handleGame() {
        if (curState == State.Start && curPos.x >= 7) {
            curState = State.MinotaurDialogue;
            startDg(dgs[curDg++]);  
        } else if (curState == State.MinotaurDialogue && curPos.x <= -8.6) {
            Minotaur.SetActive(false);
            curState = State.Landslide;
            startDg(dgs[curDg++]);
        } else if (curState == State.Landslide && curPos.x >= 23.5) {
            curState = State.CentaurDialogue;
            first = true;
            startDg(dgs[curDg++]);
            nessusSR.flipX = false;
            centaur1.GetComponent<SpriteRenderer>().flipX = false;
        } else if (curState == State.ChironDialogue && curPos.x >= 27) {
            startDg(dgs[curDg++]);
            curState = State.Follow;
        } else if (curState == State.Follow && curDg == 7 && dgEnded) {
            curState = State.NessusDialogue;
            centaursRunning = true;
            Debug.Log("started centaurs running");
            nessusSR.flipX = true;
            centaur1.GetComponent<SpriteRenderer>().flipX = true;
            centaur2.GetComponent<SpriteRenderer>().flipX = true;
        } else if (curState == State.NessusDialogue && curPos.x >= 51) {
            curState = State.NessusDialogue2;
            startDg(dgs[curDg++]);
        } else if (curState == State.NessusDialogue2 && curPos.x >= 57.6) {
            curState = State.NessusDialogue3;
            startDg(dgs[curDg++]);
        } else if (curState == State.NessusDialogue3 && curPos.x >= 80) {
            curState = State.End;
            startDg(dgs[curDg++]);
            nessusSR.flipX = false;
        }
    }

    void handleMovement() {
        if (horizontalInput > minInput)
        {
            if ((curState == State.MinotaurDialogue && curPos.x >= 4) || curPos.x >= 89.3)
                return;

            // move right
            transform.position = new Vector3(curPos.x + moveDist, curPos.y);
            curPos.x = curPos.x + moveDist;
            danteSR.flipX = true;
        } else if (horizontalInput < minInput * -1)
        {
            if (curPos.x > -12) { // hard cap on movement in level
                // move left
                transform.position = new Vector3(curPos.x - moveDist, curPos.y);
                curPos.x = curPos.x - moveDist;
                danteSR.flipX = false;
            }
        }

        // make sure virgil is following dante!
        if (virgilPos.x > curPos.x + followDist)
        {
            virgil.transform.SetPositionAndRotation(new Vector3(curPos.x + followDist, virgilPos.y), Quaternion.identity);
            virgilSR.flipX = false;
        }
        else if (virgilPos.x < curPos.x - followDist)
        {
            virgil.transform.SetPositionAndRotation(new Vector3(curPos.x - followDist, virgilPos.y), Quaternion.identity);
            virgilSR.flipX = true;
        }

        // push Nessus
        if (nessusPos.x <= curPos.x + followDist && curState != State.End) {
            nessusSR.flipX = true;
            centaur0.transform.SetPositionAndRotation(new Vector3(nessusPos.x + moveDist, nessusPos.y), Quaternion.identity);
        } else if (nessusPos.x >= curPos.x + (1.2 * followDist)) {
            centaur0.GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    void startDg(Dialogue dg) {
        dialogueBox.SetActive(true);
        dialogueActive = true;
        dialogueManager.GetComponent<DialogueManager>().StartDialogue(dg);
    }

    public void DialogueOver() {
        dialogueActive = false;
        dialogueBox.SetActive(false);

        if (curDg == 3 || curDg == 5 || curDg == 6) {
            startDg(dgs[curDg++]);
            return;
        } else if (curDg == 7)
            dgEnded = true;

        if (curState == State.MinotaurDialogue && first) {
            first = false;
            Animator anim = Minotaur.GetComponent<Animator>();
            anim.SetBool("angry", true);
            startDg(dgs[10]);
        } else if (curState == State.CentaurDialogue && first) {
            // virgil turns to dante and talks to him
            first = false;
            danteSR.flipX = false;
            startDg(dgs[11]);
            curState = State.ChironDialogue;
        } else if (curState == State.End) {
            // Nessus heads back
            nessusLeaving = true;
        }
    }

    void initDgs() {
        dgs = new Dialogue[12];

        for (int i = 0; i < 12; i++) {
            dgs[i] = new Dialogue();
        }

        dgs[0].name = "Virgil";
        string[] sentences0 = {"Perhaps you think this is Theseus, the Duke of Athens who dealt you your death in the world above. Out of our way, beast!",
        "This man does not come schooled by your sister. No, his journey is to see how all your punishment is exacted here."};
        dgs[0].sentences = sentences0;

        dgs[10].name = "Virgil";
        string[] sentences05 = {"Run for the slope! It would be best for you to get down while he is in his rage."};
        dgs[10].sentences = sentences05;

        dgs[1].name = "Virgil";
        string[] sentences1 = {"Are you wondering about this landslide, guarded by that bestial fury I just now gentled? I would have you know that the previous time I descended this far into lower Hell this rock had not yet cascaded down.",
        "But certainly, if my reckoning is right, shortly before He harrowed Hell, and bore away the great plunder of the topmost circle, the reeking abyss trembled so hard that I thought the universe was in the grip of the love by which, as some believe.",
        "The world has often been reduced to chaos. It was at that moment that this ancient rock, here and elsewhere, tumbled in avalanche.",
        "But fix your eyes down below, for we are near the River of Blood, in which are cooked those whose violence has injured others."};
        dgs[1].sentences = sentences1;

        dgs[2].name = "Centaur";
        string[] sentences2 = {"Bound for what torment do you descend the slope? Tell me from there, or I draw my bow."};
        dgs[2].sentences = sentences2;

        dgs[3].name = "Virgil";
        string[] sentences3 = {"We will make our reply to Chiron, there at your side. Your own will was always hasty. It has cost you before."};
        dgs[3].sentences = sentences3;

        dgs[11].name = "Virgil";
        string[] sentences35 = {"That is Nessus, who died for the beautiful Deianira and avenged his own death. The one in the middle, head bowed to his chest, is the great Chiron, who raised Achilles. The other is Pholus, who was so full of rage.",
        "Around the trench they go in their thousands, shooting any spirit who pulls himself out of the blood more than his guilt allows."};
        dgs[11].sentences = sentences35;

        dgs[4].name = "Chiron";
        string[] sentences4 = {"Have you noticed that the one who follows moves what he touches? The feet of the dead do not do that."};
        dgs[4].sentences = sentences4;

        dgs[5].name = "Virgil";
        string[] sentences5 = {"Yes, he is alive, and all alone, and I must show him this chasm’s murk. Necessity, not pleasure, brings us here. She who gave me this new duty came from singing Alleluia.",
        "He is no robber, nor am I the spirit of a thief, but by that Power through which I move step by step down so brutal a road, give us one of your band as an escort. Someone who could help us find the ford and ferry this one over on his back, since he is no spirit who walks upon air."};
        dgs[5].sentences = sentences5;

        dgs[6].name = "Chiron";
        string[] sentences6 = {"Go back and guide them. If another detachment bars your path, pull rank and make them yield."};
        dgs[6].sentences = sentences6;

        dgs[7].name = "Nessus";
        string[] sentences7 = {"These were tyrants eager to spill blood as they grabbed others’ goods. Here they lament their pitiless crimes. Alexander is here and cruel Dionysius who made Sicily endure decades of woe.",
        "That brow there that has hair so black is Ezzelino; and that other one, blond, is Obizzo of Este, who up in the world Was killed, in truth, by his bastard son."};
        dgs[7].sentences = sentences7;

        dgs[8].name = "Nessus";
        string[] sentences8 = {"That one clove, in the bosom of God’s church, the heart that still seeps blood on the Thames."};
        dgs[8].sentences = sentences8;

        dgs[9].name = "Nessus";
        string[] sentences9 = {"Just as you see that the boiling stream here grows ever more shallow, you must take my word for it that over there The bed of the stream gets deeper and deeper until it makes a circle and comes around to the place where tyranny groans forever.",
        "There Divine Justice stings Attila, who was a scourge on Earth, and torments eternally Pyrrhus and Sextus; and milks the tears, too, Of those brigands who made the roads run with blood, Rinier of Corneto and Rinier Pazzo, their eyes now unlocked by the boiling flood."};
        dgs[9].sentences = sentences9;
    }

    void initLavaBridge() {
        float x, y = -2.5f;

        for (x = 35.5f; x < 90.5f; x++) {
            Instantiate(lavaBridgeTile, new Vector3(x, y, 0f), Quaternion.identity);
        }
    }
}
 