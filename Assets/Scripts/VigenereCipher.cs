using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

/// <summary>
/// Module made by hockeygoalie78 and KingSlendy
/// Use the Vigenere Cipher to determine and submit the correct encoded word.
/// </summary>
public class VigenereCipher : MonoBehaviour {

    public KMBombInfo bombInfo;
    public KMAudio bombAudio;
    public TextMesh startWordMesh;
    public TextMesh keyWordMesh;
    public KMSelectable[] buttons;
    public KMSelectable submitButton;
    public TextMesh[] displayTextMesh;
    private int[] displayIndices;
    private List<char>[] displayLetters;
    private string startWord;
    private string keyWord;
    private string solutionWord;
    private string submissionWord;
    private int randomInt;
    private KMBombModule bombModule;

    void Start()
    {
        //Initialize letters
        displayLetters = new List<char>[6];
        for(int c = 0; c < 6; c++)
        {
            displayLetters[c] = new List<char>();
        }

        //Initialize start word
        //TODO: Pick start word randomly from list of words
        startWord = "ABCDEF";
        startWordMesh.text = startWord;

        //Initialize key word
        keyWord = "";
        for(int c = 0; c < Random.Range(3, 7); c++)
        {
            keyWord += (char)('A' + Random.Range(0, 26));
        }
        keyWordMesh.text = keyWord;

        //Determine solution
        solutionWord = "";
        for(int c = 0; c < 6; c++)
        {
            //Formula calculating the Vigenere Cipher's transition
            solutionWord += (char)(((startWord[c] - 'A') + (keyWord[c % keyWord.Length] - 'A')) % 26 + 'A');
        }
        //Debug.Log("Solution: " + solutionWord);

        //Add the correct character to the display letters
        for(int c = 0; c < 6; c++)
        {
            displayLetters[c].Add(solutionWord[c]);
        }

        //Add filler letters to the display letters
        //TODO: Optimize method to prevent brute force
        for(int c = 0; c < 6; c++)
        {
            for(int d = 0; d < 4; d++)
            {
                randomInt = Random.Range(0, 26);
                while(displayLetters[c].Contains((char)('A' + randomInt)))
                {
                    randomInt = Random.Range(0, 26);
                }
                displayLetters[c].Add((char)('A' + randomInt));
            }
        }

        //Set display indices
        displayIndices = new int[6];
        for(int c = 0; c < 6; c++)
        {
            displayIndices[c] = Random.Range(0, 5);
            displayTextMesh[c].text = displayLetters[c][displayIndices[c]].ToString();
        }

        //DEBUG
        /*for(int c = 0; c < 6; c++)
        {
            Debug.Log("Display " + c + ":");
            for(int d = 0; d < 5; d++)
            {
                Debug.Log("Letter " + d + ": " + displayLetters[c][d]);
            }
        }
        for(int c = 0; c < 6; c++)
        {
            Debug.Log("Index " + c + ": " + displayIndices[c]);
        }*/

        //Set delegates for the up and down buttons
        for(int c = 0; c < 6; c++)
        {
            int d = c;
            buttons[c].OnInteract += delegate { ChangeLetter(d, true); return false; };
        }
        for(int c = 6; c < 12; c++)
        {
            int d = c;
            buttons[c].OnInteract += delegate { ChangeLetter(d - 6, false); return false; };
        }

        //Set delegate for submit button
        submitButton.OnInteract += delegate { CheckSubmission(); return false; };

        //Other components
        bombModule = GetComponent<KMBombModule>();
    }

    /// <summary>
    /// Changes the letter index, and adjusts the text of the display
    /// </summary>
    /// <param name="index">Index of the display</param>
    /// <param name="up">True if the index should go up; false otherwise</param>
    private void ChangeLetter(int index, bool up)
    {
        //Movement/audio
        buttons[index + (6 * (up ? 0 : 1))].AddInteractionPunch(.2f);
        bombAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, transform);

        if(up)
        {
            displayIndices[index]++;
        }
        else
        {
            displayIndices[index] += 4;
        }
        displayIndices[index] %= 5;
        displayTextMesh[index].text = displayLetters[index][displayIndices[index]].ToString();
    }

    /// <summary>
    /// Checks if the submission is correct, and handles a strike or pass appropriately
    /// </summary>
    private void CheckSubmission()
    {
        //Movement/audio
        submitButton.AddInteractionPunch(.5f);
        bombAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, transform);

        //Get the submission word
        submissionWord = "";
        for(int c = 0; c < 6; c++)
        {
            submissionWord += displayLetters[c][displayIndices[c]];
        }

        //Handle pass or strike
        if(submissionWord == solutionWord)
        {
            bombModule.HandlePass();
        }
        else
        {
            bombModule.HandleStrike();
        }
    }
}
