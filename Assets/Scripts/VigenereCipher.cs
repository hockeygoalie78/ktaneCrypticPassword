using KModkit;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Module made by hockeygoalie78 and KingSlendy
/// Use a cipher similar to Vigenere to determine and submit the correct encoded word.
/// </summary>
public class VigenereCipher : MonoBehaviour {
    public KMAudio BombAudio;
    public KMBombInfo BombInfo;
    public KMBombModule BombModule;
    public KMRuleSeedable RuleSeedable;
    public KMSelectable[] Buttons;
    public KMSelectable SubmitButton;
    public TextMesh StartWordMesh, KeyWordMesh;
    public TextMesh[] DisplaytextMesh;

    static MonoRandom RNG;

    private readonly string[] wordList = {
        "PUZZLE", "ANSWER", "DEFUSE", "DANGER", "STRIKE", "EXPERT", "SELECT", "UNSURE", "BUTTON", "LIGHTS",
        "THINGS", "MODULE", "DISARM", "MANUAL", "ALARMS", "NUMBER", "LETTER", "ASSETS", "CIPHER", "CAESAR",
        "ATBASH", "DECODE", "FOLDER", "SHAPES", "EXPOSE", "PENCIL", "SUBMIT", "RANGES", "WINDOW", "VISUAL",
        "ATTACH", "SAVING", "SEARCH", "COLORS", "ARROWS", "SCREEN", "STATIC", "STRING", "LISTEN", "SOLVES",
        "EXCEPT", "ORDERS", "PUBLIC", "HOCKEY", "SLENDY", "SYSTEM", "MEMORY", "SKILLS", "SERIAL", "RANDOM"
    };

    private readonly char[][] charTable = new char[25][];
    private readonly List<char>[] displayLetters = new List<char>[6];
    private readonly int[] displayIndices = new int[6];
    private readonly char[] charReference =  Enumerable.Range('A', 25).Select(x => (char)x).ToArray();

    private string solutionWord = "";

    void Start() {
        //Gets the rule seed RNG methods
        RNG = RuleSeedable.GetRNG();
        var charList = charReference;

        //Generates the cipher table
        for (var c = 0; c < 25; c++) {
            charTable[c] = RNG.ShuffleFisherYates(charList);
        }

        //Initialize start word
        var startWord = wordList.Pick();
        StartWordMesh.text = startWord;

        //Initialize key word
        charList = charReference;
        var keyWord = charList.Shuffle().Take(Random.Range(3, 7)).Join("");
        KeyWordMesh.text = keyWord;

        //Determine solution
        charList = charReference;

        for (var c = 0; c < 6; c++) {
            //Formula calculating the Vigenere Cipher's transition
            solutionWord += charTable[keyWord[c % keyWord.Length] - 'A'][startWord[c] - 'A'];

            //Initialize letters
            displayLetters[c] = new List<char> {
                //Add the correct character to the display letters
                solutionWord[c]
            };

            //Add filler letters to the display letters
            displayLetters[c].AddRange(charList.Shuffle().Take(4));

            //Set display indices
            displayIndices[c] = Random.Range(0, 5);
        }

        for (var c = 0; c < 6; c++) {
            DisplaytextMesh[c].text = displayLetters[c][displayIndices[c]].ToString();
        }

        //Debug.Log("Solution: " + solutionWord);

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
        for (var c = 0; c < 12; c++) {
            var d = c;

            if (c < 6) {
                Buttons[c].OnInteract += delegate() {
                    ChangeLetter(d, true);

                    return false;
                };
            } else {
                Buttons[c].OnInteract += delegate() {
                    ChangeLetter(d - 6, false);

                    return false;
                };
            }
        }

        //Set delegate for submit button
        SubmitButton.OnInteract += delegate() {
            CheckSubmission();

            return false;
        };
    }

    /// <summary>
    /// Changes the letter index, and adjusts the text of the display
    /// </summary>
    private void ChangeLetter(int index, bool up) {
        //Movement/audio
        Buttons[index + 6 * up.ToInt()].AddInteractionPunch(0.2f);
        BombAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, transform);

        displayIndices[index] += up ? 1 : 4;
        displayIndices[index] %= 5;
        DisplaytextMesh[index].text = displayLetters[index][displayIndices[index]].ToString();
    }

    /// <summary>
    /// Checks if the submission is correct, and handles a strike or pass appropriately
    /// </summary>
    private void CheckSubmission() {
        //Movement/audio
        SubmitButton.AddInteractionPunch(.5f);
        BombAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, transform);

        //Handle pass if the current input is the same as the solution otherwise strike
        if (solutionWord.Equals(Enumerable.Range(0, 6).Select(x => displayLetters[displayIndices[x]]).Join(""))) {
            BombModule.HandlePass();
        } else {
            BombModule.HandleStrike();
        }
    }
}