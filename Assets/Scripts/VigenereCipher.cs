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

    private readonly char[][] charTable = new char[26][];
    private readonly List<char>[] displayLetters = new List<char>[6];
    private readonly int[] displayIndices = new int[6];

    private string solutionWord = "";

    bool moduleSolved;
    static int moduleIdCounter = 1;
    int moduleId;

    void Start() {
        moduleId = moduleIdCounter++;

        //Gets the rule seed RNG methods
        RNG = RuleSeedable.GetRNG();
        var charList = Enumerable.Range('A', 26).Select(x => (char)x).ToArray();

        //Generates the cipher table
        for (var c = 0; c < 26; c++) {
            charTable[c] = new char[26];
            charList = RNG.ShuffleFisherYates(charList);

            for (var d = 0; d < 26; d++) {
                charTable[c][d] = charList[d];
            }
        }

        //Initialize start word
        var startWord = StartWordMesh.text = wordList.Pick();
        Debug.LogFormat(@"[Cryptic Password #{0}] Starting word is: {1}", moduleId, startWord);

        //Initialize key word
        var keyWord = KeyWordMesh.text = charList.Shuffle().Take(Random.Range(3, 7)).Join("");
        Debug.LogFormat(@"[Cryptic Password #{0}] Key word is: {1}", moduleId, keyWord);

        //Determine solution
        for (var c = 0; c < 6; c++) {
            //Calculates the solution based on the table cipher
            solutionWord += charTable[keyWord[c % keyWord.Length] - 'A'][startWord[c] - 'A'];

            //Initialize letters
            displayLetters[c] = new List<char> {
                //Add the correct character to the display letters
                solutionWord[c]
            };

            //Add filler letters to the display letters
            displayLetters[c].AddRange(charList.Except(displayLetters[c]).ToArray().Shuffle().Take(4));

            //Set display indices
            displayIndices[c] = Random.Range(0, 5);
        }

        Debug.LogFormat(@"[Cryptic Password #{0}] Solution is: {1}", moduleId, solutionWord);

        //Set the displays to the proper letter
        for (var c = 0; c < 6; c++) {
            DisplaytextMesh[c].text = displayLetters[c][displayIndices[c]].ToString();
        }

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
        SubmitButton.AddInteractionPunch(0.5f);
        BombAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, transform);

        if (moduleSolved) return;

        var submissionWord = Enumerable.Range(0, 6).Select(x => displayLetters[x][displayIndices[x]]).Join("");

        //Handle pass if the current input is the same as the solution otherwise strike
        if (solutionWord.Equals(submissionWord)) {
            BombModule.HandlePass();
            moduleSolved = true;
            Debug.LogFormat(@"[Cryptic Password #{0}] Module solved!");
        } else {
            BombModule.HandleStrike();
            Debug.LogFormat(@"[Cryptic Password #{0}] That was incorrect. Submitted word was: {1}", moduleId, submissionWord);
        }
    }
}